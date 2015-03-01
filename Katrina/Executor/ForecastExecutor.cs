using System.Collections.Generic;
using System.IO;
using System.Linq;
using AV.Cyclone.Katrina.Executor.Interfaces;
using AV.Cyclone.Katrina.SyntaxProcessor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AV.Cyclone.Katrina.Executor
{
    public class ForecastExecutor
    {
        private readonly Solution solution;

        private static readonly MetadataReference assemblyLoader =
            MetadataReference.CreateFromAssembly(typeof (AssemblyLoader).Assembly);

        private Dictionary<Project, Project> projectMapping;
        private Project startupProject;
        private List<Project> projectReferences;

        public ForecastExecutor(Solution solution)
        {
            this.solution = solution;
        }

        public void SetStartupProject(string projectName)
        {
            startupProject = solution.Projects.First(p => p.Name == projectName);
            projectMapping = new Dictionary<Project, Project>();

            AddLogger();
        }

        public void AddLogger()
        {
            projectReferences = GetReferencedProjects(startupProject);
            foreach (var reference in projectReferences)
            {
                var updatedProject = RewriteProject(reference.AddMetadataReference(assemblyLoader));
                if (HasSyntaxErrors) return;
                projectMapping.Add(reference, updatedProject);
            }

            UpdateProjectReferences(projectReferences, projectMapping);
        }

        public bool HasSyntaxErrors { get; private set; }

        public CSharpCompilation[] GetCompilations()
        {
            if (HasSyntaxErrors) return null;
            return projectMapping.Values
                .Select(p => (CSharpCompilation)p.GetCompilationAsync().Result)
                .ToArray();
        }

        public List<ForecastItem> GetForecast()
        {
            var result = new List<ForecastItem>();
            foreach (var project in projectMapping.Values)
            {
                var compilation = project.GetCompilationAsync().Result;
                foreach (var document in project.Documents)
                {
                    var forecastItem = new ForecastItem
                    {
                        SyntaxTree = document.GetSyntaxTreeAsync().Result,
                        Compilation = (CSharpCompilation)compilation
                    };
                    result.Add(forecastItem);
                }
            }
            return result;
        }

        public string[] GetReferences()
        {
            var files = new HashSet<string>();
            foreach (var projectReference in projectMapping.Values)
            {
                var project = projectReference;
                foreach (var metadataReference in project.MetadataReferences
                    .OfType<PortableExecutableReference>()
                    .Where(a => !(a.FilePath.Contains("Microsoft.") || a.FilePath.Contains("System.") || a.FilePath.Contains("mscorlib.dll"))))
                {
                    var filePath = metadataReference.FilePath;
                    files.Add(filePath);
                }
            }
            return files.ToArray();
        }

        private static void UpdateProjectReferences(List<Project> references, Dictionary<Project, Project> projectMapping)
        {
            for (int i = references.Count - 1; i >= 0; i--)
            {
                var project = references[i];
                var changedProject = projectMapping[project];
                var projectReferences = project.ProjectReferences.ToList();
                foreach (var reference in projectReferences)
                {
                    changedProject = changedProject
                        .RemoveProjectReference(reference)
                        .AddProjectReference(new ProjectReference(projectMapping[project.Solution.GetProject(reference.ProjectId)].Id, reference.Aliases));
                    projectMapping[project] = changedProject;
                }
            }
        }

        private Project RewriteProject(Project project)
        {
            var logAssignmentRewriter = new AddExecuteLoggerVisitor();
            var documents = project.Documents;
            foreach (var document in documents)
            {
                var syntaxTree = document.GetSyntaxTreeAsync().Result;
                if (syntaxTree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    HasSyntaxErrors = true;
                    return null;
                }
                var newRoot = logAssignmentRewriter.Visit(syntaxTree.GetRoot());
                project = project.RemoveDocument(document.Id);
                project = project.AddDocument(document.Name, newRoot, document.Folders, document.FilePath).Project;
            }
            return project;
        }

        private static List<Project> GetReferencedProjects(Project project)
        {
            var projects = new List<Project>();
            GetReferencedProjectsRecursively(project, projects);
            return projects;
        }

        private static void GetReferencedProjectsRecursively(Project project, List<Project> references)
        {
            references.Add(project);
            var projects = project.ProjectReferences.ToList();
            foreach (var reference in projects)
            {
                var referenceProject = project.Solution.GetProject(reference.ProjectId);
                if (references.Contains(referenceProject))
                {
                    references.Remove(referenceProject);
                    references.Add(referenceProject);
                }
                else
                    GetReferencedProjectsRecursively(referenceProject, references);
            }
        }
    }
}
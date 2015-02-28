using System.Collections.Generic;
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

        public ForecastExecutor(Solution solution)
        {
            this.solution = solution;
        }

        public void SetStartupProject(string projectName)
        {
            startupProject = solution.Projects.First(p => p.Name == projectName);
            projectMapping = new Dictionary<Project, Project>();

            var projectReferences = GetReferencedProjects(startupProject);
            foreach (var reference in projectReferences)
            {
                var updatedProject = RewriteProject(reference.AddMetadataReference(assemblyLoader));
                projectMapping.Add(reference, updatedProject);
            }
            UpdateProjectReferences(projectReferences, projectMapping);
        }

        public CSharpCompilation[] GetCompilations()
        {
            return projectMapping.Values
                .Select(p => (CSharpCompilation)p.GetCompilationAsync().Result)
                .ToArray();
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

        private static Project RewriteProject(Project project)
        {
            var logAssignmentRewriter = new AddExecuteLoggerVisitor();
            var documents = project.Documents;
            foreach (var document in documents)
            {
                var syntaxTree = document.GetSyntaxTreeAsync().Result;
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
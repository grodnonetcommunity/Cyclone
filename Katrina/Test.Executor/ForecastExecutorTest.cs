using System.IO;
using System.Runtime.CompilerServices;
using AV.Cyclone.Katrina.Executor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Test.Executor
{
    public class ForecastExecutorTest
    {
        [Test]
        public void ComplexTest()
        {
            var source = @"
class Class
{
    public void Method()
    {
        var a = 1;
        var b = 2;
    }
}
";

            var workspace = new AdhocWorkspace();

            var solutionId = SolutionId.CreateNewId("ComplexTest");
            var versionStamp = new VersionStamp();
            workspace.AddSolution(SolutionInfo.Create(solutionId, versionStamp));

            var project1 = workspace.AddProject("Project1", LanguageNames.CSharp);
            project1 = project1
                .AddMetadataReference(CodeExecutorTest.Mscorelib)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            workspace.TryApplyChanges(project1.Solution);
            
            var project2 = workspace.AddProject("Project2", LanguageNames.CSharp);
            project2 = project2.AddProjectReference(new ProjectReference(project1.Id))
                .AddMetadataReference(CodeExecutorTest.Mscorelib)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            workspace.TryApplyChanges(project2.Solution);

            workspace.AddDocument(project2.Id, "File.cs", SourceText.From(source));

            var forecastExecutor = new ForecastExecutor(workspace.CurrentSolution);
            forecastExecutor.SetStartupProject("Project2");
            var forecastItems = forecastExecutor.GetForecast();

            var codeExecutor = new CodeExecutor();
            codeExecutor.Init(forecastItems);
            var executeLogger = new MockExecuteLogger();
            codeExecutor.SetExecuteLogger(executeLogger);
            codeExecutor.Execute("Project2", null, "Class", "Method");

            CollectionAssert.AreEqual(new[] { "a = 1", "b = 2" }, executeLogger.assigns);
        }

        [Test]
        public void RealSolutionTest()
        {
            var solutionPath = @"..\..\.TestSolution\TestSolution.sln";
            var projectName = "Test.Algorithms";
            var realSolutionPath = GetSolutionPath(solutionPath);

            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(realSolutionPath).Result.GetIsolatedSolution();

            var forecastExecutor = new ForecastExecutor(solution);
            forecastExecutor.SetStartupProject(projectName);
            var forecastItems = forecastExecutor.GetForecast();
            var files = forecastExecutor.GetReferences();

            var codeExecutor = new CodeExecutor();
            codeExecutor.Init(forecastItems);
            var executeLogger = new MockExecuteLogger();
            codeExecutor.SetExecuteLogger(executeLogger);
            codeExecutor.Execute(projectName, files, "Test.Algorithms.BinarySerchTest", "LessOrEqualRequired");

            // TODO: Add some asserts
        }

        public string GetSolutionPath(string solutionPath, [CallerFilePath] string fileName = null)
        {
            return Path.Combine(Path.GetDirectoryName(fileName), solutionPath);
        }
    }
}
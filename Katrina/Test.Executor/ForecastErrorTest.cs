using AV.Cyclone.Katrina.Executor;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

namespace Test.Executor
{
    public class ForecastErrorTest
    {
        [Test]
        public void SyntaxTest1()
        {
            var missedSemicolonSource = "class C { void M() { var a = 0 } }";
            var workspace = new AdhocWorkspace();

            var solutionId = SolutionId.CreateNewId("ErrorTest");
            var versionStamp = new VersionStamp();
            workspace.AddSolution(SolutionInfo.Create(solutionId, versionStamp));

            var project1 = workspace.AddProject("Project1", LanguageNames.CSharp);
            project1 = project1
                .AddMetadataReference(CodeExecutorTest.Mscorelib)
                .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            workspace.TryApplyChanges(project1.Solution);

            workspace.AddDocument(project1.Id, "File.cs", SourceText.From(missedSemicolonSource));

            var forecastExecutor = new ForecastExecutor(workspace.CurrentSolution);
            forecastExecutor.SetStartupProject("Project1");

            Assert.IsTrue(forecastExecutor.HasSyntaxErrors);
        }
    }
}
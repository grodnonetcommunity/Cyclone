using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AV.Cyclone.Katrina.Executor.Interfaces;
using AV.Cyclone.Sandy.Models.Operations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace AV.Cyclone.Katrina.Executor
{
    public class WeatherStation
    {
        private readonly ForecastExecutor forecastExecutor;
        private readonly string projectName;
        private readonly string startMethodDeclaration;
        private readonly string startTypeDeclaration;

        public WeatherStation(string solutionFile, string projectName, string fileName, int lineNumber)
        {
            this.projectName = projectName;
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionFile).Result.GetIsolatedSolution();

            SourceText sourceText;
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                sourceText = SourceText.From(fileStream);
            }
            var fileSyntaxTree = CSharpSyntaxTree.ParseText(sourceText);
            var root = fileSyntaxTree.GetRoot();
            startMethodDeclaration = null;
            foreach (var namespaceDeclaration in root.DescendantNodes().OfType<NamespaceDeclarationSyntax>())
            {
                foreach (var typeDeclaration in namespaceDeclaration.DescendantNodes().OfType<TypeDeclarationSyntax>())
                {
                    foreach (
                        var methodDeclaration in typeDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>())
                    {
                        var line = fileSyntaxTree.GetLineSpan(methodDeclaration.FullSpan);
                        if (line.StartLinePosition.Line >= lineNumber || line.EndLinePosition.Line <= lineNumber)
                        {
                            startMethodDeclaration = methodDeclaration.Identifier.Text;
                            startTypeDeclaration = namespaceDeclaration.Name + "." + typeDeclaration.Identifier.Text;
                            break;
                        }
                    }
                }
            }

            if (startMethodDeclaration == null || startTypeDeclaration == null)
            {
                throw new Exception(string.Format("Can't find method in line: {0}", lineNumber));
            }

            forecastExecutor = new ForecastExecutor(solution);
        }

        public void Start()
        {
            StartThread();
        }

        public Operation[][] GetOperations(string fileName)
        {
            return null;
        }

        private void StartThread()
        {
            new Thread(BackgroundExecutor)
            {
                IsBackground = true
            };
        }

        private void BackgroundExecutor()
        {
            Execute();
        }

        private void Execute()
        {
            var compilations = forecastExecutor.GetCompilations();
            if (compilations == null) return;
            var files = forecastExecutor.GetReferences();

            var codeExecutor = new CodeExecutor();
            codeExecutor.AddCompilations(compilations);
            var executeLogger = new OperationsExecuteLogger();
            codeExecutor.SetExecuteLogger(executeLogger);
            codeExecutor.Execute(projectName, files, startTypeDeclaration, startMethodDeclaration);

            UpdateOperations(executeLogger.MethodCalls);
        }

        private void UpdateOperations(Dictionary<MethodReference, List<List<Operation>>> methodCalls)
        {
        }
    }
}
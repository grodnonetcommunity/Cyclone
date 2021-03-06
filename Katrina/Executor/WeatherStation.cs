﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AV.Cyclone.Katrina.Executor.Interfaces;
using AV.Cyclone.Katrina.SyntaxProcessor;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.Models.Operations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace AV.Cyclone.Katrina.Executor
{
    public class WeatherStation : IDisposable
    {
        private readonly ForecastExecutor forecastExecutor;
        private readonly string projectName;
        private string startMethodDeclaration;
        private string startTypeDeclaration;
        private Dictionary<MethodReference, List<List<Operation>>> methodCalls; 
        private object changesSync = new object();
        private Dictionary<string, string> changes = new Dictionary<string, string>();
        private bool disposed;
        private Thread backgroundThread;
        private readonly AutoResetEvent waitChanges = new AutoResetEvent(false);
        private CodeExecutor codeExecutor;

        public event EventHandler Executed;

        public WeatherStation(string solutionFile, string projectName, string fileName, int lineNumber)
        {
            this.projectName = projectName;
            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionFile).Result.GetIsolatedSolution();

            InitClassNameAndMethodName(fileName, lineNumber);

            if (startMethodDeclaration == null || startTypeDeclaration == null)
            {
                throw new Exception(string.Format("Can't find method in line: {0}", lineNumber));
            }

            forecastExecutor = new ForecastExecutor(solution);
        }

        private void InitClassNameAndMethodName(string fileName, int lineNumber)
        {
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
                        if (lineNumber >= line.StartLinePosition.Line && lineNumber <= line.EndLinePosition.Line)
                        {
                            startMethodDeclaration = methodDeclaration.Identifier.Text;
                            startTypeDeclaration = namespaceDeclaration.Name + "." + typeDeclaration.Identifier.Text;
                            break;
                        }
                    }
                }
            }
        }

        public void Start()
        {
            StartThread();
        }

        public void FileUpdated(string fileName, string content)
        {
            lock (changesSync)
            {
                changes[fileName] = content;
            }
            waitChanges.Set();
        }

        public Dictionary<string, List<List<Operation>>> GetMethodCalls(string fileName)
        {
            var fileMethodCalls = methodCalls.Where(e => e.Key.FileName == fileName).ToList();
            if (fileMethodCalls.Count == 0) return null;
            return fileMethodCalls.ToDictionary(e => e.Key.MethodName, e => e.Value);
        }

        private void StartThread()
        {
            backgroundThread = new Thread(BackgroundExecutor)
            {
                IsBackground = true
            };
            backgroundThread.Start();
        }

        private void BackgroundExecutor()
        {
            forecastExecutor.SetStartupProject(projectName);

            codeExecutor = new CodeExecutor();
            codeExecutor.Init(forecastExecutor.GetForecast());

            do
            {
                if (ApplyChanges())
                {
                    var executeLogger = new OperationsExecuteLogger();
                    codeExecutor.SetExecuteLogger(executeLogger);

                    var executeThread = new Thread(Execute);
                    executeThread.Start();
                    if (!executeThread.Join(TimeSpan.FromSeconds(50)))
                    {
                        executeThread.Abort();
                    }
                    UpdateMethodCalls(executeLogger.MethodCalls);
                    OnExecuted();
                }
                waitChanges.WaitOne();
            } while (!disposed);
        }

        private bool ApplyChanges()
        {
            Dictionary<string, string> changesCopy;
            var result = true;
            lock (changesSync)
            {
                changesCopy = changes;
                changes = new Dictionary<string, string>();
            }
            foreach (var fileChanges in changesCopy)
            {
                result &= ApplyChanges(fileChanges.Key, fileChanges.Value);
            }
            return result;
        }

        private bool ApplyChanges(string fileName, string content)
        {
            var newSyntaxTree = CSharpSyntaxTree.ParseText(content).WithFilePath(fileName);
            if (newSyntaxTree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) return false;
            var newSyntaxTreeRoot = newSyntaxTree.GetRoot();
            var visitor = new AddExecuteLoggerVisitor();
            newSyntaxTreeRoot = visitor.Visit(newSyntaxTreeRoot);
            codeExecutor.UpdateFile(fileName, CSharpSyntaxTree.Create((CSharpSyntaxNode)newSyntaxTreeRoot, encoding: Encoding.UTF8).WithFilePath(fileName));
            return true;
        }

        private void Execute()
        {
            var files = forecastExecutor.GetReferences();

            codeExecutor.Execute(projectName, files, startTypeDeclaration, startMethodDeclaration);
        }

        private void UpdateMethodCalls(Dictionary<MethodReference, List<List<Operation>>> methodCalls)
        {
            this.methodCalls = methodCalls;
        }

        protected virtual void OnExecuted()
        {
            var executed = Executed;
            if (executed != null)
                executed.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            backgroundThread.Interrupt();
            disposed = true;
        }
    }
}
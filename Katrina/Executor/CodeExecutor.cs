﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using AV.Cyclone.Katrina.Executor.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace AV.Cyclone.Katrina.Executor
{
    public class CodeExecutor
    {
        private class CompilationEmitResult
        {
            public byte[] RawAssembly { get; set; }
            
            public byte[] PdbAssembly { get; set; }

            public EmitResult EmitResult { get; set; }
        }

        private readonly HashSet<CSharpCompilation> compilations =
            new HashSet<CSharpCompilation>();

        private List<CSharpCompilation> executeCompilations;
        private List<CompilationEmitResult> compilationEmitResults;
        private readonly Dictionary<string, ForecastItem> forecastItems = new Dictionary<string, ForecastItem>();
        private IExecuteLogger currentExecuteLogger;

        public void Init(IEnumerable<ForecastItem> items)
        {
            compilations.Clear();
            forecastItems.Clear();
            foreach (var forecastItem in items)
            {
                forecastItems[forecastItem.SyntaxTree.FilePath] = forecastItem;
                compilations.Add(forecastItem.Compilation);
            }
            executeCompilations = compilations.ToList();
        }

        public void UpdateFile(string fileName, SyntaxTree newSyntaxTree)
        {
            ForecastItem forecastItem;
            if (!forecastItems.TryGetValue(fileName, out forecastItem)) return;
            if (newSyntaxTree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) return;

            var oldCompilation = forecastItem.Compilation;
            var oldSyntaxTree = forecastItem.SyntaxTree;

            var newCompilation = oldCompilation.ReplaceSyntaxTree(oldSyntaxTree, newSyntaxTree);
            if (newCompilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) return;
            forecastItem.SyntaxTree = newSyntaxTree;
            UpdateCompilation(oldCompilation, newCompilation);
        }

        public void UpdateFile(string fileName, string content)
        {
            ForecastItem forecastItem;
            if (!forecastItems.TryGetValue(fileName, out forecastItem)) return;
            var newSyntaxTree = CSharpSyntaxTree.ParseText(content, encoding: Encoding.UTF8).WithFilePath(fileName);
            if (newSyntaxTree.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) return;

            var oldCompilation = forecastItem.Compilation;
            var oldSyntaxTree = forecastItem.SyntaxTree;

            var newCompilation = oldCompilation.ReplaceSyntaxTree(oldSyntaxTree, newSyntaxTree);
            if (newCompilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error)) return;
            forecastItem.SyntaxTree = newSyntaxTree;
            UpdateCompilation(oldCompilation, newCompilation);
        }

        private void UpdateCompilation(CSharpCompilation oldCompilation, CSharpCompilation newCompilation)
        {
            compilations.Remove(oldCompilation);
            compilations.Add(newCompilation);
            executeCompilations = compilations.ToList();
            foreach (var forecastItem in forecastItems.Values)
            {
                if (forecastItem.Compilation == oldCompilation)
                {
                    forecastItem.Compilation = newCompilation;
                }
            }
        }

        public void SetExecuteLogger(IExecuteLogger executeLogger)
        {
            this.currentExecuteLogger = executeLogger;
        }

        public void Execute(string compilationName, string[] files, string className, string methodName)
        {
            AppDomain executorDomain = null;
            try
            {
                compilationEmitResults = new List<CompilationEmitResult>(executeCompilations.Count);

                foreach (var compilation in executeCompilations)
                {
                    using (var assemblyMemoryStream = new MemoryStream())
                    using (var pdbMemoryStream = new MemoryStream())
                    {
                        var emitResult = compilation.Emit(assemblyMemoryStream, pdbMemoryStream);
                        if (!emitResult.Success) return;
                        compilationEmitResults.Add(new CompilationEmitResult
                        {
                            RawAssembly = assemblyMemoryStream.ToArray(),
                            PdbAssembly = pdbMemoryStream.ToArray(),
                            EmitResult = emitResult
                        });
                    }
                }

                executorDomain = AppDomain.CreateDomain("ExecutorDomain", null);

                AppDomain.CurrentDomain.AssemblyResolve += ExecutorInterfacesAssemblyResolve;
                var loader =
                    (AssemblyLoader)
                        executorDomain.CreateInstanceFromAndUnwrap(typeof (AssemblyLoader).Assembly.Location,
                            typeof (AssemblyLoader).FullName);
                AppDomain.CurrentDomain.AssemblyResolve -= ExecutorInterfacesAssemblyResolve;

                foreach (var compilationEmitResult in compilationEmitResults)
                {
                    loader.LoadAssembly(compilationEmitResult.RawAssembly, compilationEmitResult.PdbAssembly);
                }

                var executorInterfacesWasLoaded = false;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var assemblyName = AssemblyName.GetAssemblyName(file);
                        if (assemblyName.FullName == typeof(AssemblyLoader).Assembly.FullName)
                            executorInterfacesWasLoaded = true;
                        loader.LoadAssembly(assemblyName);
                    }
                }
                if (!executorInterfacesWasLoaded)
                {
                    loader.LoadAssembly(typeof(AssemblyLoader).Assembly.GetName());
                }

                loader.SetExecuteLogger(new DomainExecuteLogger(currentExecuteLogger));
                try
                {
                    loader.Execute(compilationName, className, methodName);
                }
                catch (TargetInvocationException)
                {
                }
            }
            finally
            {
                if (executorDomain != null)
                {
                    AppDomain.Unload(executorDomain);
                }
            }
        }

        private Assembly ExecutorInterfacesAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var executorInterfaces = typeof(AssemblyLoader).Assembly;
            return args.Name == executorInterfaces.GetName().FullName ? executorInterfaces : null;
        }
    }
}
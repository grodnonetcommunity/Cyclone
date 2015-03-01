﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
            public string AssemblyPath { get; set; }

            public EmitResult EmitResult { get; set; }
        }

        private readonly HashSet<CSharpCompilation> compilations =
            new HashSet<CSharpCompilation>();
        private List<CompilationEmitResult> compilationEmitResults;
        private readonly Dictionary<string, ForecastItem> forecastItems = new Dictionary<string, ForecastItem>();

        public void Init(IEnumerable<ForecastItem> items)
        {
            compilations.Clear();
            foreach (var forecastItem in items)
            {
                forecastItems.Add(forecastItem.SyntaxTree.FilePath, forecastItem);
                compilations.Add(forecastItem.Compilation);
            }
        }

        public void UpdateFile(string fileName, string content)
        {
            ForecastItem forecastItem;
            if (!forecastItems.TryGetValue(fileName, out forecastItem)) return;
            var newSyntaxTree = CSharpSyntaxTree.ParseText(content);
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
            foreach (var forecastItem in forecastItems.Values)
            {
                if (forecastItem.Compilation == oldCompilation)
                {
                    forecastItem.Compilation = newCompilation;
                }
            }
            compilations.Remove(oldCompilation);
            compilations.Add(newCompilation);
        }

        public void SetExecuteLogger(IExecuteLogger executeLogger)
        {
            Context.ExecuteLogger = executeLogger;
        }

        public void Execute(string compilationName, string[] files, string className, string methodName)
        {
            string tempDir = null;
            AppDomain executorDomain = null;
            try
            {
                tempDir = Path.Combine(Path.GetTempPath(), "_Cyclon_" + Guid.NewGuid());
                Directory.CreateDirectory(tempDir);
                compilationEmitResults = new List<CompilationEmitResult>(compilations.Count);

                foreach (var compilation in compilations)
                {
                    var assemblyPath = Path.Combine(tempDir, compilation.AssemblyName + ".dll");
                    // TODO: Store emit results
                    var emitResult = compilation.Emit(assemblyPath);
                    compilationEmitResults.Add(new CompilationEmitResult
                    {
                        AssemblyPath = assemblyPath,
                        EmitResult = emitResult
                    });
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        File.Copy(file, Path.Combine(tempDir, Path.GetFileName(file)));
                    }
                }

                var assemblyLoaderAssemblyFileName = Path.Combine(tempDir, typeof(AssemblyLoader).Assembly.GetName().Name + ".dll");
                if (!File.Exists(assemblyLoaderAssemblyFileName))
                    File.Copy(typeof (AssemblyLoader).Assembly.Location, assemblyLoaderAssemblyFileName);

                executorDomain = AppDomain.CreateDomain("ExecutorDomain", AppDomain.CurrentDomain.Evidence,
                    new AppDomainSetup {ApplicationBase = tempDir});
                var loader =
                    (AssemblyLoader)
                        executorDomain.CreateInstanceAndUnwrap(typeof (AssemblyLoader).Assembly.FullName,
                            typeof (AssemblyLoader).FullName);

                int classAssemblyIndex = -1;
                for (int i = 0; i < compilationEmitResults.Count; i++)
                {
                    var compilationEmitResult = compilationEmitResults[i];
                    var assemblyName = AssemblyName.GetAssemblyName(compilationEmitResult.AssemblyPath);
                    loader.LoadAssembly(assemblyName);
                    if (assemblyName.Name == compilationName)
                        classAssemblyIndex = i;
                }

                loader.SetExecuteLogger(new DomainExecuteLogger(Context.ExecuteLogger));
                loader.Execute(classAssemblyIndex, className, methodName);
            }
            finally
            {
                if (executorDomain != null)
                {
                    AppDomain.Unload(executorDomain);
                }
                if (!string.IsNullOrEmpty(tempDir))
                {
                    try
                    {
                        var tempDirInfo = new DirectoryInfo(tempDir);
                        tempDirInfo.Delete(true);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine(string.Format("Delete temp folder {0} failed", tempDir));
                    }
                }
            }
        }
    }
}
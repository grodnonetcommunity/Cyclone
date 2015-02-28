using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly Dictionary<CSharpCompilation, CSharpCompilation> compilations = 
            new Dictionary<CSharpCompilation, CSharpCompilation>();
        private List<CompilationEmitResult> compilationEmitResults;

        public void AddCompilation(CSharpCompilation oldCompilation, CSharpCompilation newCompilation)
        {
            if (oldCompilation == null)
                oldCompilation = newCompilation;
            compilations[oldCompilation] = newCompilation;
        }

        public void Emit()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "_Cyclon_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);

            compilationEmitResults = new List<CompilationEmitResult>(compilations.Count);

            foreach (var compilation in compilations.Values)
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
        }

        public void SetExecuteLogger(IExecuteLogger executeLogger)
        {
            Context.ExecuteLogger = executeLogger;
        }

        public void Execute(string compilationName, string className, string methodName)
        {
            // TODO: Execute code in separate AppDomain
            Assembly classAssembly = null;
            foreach (var compilationEmitResult in compilationEmitResults)
            {
                var assemblyName = AssemblyName.GetAssemblyName(compilationEmitResult.AssemblyPath);
                var assembly = Assembly.Load(assemblyName);
                if (assemblyName.Name == compilationName)
                    classAssembly = assembly;
            }

            var type = classAssembly.GetType(className, true);
            var method = type.GetMethod(methodName);

            object target = null;
            if (!method.IsStatic)
                target = Activator.CreateInstance(type);

            method.Invoke(target, null);
        }
    }
}
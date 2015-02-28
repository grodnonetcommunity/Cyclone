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
        private string tempDir;

        public void AddCompilation(CSharpCompilation oldCompilation, CSharpCompilation newCompilation)
        {
            if (oldCompilation == null)
                oldCompilation = newCompilation;
            //newCompilation.AddReferences(MetadataReference.CreateFromAssembly(typeof (AssemblyLoader).Assembly));
            compilations[oldCompilation] = newCompilation;
        }

        public void Emit()
        {
            tempDir = Path.Combine(Path.GetTempPath(), "_Cyclon_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            File.Copy(typeof(AssemblyLoader).Assembly.Location, Path.Combine(tempDir, typeof(AssemblyLoader).Assembly.GetName().Name + ".dll"));

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
            var executorDomain = AppDomain.CreateDomain("ExecutorDomain", AppDomain.CurrentDomain.Evidence, new AppDomainSetup {ApplicationBase = tempDir});
            var loader = (AssemblyLoader)executorDomain.CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, typeof(AssemblyLoader).FullName);

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
            AppDomain.Unload(executorDomain);
        }
    }
}
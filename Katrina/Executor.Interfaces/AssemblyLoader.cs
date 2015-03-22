using System;
using System.Collections.Generic;
using System.Reflection;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();

        public int LoadAssembly(AssemblyName assemblyName)
        {
            var index = assemblies.Count;
            assemblies.Add(Assembly.Load(assemblyName));
            return index;
        }

        public void SetExecuteLogger(IExecuteLoggerHelper executeLogger)
        {
            Context.ExecuteLoggerHelper = executeLogger;
        }

        public void Execute(int assemblyIndex, string className, string methodName)
        {
            var assembly = assemblies[assemblyIndex];
            var type = assembly.GetType(className, true);
            var method = type.GetMethod(methodName);

            object target = null;
            if (!method.IsStatic)
                target = Activator.CreateInstance(type);

            method.Invoke(target, null);
        }
    }
}
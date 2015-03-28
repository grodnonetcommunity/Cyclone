using System;
using System.Collections.Generic;
using System.Reflection;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();

        public AssemblyName LoadAssembly(AssemblyName assemblyName)
        {
            return AddAssembly(Assembly.Load(assemblyName));
        }

        public AssemblyName LoadAssembly(byte[] rawAssembly)
        {
            return AddAssembly(Assembly.Load(rawAssembly));
        }

        private AssemblyName AddAssembly(Assembly assembly)
        {
            assemblies.Add(assembly);
            return assembly.GetName();
        }

        public void SetExecuteLogger(IExecuteLogger executeLogger)
        {
            var serializerExecuteLogger = new SerializerExecuteLogger(executeLogger);
            Context.ExecuteLoggerHelper = new BaseExecuteLoggerHelper(serializerExecuteLogger);
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
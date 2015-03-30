using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();

        public AssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return assemblies.FirstOrDefault(a => a.GetName().FullName == args.Name);
        }

        public AssemblyName LoadAssembly(AssemblyName assemblyName)
        {
            return AddAssembly(Assembly.Load(assemblyName));
        }

        public AssemblyName LoadAssembly(byte[] rawAssembly, byte[] pdbAssembly)
        {
            return AddAssembly(Assembly.Load(rawAssembly, pdbAssembly));
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

        public void Execute(string assemblyName, string className, string methodName)
        {
            var assembly = assemblies.First(a => a.GetName().Name == assemblyName);
            var type = assembly.GetType(className, true);
            var method = type.GetMethod(methodName);

            object target = null;
            if (!method.IsStatic)
                target = Activator.CreateInstance(type);

            try
            {
                method.Invoke(target, null);
            }
            catch (TargetInvocationException e)
            {
                var stackTraceString = e.InnerException.StackTrace;
                var stackTrace = new StackTrace(e.InnerException, true);
                var frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
                var line = frame.GetFileLineNumber();
                var fileName = frame.GetFileName();
            }
        }
    }
}
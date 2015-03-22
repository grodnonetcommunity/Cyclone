using System;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public class SerializerExecuteLogger : IExecuteLogger
    {
        private readonly IExecuteLogger executeLogger;

        public SerializerExecuteLogger(IExecuteLogger executeLogger)
        {
            this.executeLogger = executeLogger;
        }

        public void LogAssign(string expression, string fileNme, int lineNumber, object value)
        {
            executeLogger.LogAssign(expression, fileNme, lineNumber, SerializeValue(value));
        }

        public void BeginMethod(string methodName, string fileName, int lineNumber)
        {
            executeLogger.BeginMethod(methodName, fileName, lineNumber);
        }

        public void EndMethod(string methodName, string fileName, int lineNumber)
        {
            executeLogger.EndMethod(methodName, fileName, lineNumber);
        }

        public void BeginLoop(string fileName, int lineNumber)
        {
            executeLogger.BeginLoop(fileName, lineNumber);
        }

        public void LoopIteration(string fileName, int lineNumber)
        {
            executeLogger.LoopIteration(fileName, lineNumber);
        }

        public void EndLoop(string fileName, int lineNumber)
        {
            executeLogger.EndLoop(fileName, lineNumber);
        }

        private static object SerializeValue(object value)
        {
            if (value == null) return null;
            var type = value.GetType();
            if (IsSupportedType(type)) return value;
            return value.ToString();
        }

        private static bool IsSupportedType(Type type)
        {
            if (type.IsPrimitive) return true;
            if (type.IsArray) return IsSupportedType(type.GetElementType());
            if (type == typeof(string)) return true;
            return false;
        }
    }
}
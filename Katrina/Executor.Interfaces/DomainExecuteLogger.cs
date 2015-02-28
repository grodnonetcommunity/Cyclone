using System;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public class DomainExecuteLogger : MarshalByRefObject, IExecuteLogger
    {
        private readonly IExecuteLogger executeLogger;

        public DomainExecuteLogger(IExecuteLogger executeLogger)
        {
            this.executeLogger = executeLogger;
        }

        public T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            return executeLogger.LogAssign(expression, fileNme, lineNumber, value);
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
    }
}
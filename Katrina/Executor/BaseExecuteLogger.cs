using AV.Cyclone.Katrina.Executor.Interfaces;

namespace AV.Cyclone.Katrina.Executor
{
    public class BaseExecuteLogger : IExecuteLogger
    {
        public virtual T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            return value;
        }

        public virtual void BeginLoop(string fileName, int lineNumber)
        {
        }

        public virtual void LoopIteration(string fileName, int lineNumber)
        {
        }

        public virtual void EndLoop(string fileName, int lineNumber)
        {
        }

        public virtual void BeginMethod(string methodName, string fileName, int lineNumber)
        {
        }

        public virtual void EndMethod(string methodName, string fileName, int lineNumber)
        {
        }
    }
}
using AV.Cyclone.Katrina.Executor.Interfaces;

namespace AV.Cyclone.Katrina.Executor
{
    public class BaseExecuteLogger : IExecuteLogger
    {
        public virtual T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            return value;
        }

        public void BeginLoop(string fileName, int lineNumber)
        {
        }

        public void LoopIteration(string fileName, int lineNumber)
        {
        }

        public void EndLoop(string fileName, int lineNumber)
        {
        }
    }
}
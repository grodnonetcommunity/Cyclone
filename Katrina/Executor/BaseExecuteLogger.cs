using AV.Cyclone.Katrina.Executor.Interfaces;

namespace AV.Cyclone.Katrina.Executor
{
    public class BaseExecuteLogger : IExecuteLogger
    {
        public static BaseExecuteLogger Instance { get; set; }

        public virtual T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            return value;
        }
    }
}
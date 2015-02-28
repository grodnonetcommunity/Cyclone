using AV.Cyclone.Katrina.Executor.Interfaces;

namespace AV.Cyclone.Katrina.Executor
{
    public class FakeExecuteLogger : IExecuteLogger
    {
        public static FakeExecuteLogger Instance { get; set; }

        public T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            return value;
        }
    }
}
namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public interface IExecuteLogger
    {
        void LogAssign(string expression, string fileNme, int lineNumber, object value);

        void BeginMethod(string methodName, string fileName, int lineNumber);

        void EndMethod(string methodName, string fileName, int lineNumber);

        void BeginLoop(string fileName, int lineNumber);

        void LoopIteration(string fileName, int lineNumber);

        void EndLoop(string fileName, int lineNumber);
    }
}
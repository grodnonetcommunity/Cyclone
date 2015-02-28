namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public interface IExecuteLogger
    {
        T LogAssign<T>(string expression, string fileNme, int lineNumber, T value);
    }
}
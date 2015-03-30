using System.Collections.Generic;

namespace AV.Cyclone.Katrina.Executor.Interfaces
{
    public interface IExecuteLoggerHelper
    {
        T LogAssign<T>(string expression, string fileNme, int lineNumber, T value);

        T LogPostIncrement<T>(string expression, string fileNme, int lineNumber, T result, T value);

        void BeginMethod(string methodName, string fileName, int lineNumber);

        void EndMethod(string methodName, string fileName, int lineNumber);

        void BeginLoop(string fileName, int lineNumber);

        void LoopIteration(string fileName, int lineNumber);

        T LoopIteration<T>(string expression, string fileName, int lineNumber, T value);

        void EndLoop(string fileName, int lineNumber);
    }
}
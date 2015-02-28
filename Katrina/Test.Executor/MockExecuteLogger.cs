using System.Collections.Generic;
using AV.Cyclone.Katrina.Executor.Interfaces;

namespace Test.Executor
{
    public class MockExecuteLogger : IExecuteLogger
    {
        public readonly List<string> assigns = new List<string>();

        public T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            assigns.Add(string.Format("{0} = {1}", expression, value));
            return value;
        }

        public void BeginMethod(string methodName, string fileName, int lineNumber)
        {
        }

        public void EndMethod(string methodName, string fileName, int lineNumber)
        {
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
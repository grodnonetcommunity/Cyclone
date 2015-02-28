using System.Collections.Generic;
using AV.Cyclone.Katrina.Executor.Interfaces;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Katrina.Executor
{
    public class OperationsExecuteLogger : BaseExecuteLogger
    {
        private readonly Stack<List<Operation>> executeStack = new Stack<List<Operation>>();
        private List<Operation> currentOperations;

        public Dictionary<MethodReference, List<List<Operation>>> MethodCalls { get; } = new Dictionary<MethodReference, List<List<Operation>>>();

        public override void BeginMethod(string methodName, string fileName, int lineNumber)
        {
            executeStack.Push(currentOperations);
            currentOperations = new List<Operation>();
        }

        public override void EndMethod(string methodName, string fileName, int lineNumber)
        {
            var methodOperations = GetMethodOperations(methodName, fileName);
            methodOperations.Add(currentOperations);
            currentOperations = executeStack.Pop();
        }

        public override T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            var assignOperation = new AssignOperation
            {
                FileName = fileNme,
                LineNumber = lineNumber,
                VariableName = expression,
                VariableValue = value
            };
            currentOperations.Add(assignOperation);
            return base.LogAssign(expression, fileNme, lineNumber, value);
        }

        private List<List<Operation>> GetMethodOperations(string methodName, string fileName)
        {
            var methodReference = new MethodReference(fileName, methodName);
            List<List<Operation>> list;
            if (MethodCalls.TryGetValue(methodReference, out list))
                return list;
            list = new List<List<Operation>>();
            MethodCalls.Add(methodReference, list);
            return list;
        }
    }
}
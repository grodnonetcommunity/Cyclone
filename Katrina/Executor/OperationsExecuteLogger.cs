using System.Collections.Generic;
using AV.Cyclone.Katrina.Executor.Interfaces;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Katrina.Executor
{
    public class OperationsExecuteLogger : IExecuteLogger
    {
        private readonly Stack<OperationBuilder> executeStack = new Stack<OperationBuilder>();
        private OperationBuilder currentBuilder;

        public Dictionary<MethodReference, List<List<Operation>>> MethodCalls { get; } = new Dictionary<MethodReference, List<List<Operation>>>();

        public T LogAssign<T>(string expression, string fileNme, int lineNumber, T value)
        {
            var assignOperation = new AssignOperation
            {
                FileName = fileNme,
                LineNumber = lineNumber,
                VariableName = expression,
                VariableValue = value
            };
            currentBuilder.Add(assignOperation);
            return value;
        }

        public void BeginMethod(string methodName, string fileName, int lineNumber)
        {
            executeStack.Push(currentBuilder);
            currentBuilder = new MethodOperationsBuilder(fileName);
        }

        public void EndMethod(string methodName, string fileName, int lineNumber)
        {
            var methodOperations = GetMethodOperations(methodName, fileName);
            methodOperations.Add(currentBuilder.Operations);
            currentBuilder = executeStack.Pop();
        }

        public void BeginLoop(string fileName, int lineNumber)
        {
            executeStack.Push(currentBuilder);
            currentBuilder = new LoopOperationBuilder();
        }

        public void LoopIteration(string fileName, int lineNumber)
        {
            if (currentBuilder is LoopOperationBuilder)
            {
                BeginLoopIteration(fileName, lineNumber);
            }
            else
            {
                EndLoopIteration(fileName, lineNumber);
                BeginLoopIteration(fileName, lineNumber);
            }
        }

        private void BeginLoopIteration(string fileName, int lineNumber)
        {
            executeStack.Push(currentBuilder);
            currentBuilder = new LoopIterationOperationBuilder();
        }

        private void EndLoopIteration(string fileName, int lineNumber)
        {
            var loopIterationOperationBuilder = (LoopIterationOperationBuilder)currentBuilder;
            currentBuilder = executeStack.Pop();
            var loppOperationBuilder = (LoopOperationBuilder)currentBuilder;
            loppOperationBuilder.AddIteration(loopIterationOperationBuilder);
        }

        public void EndLoop(string fileName, int lineNumber)
        {
            if (currentBuilder is LoopIterationOperationBuilder)
            {
                EndLoopIteration(fileName, lineNumber);
            }
            var loopBuilder = (LoopOperationBuilder)currentBuilder;
            currentBuilder = executeStack.Pop();
            currentBuilder.Add(loopBuilder.Build(fileName, lineNumber));
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
using System.Collections.Generic;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Katrina.Executor
{
    public class LoopOperationBuilder : OperationBuilder
    {
        public LoopOperationBuilder()
        {
            this.Iterations = new List<LoopIterationOperationBuilder>();
        }

        public List<LoopIterationOperationBuilder> Iterations { get; private set; }

        public int Iteration { get; set; }

        public void AddIteration(LoopIterationOperationBuilder iteration)
        {
            Iterations.Add(iteration);
        }

        public LoopOperation Build()
        {
            return new LoopOperation();
        }

        public LoopOperation Build(string fileName, int lineNumber)
        {
            var operations = new Dictionary<int, IList<Operation>>();
            for (var i = 0; i < Iterations.Count; i++)
            {
                var iteration = Iterations[i];
                operations.Add(i, iteration.Operations);
            }
            return new LoopOperation
            {
                FileName = fileName,
                LineNumber = lineNumber,
                Operations = operations
            };
        }
    }
}
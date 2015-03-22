using System.Collections.Generic;
using System.Linq;
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
            var operations = Iterations.Select(iteration => (IList<Operation>)iteration.Operations).ToList();
            return new LoopOperation
            {
                FileName = fileName,
                LineNumber = lineNumber,
                Operations = operations
            };
        }
    }
}

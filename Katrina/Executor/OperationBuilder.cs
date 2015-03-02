using System.Collections.Generic;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Katrina.Executor
{
    public class OperationBuilder
    {
        public List<Operation> Operations { get; private set; }

        public OperationBuilder()
        {
            Operations = new List<Operation>();
        }

        public void Add(Operation operation)
        {
            Operations.Add(operation);
        }
    }
}
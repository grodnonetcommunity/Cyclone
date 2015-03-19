using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class ExecuteTreeLineItem
    {
        public abstract string Variable { get; }

        public abstract string DebuggerDisplay { get; }
    }

    public class ListExecuteTreeLineItem : ExecuteTreeLineItem
    {
        private readonly string variable;
        private readonly SparseArray<ExecuteTreeLineItem> items;

        public ListExecuteTreeLineItem(string variable)
        {
            this.variable = variable;
            this.items = new SparseArray<ExecuteTreeLineItem>(line => new ListExecuteTreeLineItem(this.variable));
        }

        public override string Variable
        {
            get { return variable; }
        }

        public void Add(int iteration, ExecuteTreeLineItem item)
        {
            if (item.Variable != variable)
                throw new Exception(string.Format("Try to add line item for variable {0} into line item for variable {1}", item.Variable, variable));
            items[iteration] = item;
        }

        public List<KeyValuePair<int, ExecuteTreeLineItem>> Items
        {
            get { return items.ToList(); }
        }

        public override string DebuggerDisplay
        {
            get { return "[" + string.Join(", ", items.Select(e => e.Value.DebuggerDisplay)) + "]"; }
        }
    }

    public class AssignOperationExecuteTreeLineItem : ExecuteTreeLineItem
    {
        private readonly AssignOperation assignOperation;

        public AssignOperationExecuteTreeLineItem(AssignOperation assignOperation)
        {
            this.assignOperation = assignOperation;
        }

        public override string Variable
        {
            get { return AssignOperation.VariableName; }
        }

        public AssignOperation AssignOperation
        {
            get { return assignOperation; }
        }

        public override string DebuggerDisplay
        {
            get { return string.Format("{0} = {1}", AssignOperation.VariableName, AssignOperation.VariableValue); }
        }
    }

    public class ExecuteTreeLine
    {
        private readonly Dictionary<string, ExecuteTreeLineItem> executions = new Dictionary<string, ExecuteTreeLineItem>();

        public void Add(ExecuteTreeLineItem item)
        {
            if (executions.ContainsKey(item.Variable))
                throw new Exception(string.Format("Already added one item for variable {0}", item.Variable));
            executions.Add(item.Variable, item);
        }

        public Dictionary<string, ExecuteTreeLineItem> Executions
        {
            get { return executions; }
        }

        public IEnumerable<string> Variables
        {
            get { return executions.Keys; }
        }

        public ExecuteTreeLineItem GetLineItem(string variableName)
        {
            return executions[variableName];
        }
    }

    public class ExecuteTree
    {
        private readonly string methodName;
        private readonly SparseArray<ExecuteTreeLine> lines = new SparseArray<ExecuteTreeLine>(line => new ExecuteTreeLine());

        public ExecuteTree(string methodName)
        {
            this.methodName = methodName;
        }

        public string MethodName
        {
            get { return methodName; }
        }

        public SparseArray<ExecuteTreeLine> Lines
        {
            get { return lines; }
        }

        public void Add(AssignOperation assignOperation)
        {
            var line = assignOperation.LineNumber;
            lines[line].Add(new AssignOperationExecuteTreeLineItem(assignOperation));
        }

        public void Add(IList<ExecuteTree> executeTrees)
        {
            var list = new SparseArray<ListExecuteTreeLineItem>();
            for (var i = 0; i < executeTrees.Count; i++)
            {
                var executeTree = executeTrees[i];
                foreach (var line in executeTree.Lines)
                {
                    foreach (var variable in line.Value.Variables)
                    {
                        var copyVariable = variable;
                        var listItem = list.GetOrAdd(line.Key, _ => new ListExecuteTreeLineItem(copyVariable));
                        listItem.Add(i, line.Value.GetLineItem(copyVariable));
                    }
                }
            }
            foreach (var listItem in list)
            {
                lines[listItem.Key].Add(listItem.Value);
            }
        }

        public static ExecuteTree Generate(string methodName, IEnumerable<IEnumerable<Operation>> operations)
        {
            var executeTrees = operations.Select(e => Generate(methodName, e)).ToList();
            if (executeTrees.Count <= 1) return executeTrees.FirstOrDefault();

            var executeTree = new ExecuteTree(methodName);
            executeTree.Add(executeTrees);
            return executeTree;
        }

        public static ExecuteTree Generate(string methodName, IEnumerable<Operation> operations)
        {
            var result = new ExecuteTree(methodName);
            foreach (var operation in operations)
            {
                if (operation is AssignOperation)
                {
                    result.Add((AssignOperation)operation);
                }
                else if (operation is LoopOperation)
                {
                    var loopOperation = (LoopOperation)operation;
                    result.Add(loopOperation.Operations.Values.Select(e => Generate(methodName, e)).ToList());
                }
            }
            return result;
        }
    }
}
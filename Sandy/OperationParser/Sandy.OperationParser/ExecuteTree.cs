using System;
using System.Collections.Generic;
using System.Linq;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
    public abstract class ExecuteTreeLineItem
    {
        public abstract string Variable { get; }
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
    }

    public class ExecuteTreeLine
    {
        private readonly Dictionary<string, List<ExecuteTreeLineItem>> executions = new Dictionary<string, List<ExecuteTreeLineItem>>();

        public void Add(ExecuteTreeLineItem item)
        {
            var list = GetList(item.Variable);
            list.Add(item);
        }

        public Dictionary<string, List<ExecuteTreeLineItem>> Executions
        {
            get { return executions; }
        }

        public IEnumerable<string> Variables
        {
            get { return executions.Keys; }
        }

        public List<ExecuteTreeLineItem> GetList(string variableName)
        {
            List<ExecuteTreeLineItem> list;
            if (executions.TryGetValue(variableName, out list)) return list;
            list = new List<ExecuteTreeLineItem>();
            executions[variableName] = list;
            return list;
        }
    }

    public class ExecuteTree
    {
        private readonly SparseArray<ExecuteTreeLine> lines = new SparseArray<ExecuteTreeLine>(line => new ExecuteTreeLine());

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
                        foreach (var execution in line.Value.GetList(copyVariable))
                        {
                            listItem.Add(i, execution);
                        }
                    }
                }
            }
            foreach (var listItem in list)
            {
                lines[listItem.Key].Add(listItem.Value);
            }
        }

        public static ExecuteTree Generate(IEnumerable<Operation> operations)
        {
            var result = new ExecuteTree();
            foreach (var operation in operations)
            {
                if (operation is AssignOperation)
                {
                    result.Add((AssignOperation)operation);
                }
                else if (operation is LoopOperation)
                {
                    var loopOperation = (LoopOperation)operation;
                    result.Add(loopOperation.Operations.Values.Select(Generate).ToList());
                }
            }
            return result;
        }
    }
}
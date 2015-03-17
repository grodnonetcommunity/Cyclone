using System;
using System.Collections.Generic;
using System.Linq;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
    public abstract class ExecuteTreeLineItem
    {
    }

    public class ListExecuteTreeLineItem : ExecuteTreeLineItem
    {
        private readonly SparseArray<ExecuteTreeLineItem> items = new SparseArray<ExecuteTreeLineItem>(line => new ListExecuteTreeLineItem());

        public void Add(int iteration, ExecuteTreeLineItem item)
        {
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

        public AssignOperation AssignOperation
        {
            get { return assignOperation; }
        }
    }

    public class ExecuteTreeLine
    {
        private readonly List<ExecuteTreeLineItem> executions = new List<ExecuteTreeLineItem>();

        public void Add(ExecuteTreeLineItem item)
        {
            executions.Add(item);
        }

        public List<ExecuteTreeLineItem> Executions
        {
            get { return executions; }
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
            var list = new SparseArray<ListExecuteTreeLineItem>(line => new ListExecuteTreeLineItem());
            for (int i = 0; i < executeTrees.Count; i++)
            {
                var executeTree = executeTrees[i];
                foreach (var line in executeTree.lines)
                {
                    var listItem = list[line.Key];
                    foreach (var execution in line.Value.Executions)
                    {
                        listItem.Add(i, execution);
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
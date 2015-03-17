using System;
using System.Collections.Generic;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
    public abstract class ExecuteTreeLineItem
    {
    }

    public class ListExecuteTreeLineItem : ExecuteTreeLineItem
    {
        private readonly List<ExecuteTreeLineItem> items = new List<ExecuteTreeLineItem>();

        public void Add(ExecuteTreeLineItem item)
        {
            items.Add(item);
        }

        public List<ExecuteTreeLineItem> Items
        {
            get { return items; }
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

        public void Add(IEnumerable<ExecuteTree> executeTrees)
        {
            var list = new SparseArray<ListExecuteTreeLineItem>(line => new ListExecuteTreeLineItem());
            foreach (var executeTree in executeTrees)
            {
                foreach (var line in executeTree.lines)
                {
                    var listItem = list[line.Key];
                    foreach (var execution in line.Value.Executions)
                    {
                        listItem.Add(execution);
                    }
                }
            }
            foreach (var listItem in list)
            {
                lines[listItem.Key].Add(listItem.Value);
            }
        }
    }
}
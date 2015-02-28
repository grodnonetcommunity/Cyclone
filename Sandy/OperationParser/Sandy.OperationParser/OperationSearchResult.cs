using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OperationSearchResult
	{
		public OperationSearchResult(IList<Operation> operations)
		{
			//Just to be sure
			AssignOperations.Clear();
			LoopOperations.Clear();
			//For output, there are 2 types of operations: 
			//Independant assigns and loop assigns
			foreach (var operation in operations)
			{
				var assignOperation = operation as AssignOperation;
				if (assignOperation != null)
				{
					if (operation.ParentOperation is LoopOperation)
					{
						if (!LoopOperations.ContainsKey(operation.ParentOperation))
						{
							LoopOperations.Add(operation.ParentOperation,
								new List<AssignOperation> {assignOperation});
						}
						else
						{
							LoopOperations[operation.ParentOperation].Add(assignOperation);
						};
					}
					else
					{
						AssignOperations.Add(assignOperation);
					}
				}
			}
		}

		public IList<AssignOperation> AssignOperations { get; set; } = new List<AssignOperation>();
		public Dictionary<Operation, IList<AssignOperation>> LoopOperations { get; set; } = new Dictionary<Operation, IList<AssignOperation>>();
	}
}

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
		    AssignOperations = new List<AssignOperation>();
		    LoopOperations = new Dictionary<LoopOperation, IList<AssignOperation>>();
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
					var loopParent = operation.ParentOperation as LoopOperation;
					if (loopParent != null)
					{
						if (!LoopOperations.ContainsKey(loopParent))
						{
							LoopOperations.Add(loopParent,
								new List<AssignOperation> {assignOperation});
						}
						else
						{
							LoopOperations[loopParent].Add(assignOperation);
						}
					}
					else
					{
						AssignOperations.Add(assignOperation);
					}
				}
			}
		}

		public IList<AssignOperation> AssignOperations { get; set; }

		public Dictionary<LoopOperation, IList<AssignOperation>> LoopOperations { get; set; }
	}
}

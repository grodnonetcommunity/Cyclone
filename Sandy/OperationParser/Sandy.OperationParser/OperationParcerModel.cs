using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OperationParcerModel : Operation
	{
		public Operation Operation { get; set; }
		public Operation ParentOperation { get; set; }
		public int IterationNumber { get; set; }
	}
}

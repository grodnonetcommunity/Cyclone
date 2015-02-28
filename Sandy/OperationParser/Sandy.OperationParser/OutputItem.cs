using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OutputItem
	{
		public string Output;
		public double MarginLeft;

		public OutputItem(string output, double marginLeft)
		{
			Output = output;
			MarginLeft = marginLeft;
		}

		public OutputItem(string output)
		{
			Output = output;
		}
	}
}

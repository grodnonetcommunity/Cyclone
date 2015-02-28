using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OutputItem
	{
		public string Output { get; set; }
		public double MarginLeft { get; set; }
		public Style Style { get; set; }

		public OutputItem(string output, double marginLeft, Style style = null)
		{
			Output = output;
			MarginLeft = marginLeft;
			Style = style;
		}

		public OutputItem(string output, Style style = null)
		{
			Style = style;
			Output = output;
		}
	}
}

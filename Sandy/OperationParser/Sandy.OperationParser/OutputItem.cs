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
		public Style Style { get; set; }
		public MeasureGroup MeasureGroup { get; set; }

		public OutputItem(string output, MeasureGroup measureGroup, Style style = null)
		{
			Style = style;
			Output = output;
			MeasureGroup = measureGroup;
		}
	}
}

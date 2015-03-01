using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OutputItem
	{
		public string Output { get; set; }
		public Color OutputColor { get; set; }
		public MeasureGroup MeasureGroup { get; set; }

		public OutputItem(string output, MeasureGroup measureGroup) : this(output, measureGroup, Colors.Black)
		{
			
		}

		public OutputItem(string output, MeasureGroup measureGroup, Color outputColor)
		{
			OutputColor = outputColor;
			Output = output;
			MeasureGroup = measureGroup;
		}
	}
}

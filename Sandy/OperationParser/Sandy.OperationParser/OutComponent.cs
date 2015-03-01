using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OutComponent
	{
		private readonly Dictionary<int, StackPanel> _elements;

		public OutComponent(Dictionary<int, StackPanel> elements)
		{
			_elements = elements;
		}

		public UIElement this[int index]
		{
			get
			{
				if (_elements.ContainsKey(index))
				{
					return _elements[index];
				}
				return null;
			}
		}

		public int Count => _elements.Count;
	}
}

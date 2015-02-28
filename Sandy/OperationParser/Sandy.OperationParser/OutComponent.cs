using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OutComponent
	{
		private readonly Dictionary<int, UIElement> Elements;

		public OutComponent(Dictionary<int, UIElement> elements)
		{
			Elements = elements;
		}

		public UIElement this[int index]
		{
			get
			{
				if (Elements.ContainsKey(index))
				{
					return Elements[index];
				}
				return null;
			}
		}

		public int Count => Elements.Count;
	}
}

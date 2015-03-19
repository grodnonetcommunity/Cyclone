using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AV.Cyclone.Sandy.Models
{
	public class OutComponent
	{
		private readonly Dictionary<int, UIElement> _elements;

		public OutComponent(Dictionary<int, UIElement> elements)
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

	    public int Count
	    {
	        get { return _elements.Count; }
	    }
	}
}

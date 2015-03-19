using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AV.Cyclone.Sandy.Models
{
    public class OutComponent : IOutComponent
    {
        private readonly int minLineNumber;
        private readonly int maxLineNumber;
        private readonly Dictionary<int, UIElement> elements;

        public OutComponent(Dictionary<int, UIElement> elements)
        {
            this.elements = elements;
            this.minLineNumber = this.elements.Keys.Min();
            this.maxLineNumber = this.elements.Keys.Max();
        }

        public int MinLineNumber
        {
            get { return minLineNumber; }
        }

        public int MaxLineNumber
        {
            get { return maxLineNumber; }
        }

        public UIElement this[int index]
        {
            get
            {
                if (index < minLineNumber || index > maxLineNumber)
                    return null;
                if (elements.ContainsKey(index))
                {
                    return elements[index];
                }
                return null;
            }
        }

        public int Count
        {
            get { return elements.Count; }
        }
    }
}

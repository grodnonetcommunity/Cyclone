using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using JetBrains.Annotations;

namespace AV.Cyclone.Sandy.Models
{
    public class OutComponent : IOutComponent, INotifyPropertyChanged
    {
        private int minLineNumber;
        private int maxLineNumber;
        private static readonly SandyColorProvider sandyColorProvider = new SandyColorProvider();
        private SandyColorProvider colorProvider = sandyColorProvider;
        private readonly Dictionary<int, UIElement> elements;

        public event PropertyChangedEventHandler PropertyChanged;

        public OutComponent(Dictionary<int, UIElement> elements)
        {
            this.elements = elements;
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

        public SandyColorProvider ColorProvider
        {
            get { return colorProvider; }
            set
            {
                if (Equals(value, colorProvider)) return;
                colorProvider = value;
                OnPropertyChanged();
            }
        }

        public void SetColorProvider(SandyColorProvider newColorProvider)
        {
            this.ColorProvider = newColorProvider;
        }

        public void Freeze()
        {
            this.minLineNumber = this.elements.Keys.Min();
            this.maxLineNumber = this.elements.Keys.Max();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

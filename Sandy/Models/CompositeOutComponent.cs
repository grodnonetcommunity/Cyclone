using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AV.Cyclone.Sandy.Models
{
    public class CompositeOutComponent : IOutComponent
    {
        private readonly List<IOutComponent> outComponents = new List<IOutComponent>();

        public int MinLineNumber { get; private set; }

        public int MaxLineNumber { get; private set; }

        public int Count { get; private set; }

        public UIElement this[int index]
        {
            get
            {
                return outComponents.Select(outComponent => outComponent[index]).FirstOrDefault(control => control != null);
            }
        }

        public void AddComponent(IOutComponent outComponent)
        {
            outComponents.Add(outComponent);
            MinLineNumber = outComponents.Min(e => e.MinLineNumber);
            MaxLineNumber = outComponents.Min(e => e.MaxLineNumber);
            Count = outComponents.Sum(e => e.Count);
        }
    }
}
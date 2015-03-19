using System.Windows;

namespace AV.Cyclone.Sandy.Models
{
    public interface IOutComponent
    {
        int MinLineNumber { get; }

        int MaxLineNumber { get; }

        int Count { get; }

        UIElement this[int index] { get; }
    }
}
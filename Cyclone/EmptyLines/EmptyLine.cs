using System.Windows.Controls;
using System.Windows.Media;

namespace AV.Cyclone.EmtyLines
{
    public class EmptyLine : Canvas
    {
        public EmptyLine()
        {
            Background = new SolidColorBrush(Colors.Transparent);
        }
    }
}
using System.Windows.Media;

namespace AV.Cyclone
{
    public static class ColorHelper
    {
        public static Color ToWpf(this System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
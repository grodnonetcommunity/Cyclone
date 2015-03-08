using System.Windows;

namespace AV.Cyclone.Sandy.Models
{
    public interface ICloudCollection
    {
        FrameworkElement GetCloud(int line);

        double GetHeight(int line);

        void SetColorProvider(SandyColorProvider colorProvider);
    }
}
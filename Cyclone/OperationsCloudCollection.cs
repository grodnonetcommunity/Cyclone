using System.Windows;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;

namespace AV.Cyclone
{
    public class OperationsCloudCollection : ICloudCollection
    {
        private readonly OutComponent outComponent;

        public OperationsCloudCollection(OutComponent outComponent)
        {
            this.outComponent = outComponent;
        }

        public FrameworkElement GetCloud(int line)
        {
            var frameworkElement = (FrameworkElement) outComponent[line];
            if (frameworkElement != null)
            {
                frameworkElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                frameworkElement.Arrange(new Rect(frameworkElement.DesiredSize));
            }
            return frameworkElement;
        }

        public double GetHeight(int line)
        {
            var control = GetCloud(line);
            if (control == null) return 0.0;
            return control.ActualHeight;
        }

        public void SetColorProvider(SandyColorProvider colorProvider)
        {
        }
    }
}
using System.Windows;
using AV.Cyclone.Sandy.Models;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Service
{
    public class CycloneCloudCollection : ICloudCollection
    {
        private readonly IWpfTextView textView;
        private readonly ICloudCollection cloudCollection;

        public CycloneCloudCollection(ICloudCollection cloudCollection, IWpfTextView textView)
        {
            this.cloudCollection = cloudCollection;
            this.textView = textView;
        }

        public FrameworkElement GetCloud(int line)
        {
            return cloudCollection.GetCloud(line);
        }

        public double GetHeight(int line)
        {
            return cloudCollection.GetHeight(line) / (textView.ZoomLevel / 100);
        }

        public void SetColorProvider(SandyColorProvider colorProvider)
        {
            cloudCollection.SetColorProvider(colorProvider);
        }
    }
}
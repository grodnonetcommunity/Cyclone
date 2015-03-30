using System;
using System.Windows.Threading;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone.EmptyLines
{
    public class EmptyLineTransformSource : ILineTransformSource
    {
        private readonly ICycloneService cycloneService;
        private readonly IWpfTextView textView;
        private ICloudCollection cloudCollection;

        public EmptyLineTransformSource(ICycloneService cycloneService, IWpfTextView textView)
        {
            this.cycloneService = cycloneService;
            this.textView = textView;
            this.cycloneService.Changed += CycloneServiceOnChanged;
        }

        private void CycloneServiceOnChanged(object sender, EventArgs eventArgs)
        {
            UpdateClouds();
        }

        private void UpdateClouds()
        {
            CloudCollection = cycloneService.GetClouds(textView);
        }
        public ICloudCollection CloudCollection
        {
            get { return cloudCollection; }
            set
            {
                cloudCollection = value;
                // Hack for reformat code
                var oldTabSize = textView.Options.GetOptionValue(DefaultOptions.TabSizeOptionId);
                textView.Options.SetOptionValue(DefaultOptions.TabSizeOptionId, oldTabSize + 1);
                textView.Options.SetOptionValue(DefaultOptions.TabSizeOptionId, oldTabSize);
            }
        }

        public LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement)
        {
            if (cloudCollection == null)
                return line.LineTransform;

            var lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position);
            var height = cloudCollection.GetHeight(lineNumber);

            if (line.Height < height)
            {
                var delta = (height - line.Height);
                var topDelta = delta / 2;
                var bottomDelta = delta - topDelta;
                return new LineTransform(line.LineTransform.TopSpace + topDelta,
                    line.LineTransform.BottomSpace + bottomDelta, line.LineTransform.VerticalScale);
            }

            return line.LineTransform;
        }
    }
}
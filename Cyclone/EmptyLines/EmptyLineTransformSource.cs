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
        private readonly Dispatcher dispatcher;
        private ICloudCollection cloudCollection;

        public EmptyLineTransformSource(ICycloneService cycloneService, IWpfTextView textView)
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.cycloneService = cycloneService;
            this.textView = textView;
            this.cycloneService.CycloneChanged += CycloneServiceOnCycloneChanged;
        }

        private void CycloneServiceOnCycloneChanged(object sender, CycloneEventArgs cycloneEventArgs)
        {
            if (ExamplesPackage.WeatherStation == null) return;
            ExamplesPackage.WeatherStation.Executed += WeatherStationOnExecuted;
        }

        private void WeatherStationOnExecuted(object sender, EventArgs eventArgs)
        {
            dispatcher.BeginInvoke((Action)GetCloudCollection);
        }

        private void GetCloudCollection()
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

            if (line.TextHeight < height)
            {
                return new LineTransform(line.LineTransform.TopSpace,
                    line.LineTransform.BottomSpace + (height - line.Height), line.LineTransform.VerticalScale);
            }

            return line.LineTransform;
        }
    }
}
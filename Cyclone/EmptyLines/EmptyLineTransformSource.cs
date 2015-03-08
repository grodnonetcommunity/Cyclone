using System;
using System.Windows.Threading;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone.EmptyLines
{
    public class EmptyLineTransformSource : ILineTransformSource
    {
        private readonly ICycloneService cycloneService;
        private readonly IWpfTextView textView;
        private readonly ITextDocumentFactoryService documentFactoryService;
        private readonly Dispatcher dispatcher;
        private ICloudCollection cloudCollection;

        public EmptyLineTransformSource(ICycloneService cycloneService, IWpfTextView textView, ITextDocumentFactoryService documentFactoryService)
        {
            this.dispatcher = Dispatcher.CurrentDispatcher;
            this.cycloneService = cycloneService;
            this.textView = textView;
            this.documentFactoryService = documentFactoryService;
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
            ITextDocument document;

            if (!documentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
                return;
            var operations = ExamplesPackage.WeatherStation.GetOperations(document.FilePath);
            if (operations == null)
                return;
            var uiGenerator = new UIGenerator(operations);
            var outComponent = uiGenerator.GetOutputComponents(document.FilePath);
            CloudCollection = new OperationsCloudCollection(outComponent);
        }
        public ICloudCollection CloudCollection
        {
            get { return cloudCollection; }
            set
            {
                cloudCollection = value;
                foreach (var line in textView.TextViewLines)
                {
                    textView.DisplayTextLineContainingBufferPosition(line.Start, line.Top, ViewRelativePosition.Top);
                }
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
                return new LineTransform(line.LineTransform.TopSpace,
                    line.LineTransform.BottomSpace + (height - line.Height), line.LineTransform.VerticalScale);
            }

            return line.LineTransform;
        }
    }
}
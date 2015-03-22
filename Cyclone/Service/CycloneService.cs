using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Threading;
using AV.Cyclone.Katrina.Executor;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Service
{
    [Export(typeof(ICycloneService))]
    public sealed class CycloneService : ICycloneService
    {
        private readonly ITextDocumentFactoryService textDocumentFactoryService;
        private readonly Dispatcher dispatcher;
        private Dictionary<string, ICloudCollection> clouds = new Dictionary<string, ICloudCollection>();
        private WeatherStation weatherStation;

        [ImportingConstructor]
        private CycloneService(ITextDocumentFactoryService textDocumentFactoryService)
        {
            this.textDocumentFactoryService = textDocumentFactoryService;
            this.dispatcher = Dispatcher.CurrentDispatcher;
        }

        public event EventHandler Changed;

        public void StartCyclone(string solutionPath, string projectName, string filePath, int lineNumber)
        {
            DisposeWeatherStation();
            weatherStation = new WeatherStation(solutionPath, projectName, filePath, lineNumber);
            weatherStation.Executed += WeatherStationOnExecuted;
            weatherStation.Start();
        }

        public void StopCyclone()
        {
            DisposeWeatherStation();
            OnChanged();
        }

        public void UpdateFile(ITextView textView, string content)
        {
            if (weatherStation == null) return;

            ITextDocument document;

            if (!textDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
                return;

            weatherStation.FileUpdated(document.FilePath, content);
        }

        public ICloudCollection GetClouds(IWpfTextView textView)
        {
            if (weatherStation == null)
                return null;

            ITextDocument document;

            if (!textDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
                return null;

            ICloudCollection cloudCollection;
            if (clouds.TryGetValue(document.FilePath, out cloudCollection))
                return cloudCollection;

            var methodCalls = weatherStation.GetMethodCalls(document.FilePath);
            if (methodCalls == null)
                return null;
            var generator = new UiGenerator2();
            foreach (var methodCall in methodCalls)
            {
                var executeTree = ExecuteTree.Generate(methodCall.Key, methodCall.Value);
                generator.Generate(executeTree);
            }
            var outComponent = generator.GetOutputComponents();

            cloudCollection = new OperationsCloudCollection(outComponent);
            //cloudCollection.SetColorProvider(colorProviderService.GetColorProvider(textView));
            clouds.Add(document.FilePath, cloudCollection);
            return cloudCollection;
        }

        private void WeatherStationOnExecuted(object sender, EventArgs eventArgs)
        {
            clouds = new Dictionary<string, ICloudCollection>();
            dispatcher.BeginInvoke((Action)OnChanged);
        }

        private void DisposeWeatherStation()
        {
            if (weatherStation != null)
            {
                weatherStation.Executed -= WeatherStationOnExecuted;
                weatherStation.Dispose();
                weatherStation = null;
            }
        }

        private void OnChanged()
        {
            var handler = Changed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
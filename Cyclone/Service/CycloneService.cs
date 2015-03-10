using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Service
{
    [Export(typeof(ICycloneService))]
    public sealed class CycloneService : ICycloneService
    {
        private readonly Dictionary<string, ICloudCollection> clouds = new Dictionary<string, ICloudCollection>();
        private readonly ITextDocumentFactoryService textDocumentFactoryService;

        [ImportingConstructor]
        private CycloneService(ITextDocumentFactoryService textDocumentFactoryService)
        {
            this.textDocumentFactoryService = textDocumentFactoryService;
        }

        public event EventHandler<CycloneEventArgs> CycloneChanged;
        
        public void StartCyclone()
        {
            OnCycloneChanged(new CycloneEventArgs());
        }

        public ICloudCollection GetClouds(IWpfTextView textView)
        {
            ITextDocument document;

            if (!textDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
                return null;

            ICloudCollection cloudCollection;
            if (clouds.TryGetValue(document.FilePath, out cloudCollection))
                return cloudCollection;

            var operations = ExamplesPackage.WeatherStation.GetOperations(document.FilePath);
            if (operations == null)
                return null;
            var uiGenerator = new UIGenerator(operations);
            var outComponent = uiGenerator.GetOutputComponents(document.FilePath);

            cloudCollection = new OperationsCloudCollection(outComponent);
            //cloudCollection.SetColorProvider(colorProviderService.GetColorProvider(textView));
            clouds.Add(document.FilePath, cloudCollection);
            return cloudCollection;
        }

        private void OnCycloneChanged(CycloneEventArgs e)
        {
            var cycloneChanged = CycloneChanged;
            if (cycloneChanged != null)
                cycloneChanged.Invoke(this, e);
        }
    }
}
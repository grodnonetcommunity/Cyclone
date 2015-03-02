using System;
using System.Windows;
using AV.Cyclone.OutputPane;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Margin
{
    public class Margin : MarginBase
    {
        private ICycloneService _cycloneService;
        private readonly string _filePath;

        public Margin(IWpfTextView sourceView)
        {
            SourceTextView = sourceView;
            this.Width = 0;
        }

        public Margin(IWpfTextView sourceView, ICycloneService cycloneService, string filePath) : this(sourceView)
        {
            _cycloneService = cycloneService;
            _filePath = filePath;
            cycloneService.CycloneChanged += CycloneServiceOnCycloneChanged;
        }

        private void CycloneServiceOnCycloneChanged(object sender, CycloneEventArgs cycloneEventArgs)
        {
            if (cycloneEventArgs.EventType == CycloneEventsType.Start)
            {
                this.Width = 400;
            }
          
        }

        protected IWpfTextView SourceTextView { get; set; }
        public OutputPaneView OutputPaneView { get; set; }

        protected override FrameworkElement CreatePreviewControl()
        {
            var model = new OutputPaneModel(SourceTextView, _filePath);
           
            return new OutputPaneView(model); 
        }
    }
}
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

        public Margin(IWpfTextView sourceView)
        {
            SourceTextView = sourceView;
            this.Width = 0;
        }

        public Margin(IWpfTextView sourceView, ICycloneService cycloneService) : this(sourceView)
        {
            _cycloneService = cycloneService;
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
            var model = new OutputPaneModel(SourceTextView);
            OutputPaneView = OutputPaneView ?? new OutputPaneView(model);
            return OutputPaneView;
        }
    }
}
using System;
using System.Linq;
using System.Windows;
using AV.Cyclone.OutputPane;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Margin
{
    public class Margin : MarginBase
    {
        private CycloneService _cycloneService;

        public Margin(IWpfTextView sourceView)
        {
            SourceTextView = sourceView;
        }

        public Margin(IWpfTextView sourceView, CycloneService cycloneService) : this(sourceView)
        {
            this._cycloneService = cycloneService;
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
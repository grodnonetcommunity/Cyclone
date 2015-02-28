using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Margin
{
    public class Margin : MarginBase
    {
        public Margin(IWpfTextView sourceView) : base()
        {
            SourceTextView = sourceView;
        }

        protected IWpfTextView SourceTextView { get; set; }
        public FrameworkElement OutputPaneView { get; set; }

        protected override FrameworkElement CreatePreviewControl()
        {
            OutputPaneView = OutputPaneView ?? new TextBox();
            return OutputPaneView;
        }
    }
}
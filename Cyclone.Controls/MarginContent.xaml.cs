using System;
using System.Windows.Controls;
using AV.Cyclone.Sandy.Models;

namespace Cyclon.Controls
{
    public partial class MarginContent
    {
        private readonly ITextViewService textViewService;
        private readonly ICloudCollection cycloneService;

        public MarginContent(ITextViewService textViewService, ICloudCollection cycloneService)
        {
            this.textViewService = textViewService;
            this.cycloneService = cycloneService;
            InitializeComponent();
            this.DataContext = this;
            this.textViewService.LayoutChanged += TextViewOnLayoutChanged;
            UpdateScroll();
        }

        private void UpdateScroll()
        {
            var scale = textViewService.Scale;
            contentCanvas.Children.Clear();
            foreach (var line in textViewService.GetVisibleLines())
            {
                var lineControl = cycloneService.GetCloud(line.LineNumber);
                if (lineControl == null)
                    continue;

                if (!ReferenceEquals(lineControl.Parent, this) && lineControl.Parent != null)
                    lineControl.Parent.RemoveChild(lineControl);

                if (lineControl is Control)
                {
                    ((Control)lineControl).FontSize = 12 * scale;
                }

                contentCanvas.Children.Add(lineControl);
                Canvas.SetLeft(lineControl, 0);
                Canvas.SetTop(lineControl, line.TextTop * scale);
            }
        }

        private void TextViewOnLayoutChanged(object sender, EventArgs eventArgs)
        {
            UpdateScroll();
        }
    }
}

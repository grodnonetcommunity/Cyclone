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
        private OutputPaneView _outputPaneView;
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

        public OutputPaneView OutputPaneView
        {
            get { return _outputPaneView; }
            set
            {
                if (_outputPaneView != null)
                {
                    Unsubscribe();
                }
                _outputPaneView = value;
                Subscribe();
            }
        }

        protected override FrameworkElement CreatePreviewControl()
        {
            var numberOfLines = SourceTextView.TextSnapshot.Lines.Count();
            var lineHeight = SourceTextView.LineHeight;
            var model = new OutputPaneModel(SourceTextView);
            OutputPaneView = OutputPaneView ?? new OutputPaneView(model);
            return OutputPaneView;
        }

        private void LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var args = e;
            if (!args.VerticalTranslation)
                return;

            var sourceFirstLine = SourceTextView.TextViewLines.FirstVisibleLine;
            ITextSnapshotLine sourceSnapshotLine = SourceTextView.TextSnapshot.Lines
                .FirstOrDefault(x => x.Start.Position == sourceFirstLine.Start);

            if (sourceSnapshotLine == null)
                return;

            var sourceLineNumber = sourceSnapshotLine.LineNumber;

            OutputPaneView.ViewModel
                .ScrollTo(sourceLineNumber, sourceFirstLine);
        }

        private void Subscribe()
        {
            SourceTextView.LayoutChanged += LayoutChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Unsubscribe();
            }
        }

        private void Unsubscribe()
        {
            SourceTextView.LayoutChanged -= LayoutChanged;
        }
    }
}
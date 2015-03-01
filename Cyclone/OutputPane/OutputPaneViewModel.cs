using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AV.Cyclone.Annotations;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone.OutputPane
{
    public class OutputPaneViewModel : INotifyPropertyChanged, IDisposable
    {
        public OutputPaneModel Model { get; set; }
        private readonly OutputPaneView _view;

        private bool[] IsInitMarginSet;

        public OutputPaneViewModel(OutputPaneView view, OutputPaneModel model)
        {
            _view = view;
            Model = model;

            _view.ItemsControl.ItemsSource = Model.ViewObjectModel;

            IsInitMarginSet = new bool[Model.ViewObjectModel.Elements.Count];

            //UpdateScrollInternal();

            Subscribe();
        }

        public double ZoomLevel = 1;

        public IWpfTextView SourceTextView
        {
            get
            {
                if (Model == null)
                {
                    return null;
                }
                return Model.SourceTextView;
            }
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Subscribe()
        {
            SourceTextView.LayoutChanged += UpdateScroll;
            SourceTextView.ZoomLevelChanged += UpdateZoom;
            SourceTextView.TextBuffer.Changed += Reinitialize;
        }

        private void Unsubscribe()
        {
            SourceTextView.LayoutChanged -= UpdateScroll;
            SourceTextView.ZoomLevelChanged -= UpdateZoom;
            SourceTextView.TextBuffer.Changed -= Reinitialize;
        }

        private void Reinitialize(object sender, TextContentChangedEventArgs e)
        {
            IsInitMarginSet = new bool[Model.ViewObjectModel.Elements.Count];
            Model.Reinit();
        }

        private void UpdateZoom(object sender, ZoomLevelChangedEventArgs e)
        {
            Model.ZoomLevel = SourceTextView.ZoomLevel;
        }

        private void UpdateScroll(object sender, TextViewLayoutChangedEventArgs e)
        {
            var args = e;
            if (!args.VerticalTranslation)
                return;

            UpdateScrollInternal();
        }

        private void UpdateScrollInternal()
        {
            var sourceFirstLine = SourceTextView.TextViewLines.FirstVisibleLine;
            var sourceSnapshotLine = SourceTextView.TextSnapshot.Lines
                .FirstOrDefault(x => x.Start.Position == sourceFirstLine.Start);

            if (sourceSnapshotLine == null)
                return;

            var sourceLineNumber = sourceSnapshotLine.LineNumber;

            ScrollTo(sourceLineNumber, sourceFirstLine);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ScrollTo(int sourceLineNumber, IWpfTextViewLine sourceFirstLine)
        {
            SetCodeLensAdorments(sourceLineNumber);
            var shift = sourceFirstLine.Top - sourceFirstLine.VisibleArea.Y;
            if (shift < 0)
            {
                shift = shift*-1;
            }
            var elemToScroll = Model.ViewObjectModel[sourceLineNumber];
            var translatePoint = elemToScroll.TranslatePoint(new Point(), _view.OutputPaneScrollViewer);
            ScrollTo(_view.OutputPaneScrollViewer.VerticalOffset + translatePoint.Y + shift);
        }

        public void ScrollTo(double offset)
        {
            _view.OutputPaneScrollViewer.ScrollToVerticalOffset(offset);
        }

        private void SetCodeLensAdorments(int sourceLineNumber)
        {
            var nominalLineHeight = Model.SourceTextView.LineHeight;
            var viewLines = Model.SourceTextView.TextViewLines;

            // viewLines is one element more than first visible line
            for (int i = 0; i < viewLines.Count - 1; i++)
            {
                var lineHeight = viewLines[i + 1].Height;
                var topAdormentHeight = lineHeight - nominalLineHeight;
                var index = i + sourceLineNumber;
                if (IsInitMarginSet[index])
                {
                    continue;
                }
                var wrapper = (UniformGrid)Model.ViewObjectModel[index];
                var a = wrapper.Children.OfType<UniformGrid>().FirstOrDefault();
                Model.ViewObjectModel.SetAdorment(index);
                if (a != null)
                {
                    a.Margin = new Thickness(0, topAdormentHeight, 0, 0);
                    IsInitMarginSet[index] = true;
                }
            }
        }
    }
}
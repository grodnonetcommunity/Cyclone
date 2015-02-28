using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AV.Cyclone.Annotations;
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
        }

        private void Unsubscribe()
        {
            SourceTextView.LayoutChanged -= UpdateScroll;
            SourceTextView.ZoomLevelChanged -= UpdateZoom;

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

            var sourceFirstLine = SourceTextView.TextViewLines.FirstVisibleLine;
            var sourceSnapshotLine = SourceTextView.TextSnapshot.Lines
                .FirstOrDefault(x => x.Start.Position == sourceFirstLine.Start);

            if (sourceSnapshotLine == null)
                return;

            var sourceLineNumber = sourceSnapshotLine.LineNumber;

            _view.ViewModel
                .ScrollTo(sourceLineNumber, sourceFirstLine);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ScrollTo(int sourceLineNumber, IWpfTextViewLine sourceFirstLine)
        {
            UpdateVisibleLinesHeight(sourceLineNumber, sourceFirstLine);
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

        private void UpdateVisibleLinesHeight(int sourceLineNumber, IWpfTextViewLine sourceFirstLine)
        {
            var nominalLineHeight = Model.SourceTextView.LineHeight;
            var viewLines = Model.SourceTextView.TextViewLines;

            for (int i = 0; i < viewLines.Count; i++)
            {
                var lineHeight = viewLines[i].Height;
                var topAdormentHeight = lineHeight - nominalLineHeight;
                var index = i + sourceLineNumber;
                Model.ViewObjectModel.SetAdorment(index);
                if (index < 0 || IsInitMarginSet[index])
                {
                    continue;
                }
                var wrapper = (Border)Model.ViewObjectModel.Elements[index];
                IsInitMarginSet[index] = true;
                wrapper.Padding = new Thickness(0, topAdormentHeight, 0, 0);
                wrapper.Height += topAdormentHeight;
            }
        }
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AV.Cyclone.Annotations;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone.OutputPane
{
    public class OutputPaneViewModel : INotifyPropertyChanged
    {
        private readonly OutputPaneView _view;
        private readonly OutputPaneModel _model;

        public OutputPaneViewModel(OutputPaneView view, OutputPaneModel model)
        {
            _view = view;
            _model = model;
            _view.ItemsControl.ItemsSource = _model.ViewObjectModel;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ScrollTo(int sourceLineNumber, IWpfTextViewLine sourceFirstLine)
        {
            var shift = sourceFirstLine.Top - sourceFirstLine.VisibleArea.Y;
            if (shift < 0)
            {
                shift = shift * -1;
            }
            var lineHeight = _model.LineHeight;
            ScrollTo(lineHeight * sourceLineNumber + shift);
            UpdateVisibleLinesHeight(sourceLineNumber);
        }

        public void ScrollTo(double offset)
        {
            _view.OutputPaneScrollViewer.ScrollToVerticalOffset(offset);
        }

        private void UpdateVisibleLinesHeight(int lineNumber)
        {
            var viewObjects = _model.ViewObjectModel;
            for (int i = 0; i < lineNumber; i++)
            {
                viewObjects[i].Height = _model.LineHeight;
            }
            var sourceLines = _model.SourceTextView.TextViewLines;
            for (int i = lineNumber; i < lineNumber + sourceLines.Count - 1; i++)
            {
                // don't know why indexes are broken
                if (i == 0)
                {
                    continue;
                }
                viewObjects[i - 1].Height = sourceLines[i - lineNumber].Height;
            }
        }
    }
}
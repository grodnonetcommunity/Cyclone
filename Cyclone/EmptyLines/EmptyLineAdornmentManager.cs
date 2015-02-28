using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;

namespace AV.Cyclone.EmtyLines
{
    public class EmptyLineAdornmentManager : ITagger<ErrorTag>, IDisposable
    {
        private bool _initialised1;
        private bool _initialised2;
        private readonly IWpfTextView _view;

        public EmptyLineAdornmentManager(IWpfTextView view)
        {
            _view = view;
            EmptyLines = new Dictionary<int, EmptyLine>();
            _view.LayoutChanged += ViewOnLayoutChanged;
        }

        public Dictionary<int, EmptyLine> EmptyLines { get; set; }

        public void Dispose()
        {
        }

        public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return new List<ITagSpan<ErrorTag>>();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void ViewOnLayoutChanged(object sender, TextViewLayoutChangedEventArgs textViewLayoutChangedEventArgs)
        {
            TagsChanged?.Invoke(this,
                new SnapshotSpanEventArgs(new SnapshotSpan(_view.TextSnapshot,
                    new Span(0, _view.TextSnapshot.Length))));

            foreach (var line in _view.TextViewLines)
                // TODO [?]: implement more sensible handling of removing error tags, then use e.NewOrReformattedLines
            {
                var lineNumber = line.Snapshot.GetLineFromPosition(line.Start.Position).LineNumber;
                //TODO [?]: Limit rate of calls to the below when user is editing a line
                try
                {
                    CreateVisuals(line, lineNumber);
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            if (!_initialised1)
            {
                _view.ZoomLevel++;
                _initialised1 = true;
            }
            if (!_initialised2)
            {
                _view.ZoomLevel--;
                _initialised2 = true;
            }
        }

        private void CreateVisuals(ITextViewLine line, int lineNumber)
        {
            if (lineNumber == 9)
            {
                if (!EmptyLines.ContainsKey(lineNumber))
                {
                    EmptyLines.Add(lineNumber, new EmptyLine
                    {
                        Height = 16*3
                    });
                }
            }
        }
    }
}
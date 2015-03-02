using System;
using System.Collections.Generic;
using System.Diagnostics;
using AV.Cyclone.Service;
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
        private readonly ICycloneService _cycloneService;

        public EmptyLineAdornmentManager(IWpfTextView view, ICycloneService cycloneService)
        {
            _view = view;
            _cycloneService = cycloneService;
            EmptyLines = new Dictionary<int, EmptyLine>();
            _view.LayoutChanged += ViewOnLayoutChanged;
            _cycloneService.CycloneChanged += CycloneServiceOnCycloneChanged;
        }

        private void CycloneServiceOnCycloneChanged(object sender, CycloneEventArgs cycloneEventArgs)
        {
            if (cycloneEventArgs.EventType == CycloneEventsType.ExpandLines)
            {
                var data = cycloneEventArgs as ExpandLineEventArgs;
                EmptyLine emptyLine;
                if (data != null)
                {
                    var expandLineInfo = data.ExpandLineInfo;
                    if (EmptyLines.TryGetValue(expandLineInfo.LineNumber, out emptyLine))
                    {
                        emptyLine.Height = expandLineInfo.PreferedSize;
                    }
                    else
                    {
                        EmptyLines.Add(expandLineInfo.LineNumber, new EmptyLine()
                        {
                            Height = expandLineInfo.PreferedSize
                        });
                    }
                    Render();
                }
            }
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
            Render();
        }

        private void Render()
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
            
        }
    }
}
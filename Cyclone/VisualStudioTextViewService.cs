using System;
using System.Collections.Generic;
using Cyclon.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone
{
    public class VisualStudioTextViewService : ITextViewService
    {
        private readonly IWpfTextView textView;

        public event EventHandler LayoutChanged;

        public VisualStudioTextViewService(IWpfTextView textView)
        {
            this.textView = textView;
            this.textView.LayoutChanged += (sender, args) => OnLayoutChanged();
            this.textView.ZoomLevelChanged += (sender, args) => OnLayoutChanged();
        }

        public double Scale
        {
            get { return textView.ZoomLevel / 100; }
        }

        public int FirstVisibleLineNumber
        {
            get { return GetLineNumber(textView.TextViewLines.FirstVisibleLine); }
        }

        public int LastVisibleLineNumber
        {
            get { return GetLineNumber(textView.TextViewLines.LastVisibleLine); }
        }

        public IEnumerable<LineInfo> GetVisibleLines()
        {
            var firstVisibleLineNumber = FirstVisibleLineNumber;
            var lastVisibleLineNumber = LastVisibleLineNumber;
            foreach (var viewLine in textView.TextViewLines)
            {
                var lineNumber = GetLineNumber(viewLine);
                if (lineNumber < firstVisibleLineNumber ||
                    lineNumber > lastVisibleLineNumber)
                    continue;
                yield return new LineInfo
                {
                    LineNumber = lineNumber,
                    TextTop = viewLine.Top - textView.ViewportTop
                };
            }
        }

        private static int GetLineNumber(ITextViewLine line)
        {
            return line.Snapshot.GetLineNumberFromPosition(line.Start);
        }

        protected virtual void OnLayoutChanged()
        {
            var handler = LayoutChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
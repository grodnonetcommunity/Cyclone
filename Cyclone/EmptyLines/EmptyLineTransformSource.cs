using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone.EmtyLines
{
    public class EmptyLineTransformSource : ILineTransformSource
    {
        private readonly EmptyLineAdornmentManager _manager;

        public EmptyLineTransformSource(EmptyLineAdornmentManager manager)
        {
            _manager = manager;
        }

        LineTransform ILineTransformSource.GetLineTransform(ITextViewLine line, double yPosition,
            ViewRelativePosition placement)
        {
            var lineNumber = line.Snapshot.GetLineFromPosition(line.Start.Position).LineNumber;
            LineTransform lineTransform;

            // Look up Image for current line and increase line height as necessary
            if (_manager.EmptyLines.ContainsKey(lineNumber))
            {
                var defaultHeight = line.DefaultLineTransform.BottomSpace;
                var image = _manager.EmptyLines[lineNumber];
                lineTransform = new LineTransform(0, image.Height + defaultHeight, 1.0);
            }
            else
            {
                lineTransform = new LineTransform(0, 0, 1.0);
            }
            return lineTransform;
        }
    }
}
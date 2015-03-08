using AV.Cyclone.Sandy.Models;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace AV.Cyclone.EmptyLines
{
    public class EmptyLineTransformSource : ILineTransformSource
    {
        private readonly ICloudCollection cloudCollection;

        public EmptyLineTransformSource(ICloudCollection cloudCollection)
        {
            this.cloudCollection = cloudCollection;
        }

        public LineTransform GetLineTransform(ITextViewLine line, double yPosition, ViewRelativePosition placement)
        {
            if (cloudCollection == null)
                return line.LineTransform;

            var lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position);
            var height = cloudCollection.GetHeight(lineNumber);

            if (line.Height < height)
            {
                return new LineTransform(line.LineTransform.TopSpace,
                    line.LineTransform.BottomSpace + (height - line.Height), line.LineTransform.VerticalScale);
            }

            return line.LineTransform;
        }
    }
}
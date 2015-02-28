using System.Linq;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.OutputPane
{
    public class OutputPaneModel
    {
        public IWpfTextView SourceTextView { get; set; }

        public OutputPaneModel(IWpfTextView sourceTextView)
        {
            SourceTextView = sourceTextView;
            ViewObjectModel = new ViewObjectModel(NuberOfLines, LineHeight);
        }

        public ViewObjectModel ViewObjectModel { get; set; }

        public int NuberOfLines
        {
            get { return SourceTextView.TextSnapshot.Lines.Count(); }
        }

        public double LineHeight
        {
            get { return SourceTextView.LineHeight; }
        }
    }
}
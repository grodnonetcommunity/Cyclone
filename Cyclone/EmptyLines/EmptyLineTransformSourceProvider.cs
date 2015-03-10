using System.ComponentModel.Composition;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

namespace AV.Cyclone.EmptyLines
{
    [Export(typeof(ILineTransformSourceProvider))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class EmptyLineTransformSourceProvider : ILineTransformSourceProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]
        public ICycloneService CycloneService { get; set; }

        public ILineTransformSource Create(IWpfTextView textView)
        {
            return new EmptyLineTransformSource(CycloneService, textView);
        }
    }
}
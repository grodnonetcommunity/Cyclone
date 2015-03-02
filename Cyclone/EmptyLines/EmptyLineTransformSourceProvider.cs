using System.ComponentModel.Composition;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Utilities;

namespace AV.Cyclone.EmtyLines
{
    [Export(typeof (ILineTransformSourceProvider))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class EmptyLineTransformSourceProvider : ILineTransformSourceProvider
    {
        public ILineTransformSource Create(IWpfTextView textView)
        {
            var manager =
                textView.Properties.GetOrCreateSingletonProperty(
                    () => new EmptyLineAdornmentManager(textView, CycloneServiceProvider.GetCycloneService(textView)));
            return new EmptyLineTransformSource(manager);
        }
    }
}
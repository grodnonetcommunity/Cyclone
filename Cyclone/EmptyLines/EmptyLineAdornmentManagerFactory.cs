using System.ComponentModel.Composition;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace AV.Cyclone.EmtyLines
{
    [Export(typeof (IWpfTextViewCreationListener))]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    public class EmptyLineAdornmentManagerFactory : IWpfTextViewCreationListener
    {
        [Export(typeof (AdornmentLayerDefinition))] [Name("EmptyLinesLayer")] [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)] [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)] public AdornmentLayerDefinition EditorAdornmentLayer =
            null;

        public void TextViewCreated(IWpfTextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(
                () => new EmptyLineAdornmentManager(textView, CycloneServiceProvider.GetCycloneService()));
        }
    }
}
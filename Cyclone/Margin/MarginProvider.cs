using System.ComponentModel.Composition;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace AV.Cyclone.Margin
{

    #region EditorMargin Factory

    /// <summary>
    ///     Export a <see cref="IWpfTextViewMarginProvider" />, which returns an instance of the margin for the editor
    ///     to use.
    /// </summary>
    [Export(typeof (IWpfTextViewMarginProvider))]
    [Name("Margin")]
    [Order(After = PredefinedMarginNames.VerticalScrollBar)]
    [MarginContainer(PredefinedMarginNames.Right)]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    internal sealed class MarginProvider : IWpfTextViewMarginProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }


        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin containerMargin)
        {
            var cycloneService = CycloneServiceProvider.GetCycloneService();

            var wpfTextView = wpfTextViewHost.TextView;

            ITextDocument document;

            if (!TextDocumentFactoryService.TryGetTextDocument(wpfTextViewHost.TextView.TextDataModel.DocumentBuffer, out document))
                return null;

            return new Margin(wpfTextView, cycloneService, document.FilePath);
        }
    }

    #endregion
}
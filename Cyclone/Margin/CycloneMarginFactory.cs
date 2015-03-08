using System;
using System.ComponentModel.Composition;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace AV.Cyclone.Margin
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(CycloneMargin.MarginName)]
    [Order(After = PredefinedMarginNames.VerticalScrollBar)]
    [MarginContainer(PredefinedMarginNames.Right)]
    [ContentType("CSharp")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class CycloneMarginFactory : IWpfTextViewMarginProvider
    {
        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        [Import]
        public ICycloneService CycloneService { get; set; }

        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            /*var cloudCollection = CycloneService.GetClouds(textViewHost.TextView);
            if (cloudCollection == null)
                return null;*/

            return new CycloneMargin(CycloneService, textViewHost.TextView, TextDocumentFactoryService);
        }
    }
}
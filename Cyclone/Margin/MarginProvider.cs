﻿using System.ComponentModel.Composition;
using AV.Cyclone.Service;
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
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin containerMargin)
        {
            var cycloneService = GetCycloneManager(wpfTextViewHost);

            var wpfTextView = wpfTextViewHost.TextView;

            return new Margin(wpfTextView, cycloneService);
        }

        private static ICycloneService GetCycloneManager(IWpfTextViewHost wpfTextViewHost)
        {
            return wpfTextViewHost.TextView.Properties.GetOrCreateSingletonProperty
                (() => new CycloneService());
        }
    }

    #endregion
}
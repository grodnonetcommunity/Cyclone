using System;
using System.Windows.Media;
using AV.Cyclone.Sandy.Models;
using Cyclon.Controls;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Margin
{
    class CycloneMargin : IWpfTextViewMargin
    {
        public const string MarginName = "Test.VisualStudioMargin";
        private bool isDisposed;
        private readonly MarginContent marginContent;

        public CycloneMargin(IWpfTextView textView, ICloudCollection cloudCollection)
        {
            VSColorTheme.ThemeChanged += VsColorThemeOnThemeChanged;
            this.marginContent = new MarginContent(new VisualStudioTextViewService(textView), cloudCollection);
            VsColorThemeOnThemeChanged(new ThemeChangedEventArgs(0));
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region IWpfTextViewMargin Members

        public System.Windows.FrameworkElement VisualElement
        {
            get
            {
                ThrowIfDisposed();
                return marginContent;
            }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize
        {
            get
            {
                ThrowIfDisposed();
                return this.marginContent.ActualWidth;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        /// Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of Test.VisualStudioMargin or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == CycloneMargin.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                VSColorTheme.ThemeChanged -= VsColorThemeOnThemeChanged;
                isDisposed = true;
            }
        }

        #endregion

        private void VsColorThemeOnThemeChanged(ThemeChangedEventArgs themeChangedEventArgs)
        {
            var editorColor = VSColorTheme.GetThemedColor(EnvironmentColors.EditorExpansionFillBrushKey);
            this.marginContent.Background = new SolidColorBrush(editorColor.ToWpf());
        }
    }
}
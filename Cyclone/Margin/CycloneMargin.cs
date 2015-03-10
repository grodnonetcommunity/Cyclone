using System;
using System.Windows.Media;
using System.Windows.Threading;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using AV.Cyclone.Service;
using Cyclon.Controls;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Margin
{
    class CycloneMargin : IWpfTextViewMargin
    {
        private readonly ICycloneService cycloneService;
        private readonly IWpfTextView textView;
        private readonly ITextDocumentFactoryService documentFactoryService;
        public const string MarginName = "Cyclone.WeatherStationMargin";
        private bool isDisposed;
        private readonly MarginContent marginContent;
        private readonly Dispatcher dispatcher;

        public CycloneMargin(ICycloneService cycloneService, IWpfTextView textView, ITextDocumentFactoryService documentFactoryService)
        {
            VSColorTheme.ThemeChanged += VsColorThemeOnThemeChanged;
            this.cycloneService = cycloneService;
            this.textView = textView;
            this.documentFactoryService = documentFactoryService;
            this.cycloneService.CycloneChanged += CycloneServiceOnCycloneChanged;
            this.marginContent = new MarginContent(new VisualStudioTextViewService(textView));
            this.marginContent.Width = 0;
            dispatcher = Dispatcher.CurrentDispatcher;
            VsColorThemeOnThemeChanged(new ThemeChangedEventArgs(0));
        }

        private void TextBufferOnChanged(object sender, TextContentChangedEventArgs e)
        {
            ITextDocument document;

            if (!documentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
                return;

            if (ExamplesPackage.WeatherStation == null) return;
            ExamplesPackage.WeatherStation.FileUpdated(document.FilePath, e.After.GetText());
        }

        private void CycloneServiceOnCycloneChanged(object sender, CycloneEventArgs cycloneEventArgs)
        {
            if (ExamplesPackage.WeatherStation == null) return;
            ExamplesPackage.WeatherStation.Executed += WeatherStationOnExecuted;
        }

        private void WeatherStationOnExecuted(object sender, EventArgs eventArgs)
        {
            dispatcher.BeginInvoke((Action) GetCloudCollection);
        }

        private void GetCloudCollection()
        {
            CloudCollection = cycloneService.GetClouds(textView);
            textView.TextBuffer.Changed += TextBufferOnChanged;
        }

        public ICloudCollection CloudCollection
        {
            get { return marginContent.CloudCollection; }
            set
            {
                marginContent.CloudCollection = value;
                marginContent.Width = marginContent.CloudCollection == null ? 0 : Double.NaN;
            }
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
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Margin
{
    public abstract class MarginBase : DockPanel, IWpfTextViewMargin
    {
        protected Dispatcher _dispatcher;
        private bool _isDisposed = false;
        private FrameworkElement _previewControl;
        protected ColumnDefinition columnDefinition;

        protected MarginBase()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher.BeginInvoke(
                new Action(CreateMarginControls), DispatcherPriority.ApplicationIdle, null);
        }

        protected abstract FrameworkElement CreatePreviewControl();

        protected virtual void CreateMarginControls()
        {
            int width = 0;

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(5, GridUnitType.Pixel)});
            columnDefinition = new ColumnDefinition {Width = new GridLength(width, GridUnitType.Pixel)};
            grid.ColumnDefinitions.Add(columnDefinition);
            grid.RowDefinitions.Add(new RowDefinition());

            _previewControl = CreatePreviewControl();

            if (_previewControl == null)
                return;

            grid.Children.Add(_previewControl);
            Children.Add(grid);

            Grid.SetColumn(_previewControl, 2);
            Grid.SetRow(_previewControl, 0);

            var splitter = new GridSplitter();
            splitter.Width = 5;
            splitter.ResizeDirection = GridResizeDirection.Columns;
            splitter.VerticalAlignment = VerticalAlignment.Stretch;
            splitter.HorizontalAlignment = HorizontalAlignment.Stretch;
            splitter.DragCompleted += splitter_DragCompleted;

            grid.Children.Add(splitter);
            Grid.SetColumn(splitter, 1);
            Grid.SetRow(splitter, 0);
        }

        private void splitter_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (double.IsNaN(_previewControl.ActualWidth)) return;
            //using (var key = WebEssentialsPackage.Instance.UserRegistryRoot)
            //{
            //    key.SetValue("WE_" + _settingsKey, _previewControl.ActualWidth, RegistryValueKind.DWord);
            //}
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #region IWpfTextViewMargin Members

        /// <summary>
        ///     The <see cref="Sytem.Windows.FrameworkElement" /> that implements the visual representation
        ///     of the margin.
        /// </summary>
        public FrameworkElement VisualElement
        {
            get { return this; }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize
        {
            // Since this is a horizontal margin, its width will be bound to the width of the text view.
            // Therefore, its size is its height.
            get
            {
                ThrowIfDisposed();
                return ActualHeight;
            }
        }

        public bool Enabled
        {
            get { return true; }
        }

        /// <summary>
        ///     Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of EditorMargin1 or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == GetType().Name) ? this : null;
        }


        /// <summary>Releases all resources used by the MarginBase.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Releases the unmanaged resources used by the MarginBase and optionally releases the managed resources.</summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only unmanaged
        ///     resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        #endregion
    }
}
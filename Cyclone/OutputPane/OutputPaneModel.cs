using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.OutputPane
{
    public class OutputPaneModel : INotifyPropertyChanged
    {
        private double _zoomLevel;

        public OutputPaneModel(IWpfTextView sourceTextView)
        {
            SourceTextView = sourceTextView;
            ViewObjectModel = new ViewObjectModel(NuberOfLines, LineHeight, CycloneServiceProvider.GetCycloneService(SourceTextView));

            ZoomLevel = SourceTextView.ZoomLevel;
        }

        public void Reinit()
        {
            ViewObjectModel = new ViewObjectModel(NuberOfLines, LineHeight, CycloneServiceProvider.GetCycloneService(SourceTextView));
        }

        public IWpfTextView SourceTextView { get; set; }
        public ViewObjectModel ViewObjectModel { get; set; }

        public int NuberOfLines
        {
            get { return SourceTextView.TextSnapshot.Lines.Count(); }
        }

        public double LineHeight
        {
            get { return SourceTextView.LineHeight; }
        }

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                _zoomLevel = value/100;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
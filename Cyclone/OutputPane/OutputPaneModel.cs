﻿using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.OutputPane
{
    public class OutputPaneModel : INotifyPropertyChanged
    {
        private double _zoomLevel;
        public readonly string FilePath;

        public OutputPaneModel(IWpfTextView sourceTextView, string filePath)
        {
            FilePath = filePath;
            SourceTextView = sourceTextView;
            ViewObjectModel = new ViewObjectModel(NuberOfLines, LineHeight, CycloneServiceProvider.GetCycloneService(),
                FilePath);
            SourceTextView.TextBuffer.Changed += Reinitialize;
            ZoomLevel = SourceTextView.ZoomLevel;
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

        private void Reinitialize(object sender, TextContentChangedEventArgs e)
        {
            if (ExamplesPackage.WeatherStation != null)
                ExamplesPackage.WeatherStation.FileUpdated(FilePath, e.After.GetText());
        }

        public void Reinit( /*List<Execution> operations*/)
        {
            ViewObjectModel = new ViewObjectModel(NuberOfLines, LineHeight, CycloneServiceProvider.GetCycloneService(),
                FilePath);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
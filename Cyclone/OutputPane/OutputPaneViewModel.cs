using System.ComponentModel;
using System.Runtime.CompilerServices;
using AV.Cyclone.Annotations;

namespace AV.Cyclone.OutputPane
{
    public class OutputPaneViewModel : INotifyPropertyChanged
    {
        private readonly OutputPaneView _view;
        private readonly OutputPaneModel _model;

        public OutputPaneViewModel(OutputPaneView view, OutputPaneModel model)
        {
            _view = view;
            _model = model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
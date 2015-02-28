using System.Windows.Controls;

namespace AV.Cyclone.OutputPane
{
    /// <summary>
    ///     Interaction logic for OutputPaneView.xaml
    /// </summary>
    public partial class OutputPaneView : UserControl
    {
        public OutputPaneView(OutputPaneModel model)
        {
            InitializeComponent();
            ViewModel = new OutputPaneViewModel(this, model);
            DataContext = ViewModel;
        }

        public OutputPaneViewModel ViewModel { get; set; }
    }
}
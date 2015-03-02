using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AV.Cyclone.Annotations;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.OperationParser;
using AV.Cyclone.Service;

namespace AV.Cyclone.OutputPane
{
    public class ViewObjectModel : INotifyPropertyChanged, IEnumerable
    {
        private readonly double _lineHeight;
        private readonly int _numberOfLines;
        private readonly ICycloneService _cycloneService;
        private List<Execution> operations;

        public ViewObjectModel(int numberOfLines, double lineHeight)
        {
            _numberOfLines = numberOfLines;
            _lineHeight = lineHeight;
        }

        public ViewObjectModel(int numberOfLines, double lineHeight, ICycloneService cycloneService, List<Execution> operations) : this(numberOfLines, lineHeight)
        {
            this._cycloneService = cycloneService;
            this.operations = operations;
            Init();
        }

        public FrameworkElement this[int key]
        {
            get
            {
                if (key < 0 || key > Elements.Count - 1)
                {
                    return null;
                }
                return Elements[key];
            }
            set { Elements[key] = value; }
        }

        public ObservableCollection<FrameworkElement> Elements { get; set; }

        public IEnumerator GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Init()
        {
            Elements = new ObservableCollection<FrameworkElement>();
            
            UIGenerator generator = new UIGenerator(operations);
            OutComponent components = generator.GetOutputComponents(ExamplesPackage.Dte.ActiveDocument.FullName);

            for (var i = 0; i < _numberOfLines + 100; i++)
            {
                var elemToAdd = new UniformGrid();
                elemToAdd.MinHeight = _lineHeight;
//                if (i % 2 == 0)
//                {
//                    elemToAdd.Background = Brushes.LightGray;
//                }
//                else
//                {
//                    elemToAdd.Background = Brushes.DimGray;
//                }
                var component = components[i];
                if (component != null)
                {
                    elemToAdd.Children.Add(component);
                }
                if (components[i] == null)
                {
                    elemToAdd.Height = _lineHeight;
                }

                var wrap = new UniformGrid();
                wrap.Children.Add(elemToAdd);
                Elements.Add(wrap);
            }
        }

        public void SetAdorment(int lineIndex)
        {
            var height = this[lineIndex].ActualHeight - _lineHeight;
            if (height < 0)
            {
                height = 0;
            }
            _cycloneService.ExpandLine(lineIndex, height);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
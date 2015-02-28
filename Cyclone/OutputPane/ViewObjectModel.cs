﻿using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AV.Cyclone.Annotations;
using AV.Cyclone.Service;

namespace AV.Cyclone.OutputPane
{
    public class ViewObjectModel : INotifyPropertyChanged, IEnumerable
    {
        private readonly double _lineHeight;
        private readonly int _numberOfLines;
        private readonly ICycloneService _cycloneService;

        public ViewObjectModel(int numberOfLines, double lineHeight)
        {
            _numberOfLines = numberOfLines;
            _lineHeight = lineHeight;
        }

        public ViewObjectModel(int numberOfLines, double lineHeight, ICycloneService cycloneService) : this(numberOfLines, lineHeight)
        {
            this._cycloneService = cycloneService;
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
            for (var i = 0; i < _numberOfLines; i++)
            {
                var tb = new TextBlock();
                tb.Text = "line: " + (i + 1);

                //if (i == 10)
                //{
                //    var newHeigth = 50;
                //    tb.Height = newHeigth;
                //    tb.Background = Brushes.Red;
                //    Elements.Add(tb);
                //    _cycloneService.ExpandLine(i, newHeigth);
                //    continue;
                //}

                tb.Height = _lineHeight;
                if (i%2 == 0)
                {
                    tb.Background = Brushes.LightGray;
                }
                else
                {
                    tb.Background = Brushes.DimGray;
                }
                Elements.Add(tb);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
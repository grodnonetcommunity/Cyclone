﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using AV.Cyclone.Sandy.Models;
using JetBrains.Annotations;

namespace Cyclon.Controls
{
    public partial class MarginContent : INotifyPropertyChanged
    {
        private readonly ITextViewService textViewService;
        private ICloudCollection cloudCollection;

        public MarginContent(ITextViewService textViewService)
        {
            this.textViewService = textViewService;
            InitializeComponent();
            this.DataContext = this;
            this.textViewService.LayoutChanged += TextViewOnLayoutChanged;
            UpdateScroll();
        }

        public ICloudCollection CloudCollection
        {
            get { return cloudCollection; }
            set
            {
                if (Equals(value, cloudCollection)) return;
                cloudCollection = value;
                OnPropertyChanged();
                UpdateScroll();
            }
        }

        private void UpdateScroll()
        {
            if (cloudCollection == null) return;
            var scale = textViewService.Scale;
            contentCanvas.Children.Clear();
            foreach (var line in textViewService.GetVisibleLines())
            {
                var lineControl = cloudCollection.GetCloud(line.LineNumber);
                if (lineControl == null)
                    continue;

                if (!ReferenceEquals(lineControl.Parent, this) && lineControl.Parent != null)
                    lineControl.Parent.RemoveChild(lineControl);

                SetFontSize(lineControl, scale);

                contentCanvas.Children.Add(lineControl);
                Canvas.SetLeft(lineControl, 0);
                Canvas.SetTop(lineControl, line.TextTop * scale);
            }
        }

        private static void SetFontSize(FrameworkElement control, double scale)
        {
            if (control is Control)
            {
                ((Control) control).FontSize = 12*scale;
            }
            else if (control is Panel)
            {
                foreach (var child in ((Panel)control).Children)
                {
                    SetFontSize((FrameworkElement)child, scale);
                }
            }
            else if (control is TextBlock)
            {
                ((TextBlock) control).FontSize = 12*scale;
            }
            else if (control is Decorator)
            {
                SetFontSize((FrameworkElement) ((Decorator) control).Child, scale);
            }
            else
            {
                
            }
        }

        private void TextViewOnLayoutChanged(object sender, EventArgs eventArgs)
        {
            UpdateScroll();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.Models.Operations;
using JetBrains.Annotations;

namespace AV.Cyclone.Sandy.OperationParser
{
    public class UiGenerator2 : INotifyPropertyChanged, IUIGenerator
    {
        private static readonly SandyColorProvider sandyColorProvider = new SandyColorProvider();
        private SandyColorProvider colorProvider = sandyColorProvider;
        private readonly Dictionary<int, UIElement> controls = new Dictionary<int, UIElement>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Generate(ExecuteTree executeTree)
        {
            foreach (var line in executeTree.Lines)
            {
                var grid = CreateGrid(line.Value.Executions[0], "Call0_");
                grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                grid.Arrange(new Rect(grid.DesiredSize));
                controls[line.Key] = grid;
            }
        }

        public OutComponent GetOutputComponents()
        {
            return new OutComponent(controls);
        }

        public SandyColorProvider ColorProvider
        {
            get { return colorProvider; }
            set
            {
                if (Equals(value, colorProvider)) return;
                colorProvider = value;
                OnPropertyChanged();
            }
        }

        private int GetColumns(ExecuteTreeLineItem lineItem)
        {
            if (lineItem is AssignOperationExecuteTreeLineItem)
            {
                return 1;
            }
            var listExecuteTreeLineItem = lineItem as ListExecuteTreeLineItem;
            if (listExecuteTreeLineItem != null)
            {
                return listExecuteTreeLineItem.Items.Max(e => e.Key + 1);
            }
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", lineItem.GetType().Name));
        }

        private Grid CreateGrid(ExecuteTreeLineItem lineItem, string sharedScopeName)
        {
            if (lineItem is AssignOperationExecuteTreeLineItem)
                return CreateGrid((AssignOperationExecuteTreeLineItem)lineItem, sharedScopeName);
            if (lineItem is ListExecuteTreeLineItem)
                return CreateGrid((ListExecuteTreeLineItem)lineItem, sharedScopeName);
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", lineItem.GetType().Name));
        }

        private Grid CreateGrid(AssignOperationExecuteTreeLineItem assignOperationItem, string sharedScopeName)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = sharedScopeName });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });

            var textBlock = CreateTextBlock(assignOperationItem.AssignOperation);
            Grid.SetColumn(textBlock, 0);
            Grid.SetRow(textBlock, 0);

            grid.Children.Add(textBlock);

            return grid;
        }

        private Grid CreateGrid(ListExecuteTreeLineItem listItem, string sharedScopeName)
        {
            var grid = new Grid();
            for (int i = 0; i < GetColumns(listItem); i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = sharedScopeName + "_I" + i});
            }
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });


            foreach (var item in listItem.Items)
            {
                var itemGrid = CreateGrid(item.Value, sharedScopeName + "_I" + item.Key);
                Grid.SetColumn(itemGrid, item.Key);
                Grid.SetRow(itemGrid, 0);
                grid.Children.Add(itemGrid);
            }

            return grid;
        }

        private Control CreateTextBlock(AssignOperation assignOperation)
        {
            return CreateTextBlock(assignOperation.VariableName, assignOperation.VariableValue);
        }

        private Control CreateTextBlock(string variable, object value)
        {
            var textBlock = new TextBlock();
            textBlock.Inlines.Add(CreateRun("var ", "ColorProvider.KeywordBrush"));
            textBlock.Inlines.Add(CreateRun(variable, "ColorProvider.IdentifierBrush"));
            textBlock.Inlines.Add(CreateRun(" = ", "ColorProvider.IdentifierBrush"));
            textBlock.Inlines.Add(CreateRun(value.ToString(), "ColorProvider.NumberBrush"));

            var contentControl = new ContentControl
                                 {
                                     Content = textBlock,
                                     FontFamily = new FontFamily("Consolas"),
                                     FontSize = 12,
                                     UseLayoutRounding = true
                                 };
            // TODO: Move measure into Grid creation
            //contentControl.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //contentControl.Arrange(new Rect(contentControl.DesiredSize));
            return contentControl;
        }

        private Run CreateRun(string text, string path)
        {
            var varRun = new Run(text);
            varRun.SetBinding(TextElement.ForegroundProperty, CreateBinding(path));
            return varRun;
        }

        private Binding CreateBinding(string path)
        {
            return new Binding(path)
            {
                Source = this,
            };
        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
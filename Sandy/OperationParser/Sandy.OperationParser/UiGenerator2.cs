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
        private static readonly string[] keywordVariableNames = {"while", "if", "return"};

        private static readonly SandyColorProvider sandyColorProvider = new SandyColorProvider();
        private SandyColorProvider colorProvider = sandyColorProvider;
        private readonly Dictionary<int, UIElement> controls = new Dictionary<int, UIElement>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Generate(ExecuteTree executeTree)
        {
            var methodName = "Method";
            var columns = GetColumns(executeTree);
            foreach (var lineItem in executeTree.Lines)
            {
                var lineNumber = lineItem.Key;
                var line = lineItem.Value;

                var variables = GetVariables(line);
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_Type"});
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_Name"});
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_EqualSign"});
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_Values"});
                for (var r = 0; r < variables.Count; r++)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                    CreateVariableTextBlock(grid, r, variables[r]);
                }

                for (var r = 0; r < variables.Count; r++)
                {
                    var executeTreeLineItems = line.GetList(variables[r]);
                    for (var i = 0; i < executeTreeLineItems.Count; i++)
                    {
                        var element = CreateElement(executeTreeLineItems[i], methodName + "_Values", columns);

                        Grid.SetColumn(element, 3 + i);
                        Grid.SetRow(element, r);

                        grid.Children.Add(element);
                    }
                }

                grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                grid.Arrange(new Rect(grid.DesiredSize));
                controls[lineNumber] = grid;
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

        private List<string> GetVariables(ExecuteTreeLine line)
        {
            var variables = new List<string>();
            foreach (var variable in line.Variables)
            {
                if (!variables.Contains(variable))
                    variables.Add(variable);
            }
            return variables;
        }

        private void AddVariables(List<string> variables, ExecuteTreeLineItem lineItem)
        {
            if (lineItem is AssignOperationExecuteTreeLineItem)
            {
                AddVariable(variables, ((AssignOperationExecuteTreeLineItem)lineItem).AssignOperation.VariableName);
            }
            else if (lineItem is ListExecuteTreeLineItem)
            {
                foreach (var item in ((ListExecuteTreeLineItem)lineItem).Items)
                {
                    AddVariables(variables, item.Value);
                }
            }
        }

        private void AddVariable(List<string> variables, string variableName)
        {
            if (variables.Contains(variableName)) return;
            variables.Add(variableName);
        }

        private int GetColumns(ExecuteTree executeTree)
        {
            return executeTree.Lines.Max(e => GetColumns(e.Value));
        }

        private int GetColumns(ExecuteTreeLine line)
        {
            return line.Executions.SelectMany(e => e.Value).Max(e => GetColumns(e));
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

        private FrameworkElement CreateElement(ExecuteTreeLineItem lineItem, string sharedScopeName, int columns)
        {
            if (lineItem is AssignOperationExecuteTreeLineItem)
                return CreateElement((AssignOperationExecuteTreeLineItem)lineItem);
            if (lineItem is ListExecuteTreeLineItem)
                return CreateElement((ListExecuteTreeLineItem)lineItem, sharedScopeName, columns);
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", lineItem.GetType().Name));
        }

        private Control CreateElement(AssignOperationExecuteTreeLineItem assignOperationItem)
        {
            return CreateTextBlock(assignOperationItem.AssignOperation);
        }

        private FrameworkElement CreateElement(ListExecuteTreeLineItem listItem, string sharedScopeName, int columns)
        {
            var grid = new Grid
                       {
                           Margin = new Thickness(5, 0, 5, 0),
                           VerticalAlignment = VerticalAlignment.Center
                       };
            for (int i = 0; i < columns; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = sharedScopeName + "_I" + i});
            }
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });


            foreach (var item in listItem.Items)
            {
                var itemElement = CreateElement(item.Value, sharedScopeName + "_I" + item.Key, GetColumns(listItem));
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1, 0, 1, 0),
                    Margin = new Thickness(0, 0, -1, 0),
                    Padding = new Thickness(2, 2, 2, 2),
                    Child = itemElement,
                };
                Grid.SetColumn(border, item.Key);
                Grid.SetRow(border, 0);
                grid.Children.Add(border);
            }


            return grid;
        }

        private void CreateVariableTextBlock(Grid grid, int row, string variable)
        {
            if (Array.IndexOf(keywordVariableNames, variable) >= 0)
            {
                AddTextBlock(grid, 0, 3, row, variable, "ColorProvider.KeywordBrush");
            }
            else
            {
                AddTextBlock(grid, 0, 1, row, "var", "ColorProvider.KeywordBrush");
                AddTextBlock(grid, 1, 1, row, variable, "ColorProvider.IdentifierBrush");
                AddTextBlock(grid, 2, 1, row, "=", "ColorProvider.OperatorBrush");
            }
        }

        private void AddTextBlock(Grid grid, int column, int columnSpan, int row, string text, string foregroundBinding)
        {
            var textBlock = CreateTextBlock(text, foregroundBinding);

            Grid.SetColumn(textBlock, column);
            Grid.SetColumnSpan(textBlock, columnSpan);
            Grid.SetRow(textBlock, row);

            grid.Children.Add(textBlock);
        }

        private Control CreateTextBlock(AssignOperation assignOperation)
        {
            return CreateTextBlock(assignOperation.VariableValue);
        }

        private Control CreateTextBlock(object value)
        {
            return new ContentControl
                   {
                       Content = CreateTextBlock(CreateRun(value)),
                       FontFamily = new FontFamily("Consolas"),
                       FontSize = 12,
                       UseLayoutRounding = true
                   };
        }

        private TextBlock CreateTextBlock(string text, string foregroundBinding)
        {
            return CreateTextBlock(CreateRun(text, foregroundBinding));
        }

        private static TextBlock CreateTextBlock(Run run)
        {
            var textBlock = new TextBlock(run)
                            {
                                Margin = new Thickness(5, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center
                            };
            return textBlock;
        }

        private Run CreateRun(object value)
        {
            if (value is int)
            {
                return CreateRun(value.ToString(), "ColorProvider.NumberBrush");
            }
            if (value is bool)
            {
                return CreateRun((bool)value ? "true" : "false", "ColorProvider.KeywordBrush");
            }
            return CreateRun(value.ToString(), "ColorProvider.IdentifierBrush");
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
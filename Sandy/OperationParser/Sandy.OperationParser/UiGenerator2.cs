﻿using System;
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
        private readonly CompositeOutComponent outComponent = new CompositeOutComponent();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Generate(ExecuteTree executeTree)
        {
            var methodName = executeTree.MethodName;
            var deep = GetDeep(executeTree);
            var columnsDeep = new int[deep];
            GetDeepColumns(executeTree, columnsDeep);
            var controls = new Dictionary<int, UIElement>();
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
                    var executeTreeLineItem = line.GetLineItem(variables[r]);
                    var element = CreateElement(executeTreeLineItem, methodName + "_Values", columnsDeep, 0);

                    Grid.SetColumn(element, 3);
                    Grid.SetRow(element, r);

                    grid.Children.Add(element);
                }

                grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                grid.Arrange(new Rect(grid.DesiredSize));
                controls[lineNumber] = grid;
            }
            outComponent.AddComponent(new OutComponent(controls));
        }

        private void GetDeepColumns(ExecuteTree executeTree, int[] columnsDeep)
        {
            GetDeepColumns(executeTree.Lines.Select(e => e.Value), columnsDeep);
        }

        private void GetDeepColumns(IEnumerable<ExecuteTreeLine> executeTreeLines, int[] columnsDeep)
        {
            foreach (var executeTreeLine in executeTreeLines)
            {
                foreach (var executeTreeLineItem in executeTreeLine.Executions.Values)
                {
                    GetDeepColumns(executeTreeLineItem, columnsDeep, 0);
                }
            }
        }

        private void GetDeepColumns(ExecuteTreeLineItem executeTreeLineItem, int[] columnsDeep, int deep)
        {
            if (executeTreeLineItem is AssignOperationExecuteTreeLineItem)
            {
                columnsDeep[deep] = Math.Max(columnsDeep[deep], 1);
            }
            else if (executeTreeLineItem is ListExecuteTreeLineItem)
            {
                var items = ((ListExecuteTreeLineItem)executeTreeLineItem).Items;
                columnsDeep[deep] = Math.Max(columnsDeep[deep], items.Count);
                foreach (var item in items)
                {
                    GetDeepColumns(item.Value, columnsDeep, deep + 1);
                }
            }
        }

        private int GetDeep(ExecuteTree executeTree)
        {
            return executeTree.Lines.Max(e => GetDeep(e.Value));
        }

        private int GetDeep(ExecuteTreeLine executeTreeLine)
        {
            return executeTreeLine.Executions.Values.Max(e => GetDeep(e));
        }

        private int GetDeep(ExecuteTreeLineItem executeTreeLineItem)
        {
            if (executeTreeLineItem is AssignOperationExecuteTreeLineItem)
                return 1;
            if (executeTreeLineItem is ListExecuteTreeLineItem)
            {
                return ((ListExecuteTreeLineItem)executeTreeLineItem)
                           .Items.Select(e => e.Value).Max(e => GetDeep(e)) + 1;
            }
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", executeTreeLineItem.GetType().Name));
        }

        public IOutComponent GetOutputComponents()
        {
            return outComponent;
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
            return line.Executions.Max(e => GetColumns(e.Value));
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

        private FrameworkElement CreateElement(ExecuteTreeLineItem lineItem, string sharedScopeName, int[] columnsDeep, int deep)
        {
            if (lineItem is AssignOperationExecuteTreeLineItem)
                return CreateElement((AssignOperationExecuteTreeLineItem)lineItem);
            if (lineItem is ListExecuteTreeLineItem)
                return CreateElement((ListExecuteTreeLineItem)lineItem, sharedScopeName, columnsDeep, deep);
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", lineItem.GetType().Name));
        }

        private Control CreateElement(AssignOperationExecuteTreeLineItem assignOperationItem)
        {
            return CreateTextBlock(assignOperationItem.AssignOperation);
        }

        private FrameworkElement CreateElement(ListExecuteTreeLineItem listItem, string sharedScopeName, int[] columnsDeep, int deep)
        {
            var grid = new Grid
                       {
                           Margin = new Thickness(5, 0, 5, 0),
                           VerticalAlignment = VerticalAlignment.Center
                       };
            for (int i = 0; i < columnsDeep[deep]; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = sharedScopeName + "_I" + i});
            }
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });


            foreach (var item in listItem.Items)
            {
                var itemElement = CreateElement(item.Value, sharedScopeName + "_I" + item.Key, columnsDeep, deep + 1);
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
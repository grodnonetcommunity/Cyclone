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
using AV.Cyclone.Katrina.Executor.Interfaces;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.Models.Operations;
using JetBrains.Annotations;

namespace AV.Cyclone.Sandy.OperationParser
{
    public class UiGenerator : INotifyPropertyChanged, IUIGenerator
    {
        private const string ColorProviderKeywordBrushPath = "ColorProvider.KeywordBrush";
        private const string ColorProviderOperatorBrush = "ColorProvider.OperatorBrush";
        private const string ColorProviderIdentifierBrushPath = "ColorProvider.IdentifierBrush";
        private const string ColorProviderNumberBrushPath = "ColorProvider.NumberBrush";
        private const string ColorProviderStringBrushPath = "ColorProvider.StringBrush";
        private const string ColorProviderCharacterBrushPath = "ColorProvider.StringBrush";

        private static readonly string[] keywordVariableNames = {"while", "do", "for", "if", "return", "exception"};

        private readonly CompositeOutComponent compositeOutComponent = new CompositeOutComponent();

        public event PropertyChangedEventHandler PropertyChanged;

        public void Generate(ExecuteTree executeTree)
        {
            var methodName = executeTree.MethodName;
            var deep = GetDeep(executeTree);
            var columnsDeep = new int[deep];
            GetDeepColumns(executeTree, columnsDeep);
            var controls = new Dictionary<int, UIElement>();
            var outComponent = new OutComponent(controls);
            foreach (var lineItem in executeTree.Lines)
            {
                var lineNumber = lineItem.Key;
                var line = lineItem.Value;

                var variables = GetVariables(line);
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_Type_Name"});
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_EqualSign"});
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto), SharedSizeGroup = methodName + "_Values"});
                for (var r = 0; r < variables.Count; r++)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto) });
                    CreateVariableTextBlock(grid, outComponent, r, variables[r]);
                }

                for (var r = 0; r < variables.Count; r++)
                {
                    var executeTreeLineItem = line.GetLineItem(variables[r]);
                    var element = CreateElement(outComponent, executeTreeLineItem, methodName + "_Values", columnsDeep, 0);

                    Grid.SetColumn(element, 2);
                    Grid.SetRow(element, r);

                    grid.Children.Add(element);
                }

                grid.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                grid.Arrange(new Rect(grid.DesiredSize));
                controls[lineNumber] = grid;
            }
            if (controls.Count > 0)
            {
                outComponent.Freeze();
                compositeOutComponent.AddComponent(outComponent);
            }
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
            if (executeTree.Lines.Count == 0) return 1;
            return executeTree.Lines.Max(e => GetDeep(e.Value));
        }

        private int GetDeep(ExecuteTreeLine executeTreeLine)
        {
            if (executeTreeLine.Executions.Count == 0) return 1;
            return executeTreeLine.Executions.Values.Max(e => GetDeep(e));
        }

        private int GetDeep(ExecuteTreeLineItem executeTreeLineItem)
        {
            if (executeTreeLineItem is AssignOperationExecuteTreeLineItem)
                return 1;
            if (executeTreeLineItem is ListExecuteTreeLineItem)
            {
                var items = ((ListExecuteTreeLineItem)executeTreeLineItem).Items;
                if (items.Count == 0) return 1;
                return items.Max(e => GetDeep(e.Value)) + 1;
            }
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", executeTreeLineItem.GetType().Name));
        }

        public IOutComponent GetOutputComponents()
        {
            return compositeOutComponent;
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

        private FrameworkElement CreateElement(OutComponent outComponent, ExecuteTreeLineItem lineItem, string sharedScopeName, int[] columnsDeep, int deep)
        {
            if (lineItem is AssignOperationExecuteTreeLineItem)
                return CreateElement(outComponent, (AssignOperationExecuteTreeLineItem)lineItem);
            if (lineItem is ListExecuteTreeLineItem)
                return CreateElement(outComponent, (ListExecuteTreeLineItem)lineItem, sharedScopeName, columnsDeep, deep);
            throw new Exception(string.Format("Unknow ExecuteTreeLineItem type: {0}", lineItem.GetType().Name));
        }

        private Control CreateElement(OutComponent outComponent, AssignOperationExecuteTreeLineItem assignOperationItem)
        {
            return CreateTextBlock(outComponent, assignOperationItem.AssignOperation);
        }

        private FrameworkElement CreateElement(OutComponent outComponent, ListExecuteTreeLineItem listItem, string sharedScopeName, int[] columnsDeep, int deep)
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
                var itemElement = CreateElement(outComponent, item.Value, sharedScopeName + "_I" + item.Key, columnsDeep, deep + 1);
                var border = new Border
                {
                    BorderThickness = new Thickness(1, 0, 1, 0),
                    Margin = new Thickness(0, 0, -1, 0),
                    Padding = new Thickness(2, 2, 2, 2),
                    Child = itemElement,
                };
                border.SetBinding(Border.BorderBrushProperty, CreateBinding(outComponent, ColorProviderOperatorBrush));
                Grid.SetColumn(border, item.Key);
                Grid.SetRow(border, 0);
                grid.Children.Add(border);
            }


            return grid;
        }

        private void CreateVariableTextBlock(Grid grid, OutComponent outComponent, int row, string variable)
        {
            if (Array.IndexOf(keywordVariableNames, variable) >= 0)
            {
                AddTextBlock(grid, outComponent, 0, row, variable, ColorProviderKeywordBrushPath);
            }
            else
            {
                AddTextBlock(grid, outComponent, 0, row, new[] { "var ", variable }, new[] { ColorProviderKeywordBrushPath, ColorProviderIdentifierBrushPath });
                AddTextBlock(grid, outComponent, 1, row, "=", ColorProviderOperatorBrush);
            }
        }

        private void AddTextBlock(Grid grid, OutComponent outComponent, int column, int row, string text, string foregroundBinding)
        {
            AddTextBlock(grid, outComponent, column, row, new[] { text }, new[] { foregroundBinding });
        }

        private void AddTextBlock(Grid grid, OutComponent outComponent, int column, int row, string[] texts, string[] foregroundsBinding)
        {
            var textBlock = CreateTextBlock(outComponent, texts, foregroundsBinding);

            Grid.SetColumn(textBlock, column);
            Grid.SetRow(textBlock, row);

            grid.Children.Add(textBlock);
        }

        private Control CreateTextBlock(OutComponent outComponent, AssignOperation assignOperation)
        {
            return CreateTextBlock(outComponent, assignOperation.VariableValue);
        }

        private Control CreateTextBlock(OutComponent outComponent, object value)
        {
            return new ContentControl
                   {
                       Content = CreateTextBlock(CreateRun(outComponent, value)),
                       FontFamily = new FontFamily("Consolas"),
                       FontSize = 12,
                       UseLayoutRounding = true
                   };
        }

        private TextBlock CreateTextBlock(OutComponent outComponent, IReadOnlyList<string> texts, IReadOnlyList<string> foregroundsBinding)
        {
            var runs = new Inline[texts.Count];
            for (var i = 0; i < texts.Count; i++)
            {
                var text = texts[i];
                var foregroundBinding = foregroundsBinding[i];
                runs[i] = CreateRun(outComponent, text, foregroundBinding);
            }

            return CreateTextBlock(runs);
        }

        private static TextBlock CreateTextBlock(Run run)
        {
            return CreateTextBlock(new[] {run});
        }

        private static TextBlock CreateTextBlock(IEnumerable<Inline> runs)
        {
            var textBlock = new TextBlock()
                            {
                                Margin = new Thickness(5, 0, 5, 0),
                                VerticalAlignment = VerticalAlignment.Center,
                            };
            textBlock.Inlines.AddRange(runs);
            return textBlock;
        }

        private IEnumerable<Run> CreateRun(OutComponent outComponent, object value)
        {
            if (value == null)
            {
                return new[] {CreateRun(outComponent, null, ColorProviderKeywordBrushPath)};
            }
            if (value is int)
            {
                return new[] { CreateRun(outComponent, value.ToString(), ColorProviderNumberBrushPath) };
            }
            if (value is bool)
            {
                return new[] { CreateRun(outComponent, (bool)value ? "true" : "false", ColorProviderKeywordBrushPath) };
            }
            if (value is string)
            {
                return new[] { CreateRun(outComponent, '"' + value.ToString() + '"', ColorProviderStringBrushPath) };
            }
            if (value is char)
            {
                return new[] { CreateRun(outComponent, "'" + value + "'", ColorProviderCharacterBrushPath) };
            }
            if (value is ToStringValue)
            {
                return new[] { CreateRun(outComponent, ((ToStringValue)value).Value, ColorProviderIdentifierBrushPath) };
            }
            if (value.GetType().IsArray)
            {
                var arrayRuns = new List<Run>();
                arrayRuns.Add(CreateRun(outComponent, "[", ColorProviderIdentifierBrushPath));
                // TODO: Work with multi-dimension array
                var array = (Array)value;
                for (var i = 0; i < array.Length; i++)
                {
                    var item = array.GetValue(i);
                    arrayRuns.AddRange(CreateRun(outComponent, item));
                    if (i < array.Length - 1)
                    {
                        arrayRuns.Add(CreateRun(outComponent, ", ", ColorProviderIdentifierBrushPath));
                    }
                }
                arrayRuns.Add(CreateRun(outComponent, "]", ColorProviderIdentifierBrushPath));
                return arrayRuns;
            }
            return new[] { CreateRun(outComponent, value.ToString(), ColorProviderIdentifierBrushPath) };
        }

        private Run CreateRun(OutComponent outComponent, string text, string path)
        {
            var varRun = new Run(text);
            varRun.SetBinding(TextElement.ForegroundProperty, CreateBinding(outComponent, path));
            return varRun;
        }

        private Binding CreateBinding(OutComponent outComponent, string path)
        {
            return new Binding(path)
            {
                Source = outComponent,
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
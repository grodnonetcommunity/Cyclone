
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.Models.Operations;
using JetBrains.Annotations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class UIGenerator : IUIGenerator
	{
		private readonly IList<Execution> _executions;
		private readonly OperationTypeParser _operationTypeParser = new OperationTypeParser();

		public UIGenerator(Execution execution) : this(new List<Execution> {execution})
		{
			
		}

		public UIGenerator(IList<Execution> executions)
		{
			_executions = executions;
		}

		public IOutComponent GetOutputComponents()
		{
			Dictionary<int, UIElement> uiComponents = new Dictionary<int, UIElement>();
			var outComponent = new OutComponent(uiComponents);
			if (_executions == null)
			{
				return outComponent;
			}

			List<int> lines = new List<int>();
			lines.Sort();
			for (int i = 0; i < _executions.Count; i++)
			{
				var execution = _executions[i];
				RecursiveSearchLineNumbers(execution.Operations, lines);
				foreach (var line in lines)
				{
					var uiElement = GetLine(line, execution);
					if (uiElement == null) continue;
					if (!uiComponents.ContainsKey(line))
					{
						var panel = new StackPanel
						{
							Orientation = Orientation.Horizontal
						};
						panel.Children.Add(uiElement);
						uiComponents.Add(line, panel);
					}
					else
					{
						var panel = (Panel)uiComponents[line];
						IList<UIElement> replaced = new List<UIElement>();
						foreach (var child in panel.Children)
						{
							var element = child as StackPanel;
							if (element != null)
							{
								replaced.Add(element);
							}
						}
						foreach (var element in replaced)
						{
							panel.Children.Remove(element);
							var border = new Border
							{
								BorderThickness = new Thickness(0, 0, 1, 0),
								BorderBrush = new SolidColorBrush(Colors.Black),
								Child = element
							};
							panel.Children.Add(border);
						}
						panel.Children.Add(uiElement);
					}
				}
			}
			return outComponent;
		}

		private void RecursiveSearchLineNumbers(IEnumerable<Operation> operations,[NotNull] List<int> results)
		{
			foreach (var operation in operations)
			{
				if (!results.Contains(operation.LineNumber))
				{
					results.Add(operation.LineNumber);
				}

				var loopOperation = operation as LoopOperation;
				if (loopOperation != null)
				{
					foreach (var loopOperationItem in loopOperation.Operations)
					{
						RecursiveSearchLineNumbers(loopOperationItem.Value, results);
					}		
				}
			}
		} 

		public UIElement GetLine(int lineNumber, Execution execution)
		{
			var result = new StackPanel();

			List<Operation> foundOperations = new List<Operation>();
			SearchOperation(lineNumber, execution.Operations, foundOperations);

			//No component for this line
			if (foundOperations.Count < 1)
			{
				return null;
			}

			var searchResult = new OperationSearchResult(foundOperations);

			//Process loops first
			foreach (var loopOperationWithParent in searchResult.LoopOperations)
			{
				AppendTextBlockLine(result, _operationTypeParser.ProcessLoopOperation(
					loopOperationWithParent.Value, loopOperationWithParent.Key)); 
			}

			foreach (var assignOperation in searchResult.AssignOperations)
			{
				AppendTextBlockLine(result, _operationTypeParser.ProcessAssignOperation(
				assignOperation));
			}

			return result;
		}

		private void AppendTextBlockLine(Panel baseElement, IList<OutputItem> runs)
		{
			Grid grid = new Grid();

			grid.RowDefinitions.Add(new RowDefinition());
			foreach (var outputItem in runs)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition
				{
					SharedSizeGroup = outputItem.MeasureGroup.ToString(),
					Width = GridLength.Auto
				});
			}

			for (int i = 0; i < runs.Count; i++)
			{
				var run = runs[i];
				TextBlock textBlock = new TextBlock
				{
					HorizontalAlignment = HorizontalAlignment.Stretch,
					Text = run.MeasureGroup == MeasureGroup.VariableValuesCycle ? " " + run.Output + " " : run.Output,
					Foreground = new SolidColorBrush(run.OutputColor),
					FontFamily = new FontFamily("Consolas"),
					FontSize = 12
				};

				if (run.MeasureGroup == MeasureGroup.VariableValuesCycle)
				{
					textBlock.TextAlignment = TextAlignment.Center;
					Border border = new Border
					{
						HorizontalAlignment = HorizontalAlignment.Stretch,
						BorderThickness = new Thickness(1, 0, 1, 0),
						BorderBrush = new SolidColorBrush(Colors.Black)
					};
					border.Child = textBlock;
					grid.Children.Add(border);
					Grid.SetColumn(border, i);
				}
				else
				{
					grid.Children.Add(textBlock);
					Grid.SetColumn(textBlock, i);
				}
			}
			
			baseElement.Children.Add(grid);
		}

		public void SearchOperation(int lineNumber, 
			[NotNull]IList<Operation> operations, 
			[NotNull]IList<Operation> results)
		{
			foreach (var operation in operations)
			{
				if (operation.LineNumber == lineNumber)
				{
					results.Add(operation);
				}

				LoopOperation loopOperation = operation as LoopOperation;
				if (loopOperation != null)
				{
					foreach (var loopOperationItem in loopOperation.Operations)
					{
						foreach (var operationToPopulate in loopOperationItem.Value)
						{
							//TODO use different model here
							operationToPopulate.ParentOperation = loopOperation;
							operationToPopulate.IterationNumber = loopOperationItem.Key;
						}
						SearchOperation(lineNumber, loopOperationItem.Value, results);
					}
				}
			}
		}
	}
}
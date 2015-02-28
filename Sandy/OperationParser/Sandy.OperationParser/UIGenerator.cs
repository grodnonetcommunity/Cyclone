
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using AV.Cyclone.Sandy.Models;
using AV.Cyclone.Sandy.Models.Operations;
using JetBrains.Annotations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class UIGenerator
	{
		private readonly Execution _execution;
		private readonly OperationTypeParser _operationTypeParser = new OperationTypeParser();

		public UIGenerator(Execution execution)
		{
			_execution = execution;
		}

		public OutComponent GetOutputComponents(string fileName)
		{
			Dictionary<int, UIElement> uiComponents = new Dictionary<int, UIElement>();
			OutComponent outComponent = new OutComponent(uiComponents);
			List<int> lines = new List<int>();
			RecursiveSearchLineNumbers(_execution.Operations.Where(op => op.FileName == fileName), lines);
			foreach (var line in lines)
			{
				var uiElement = GetLine(line, fileName);
				uiComponents.Add(line, uiElement);
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

		public UIElement GetLine(int lineNumber, string fileName)
		{
			var result = new StackPanel();

			List<Operation> foundOperations = new List<Operation>();
			SearchOperation(lineNumber, _execution.Operations, foundOperations, fileName);

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
			StackPanel horizontalPanel = new StackPanel();
			horizontalPanel.Orientation = Orientation.Horizontal;
			foreach (var run in runs)
			{
				TextBlock textBlock = new TextBlock
				{
					Text = run.Output,
					Margin = new Thickness(run.MarginLeft, 0, 0, 0)
				};
				horizontalPanel.Children.Add(textBlock);
			}
			
			baseElement.Children.Add(horizontalPanel);
		}

		public void SearchOperation(int lineNumber, 
			[NotNull]IList<Operation> operations, 
			[NotNull]IList<Operation> results,[NotNull] string fileName)
		{
			foreach (var operation in operations
				.Where(o => o.FileName == fileName))
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
						SearchOperation(lineNumber, loopOperationItem.Value, results, fileName);
					}
				}
			}
		}
	}
}
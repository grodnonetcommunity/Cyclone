
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

		public IList<UIElement> GetOutputComponents(string fileName)
		{
			throw new NotImplementedException();
		} 

		public UIElement GetLine(int lineNumber, string fileName)
		{
			var result = new TextBlock();

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

		private void AppendTextBlockLine(TextBlock textBlock, IList<Run> runs)
		{
			if (textBlock.Inlines.Count > 0)
			{
				textBlock.Inlines.Add(new LineBreak());
			}
			textBlock.Inlines.AddRange(runs);
		}

		internal void SearchOperation(int lineNumber, 
			[NotNull]IList<Operation> operations, 
			[NotNull]IList<Operation> results,[NotNull] string fileName)
		{
			foreach (var operation in operations
				.Where(o => o.FileName.Equals(fileName)))
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
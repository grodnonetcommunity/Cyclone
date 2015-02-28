
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		private Execution _execution;
		private OperationTypeParser _operationTypeParser = new OperationTypeParser();

		public UIGenerator(Execution execution)
		{
			_execution = execution;
		}


		public UIElement GetLine(int line, string fileName)
		{
			UIElement result = new TextBlock();



			return result;
		}

		internal void SearchOperation(int lineNumber, 
			IList<Operation> operations, 
			[NotNull] IList<OperationParcerModel> results)
		{
			foreach (var operation in operations)
			{
				if (operation.LineNumber == lineNumber)
				{
					results.Add(new OperationParcerModel
					{
						Operation = operation
					});
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
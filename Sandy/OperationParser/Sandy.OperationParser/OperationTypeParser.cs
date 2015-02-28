using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OperationTypeParser
	{
		private const string EqualSign = " = ";
		private const string SpaceSign = " ";
		private const char OpenBracket = '[';
		private const char CloseBracket = ']';
		private const string Separator = ", ";
		private const string LoopSeparationToken = "|";

		public IList<OutputItem> ProcessLoopOperation<T>(IList<T> operations, LoopOperation parent) where T : Operation
		{
			var result = new List<OutputItem>();
			//TODO max width should also take ariable name into consideration
			double maxWidthValue = CalculateMaxWidth(parent.Operations.SelectMany(o => o.Value).ToList());
			double maxWidthName = CalculateMaxWidth(parent.Operations.SelectMany(o => o.Value).ToList(), false);
			//TODO: do it better
			var firstAssign = operations.FirstOrDefault() as AssignOperation;
			if (firstAssign != null)
			{
				var variableName = firstAssign.VariableName;
				var variableNameSize = MeasureString(variableName, new TextBlock());
				result.Add(new OutputItem(variableName, maxWidthName - variableNameSize.Width));
				result.Add(new OutputItem(EqualSign));
			}
			result.Add(new OutputItem(LoopSeparationToken));

			for (int i = 0; i < parent.GetTotalNumberOfIteration; i++)
			{
				//TODO for now only assign operations are supported
				string variableValueAsString = string.Empty;
				var singleAssignValue = operations.FirstOrDefault(v => v.IterationNumber == i) as AssignOperation;
				if (singleAssignValue != null)
				{
					variableValueAsString = GetVariableValueAsString(singleAssignValue.VariableValue);
				}

				var variableSize = MeasureString(variableValueAsString, new TextBlock());

				result.Add(new OutputItem(variableValueAsString, maxWidthValue - variableSize.Width));
				result.Add(new OutputItem(LoopSeparationToken));
			}
			return result;
		}

		public IList<OutputItem> ProcessAssignOperation(AssignOperation assignOperation)
		{
			var result = new List<OutputItem>
			{
				new OutputItem(assignOperation.VariableName),
				new OutputItem(EqualSign),
				new OutputItem(GetVariableValueAsString(assignOperation.VariableValue))
			};
			return result;
		}

		private string GetVariableValueAsString(object variableValue)
		{
			var valueList = variableValue as IList;
			if(valueList != null)
            {
				StringBuilder builder = new StringBuilder();
				builder.Append(OpenBracket);

	            for (int i = 0; i < valueList.Count; i++)
	            {
		            var valueObject = valueList[i];
					builder.Append(GetVariableValueAsString(valueObject));
		            if (i < valueList.Count - 1)
		            {
						builder.Append(Separator);
					}
				}

				builder.Append(CloseBracket);
				return builder.ToString();
			}

			if (variableValue != null)
			{
				return variableValue.ToString();
			}

			//TODO looks like this covers all potential stuff
			//TODO maybe exception should also go here

			return string.Empty;
		}

		private double CalculateMaxWidth(IList<Operation> loopList, bool useVariableValue = true)
		{
			double maxWidth = 0;
			foreach (var operation in loopList)
			{
				//TODO once again, only assign values here
				var assignOperation = operation as AssignOperation;
				if (assignOperation != null)
				{
					if (useVariableValue)
					{
						maxWidth = Math.Max(maxWidth,
							MeasureString(GetVariableValueAsString(assignOperation.VariableValue),
								new TextBlock()).Width);
					}
					else
					{
						maxWidth = Math.Max(maxWidth,
							MeasureString(GetVariableValueAsString(assignOperation.VariableName),
								new TextBlock()).Width);
					}
				}
			}
			return maxWidth;
		}

		private Size MeasureString(string candidate, TextBlock textBlock)
		{
			var formattedText = new FormattedText(
				candidate,
				CultureInfo.CurrentUICulture,
				FlowDirection.LeftToRight,
				new Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
				textBlock.FontSize,
				Brushes.Black);

			return new Size(formattedText.Width, formattedText.Height);
		}
	}
}
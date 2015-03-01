﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OperationTypeParser
	{
		private const string EqualSign = " = ";
		private const char OpenBracket = '[';
		private const char CloseBracket = ']';
		private const string Separator = ", ";

		private readonly Color VariableName = Colors.Black;
		private readonly Color VariableValueNumber = Colors.Black;
		private readonly Color VariableValueString = Colors.DarkOrange;
		private readonly Color VariableValueBoolean = Colors.DodgerBlue;


		public OperationTypeParser()
		{
		}

		public IList<OutputItem> ProcessLoopOperation<T>(IList<T> operations, LoopOperation parent) where T : Operation
		{
			var result = new List<OutputItem>();
			//TODO max width should also take ariable name into consideration
			//TODO: do it better
			var firstAssign = operations.FirstOrDefault() as AssignOperation;
			StringBuilder variableNamePart = new StringBuilder();
			if (firstAssign != null)
			{
				var variableName = firstAssign.VariableName;
				variableNamePart.Append(variableName);
				variableNamePart.Append(EqualSign);
			}
			//variableNamePart.Append(LoopSeparationToken);
			result.Add(new OutputItem(variableNamePart.ToString(), MeasureGroup.VariableNames, VariableName));

			for (int i = 0; i < parent.GetTotalNumberOfIteration; i++)
			{
				//TODO for now only assign operations are supported
				var singleAssignValue = operations.FirstOrDefault(v => v.IterationNumber == i) as AssignOperation;
				var variableValueAsString = GetVariableValueAsString(singleAssignValue?.VariableValue);

				result.Add(new OutputItem(
					variableValueAsString.Key, 
					MeasureGroup.VariableValuesCycle, variableValueAsString.Value));
			}
			return result;
		}

		public IList<OutputItem> ProcessAssignOperation(AssignOperation assignOperation)
		{
			var output = GetVariableValueAsString(assignOperation.VariableValue);
            var result = new List<OutputItem>
			{
				new OutputItem(
					assignOperation.VariableName + EqualSign, 
					MeasureGroup.VariableNames, VariableName),
				new OutputItem(output.Key, 
					MeasureGroup.VariableValues, output.Value)
			};
			return result;
		}

		private KeyValuePair<string, Color> GetVariableValueAsString(object variableValue)
		{
			var valueList = variableValue as IList;
			if(valueList != null)
            {
				var builder = new StringBuilder();
				builder.Append(OpenBracket);

	            object lastValue = null;
	            for (int i = 0; i < valueList.Count; i++)
	            {
					var valueObject = valueList[i];
		            lastValue = valueObject;
					builder.Append(GetVariableValueAsString(valueObject).Key);
		            if (i < valueList.Count - 1)
		            {
						builder.Append(Separator);
					}
				}

				builder.Append(CloseBracket);
	            if (IsNumber(lastValue))
	            {
		            return new KeyValuePair<string, Color>(builder.ToString(), VariableValueNumber);
	            }
				else if (lastValue is bool)
				{
					return new KeyValuePair<string, Color>(builder.ToString(), VariableValueBoolean);
				}
	            else
	            {
					return new KeyValuePair<string, Color>(builder.ToString(), VariableValueString);
				}
            }

			if (variableValue != null)
			{
				if (IsNumber(variableValue))
				{
					return new KeyValuePair<string, Color>(variableValue.ToString(), VariableValueNumber);
				}
				else if (variableValue is bool)
				{
					return new KeyValuePair<string, Color>(variableValue.ToString(), VariableValueBoolean);
				}
				else
				{
					return new KeyValuePair<string, Color>(variableValue.ToString(), VariableValueString);
				}
			}

			//TODO looks like this covers all potential stuff
			//TODO maybe exception should also go here

			return new KeyValuePair<string, Color>();
		}

		private static bool IsNumber(object value)
		{
			return value is sbyte
					|| value is byte
					|| value is short
					|| value is ushort
					|| value is int
					|| value is uint
					|| value is long
					|| value is ulong
					|| value is float
					|| value is double
					|| value is decimal;
		}
	}
}
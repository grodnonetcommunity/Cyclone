using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AV.Cyclone.Sandy.Models.Operations;

namespace AV.Cyclone.Sandy.OperationParser
{
	public class OperationTypeParser
	{
		private const string EqualSign = " = ";
		private const char OpenBracket = '[';
		private const char CloseBracket = ']';
		private const string Separator = ", ";

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
			result.Add(new OutputItem(variableNamePart.ToString(), MeasureGroup.VariableNames));

			for (int i = 0; i < parent.GetTotalNumberOfIteration; i++)
			{
				//TODO for now only assign operations are supported
				string variableValueAsString = string.Empty;
				var singleAssignValue = operations.FirstOrDefault(v => v.IterationNumber == i) as AssignOperation;
				if (singleAssignValue != null)
				{
					variableValueAsString = GetVariableValueAsString(singleAssignValue.VariableValue);
				}

				result.Add(new OutputItem(
					variableValueAsString, 
					MeasureGroup.VariableValuesCycle));
			}
			return result;
		}

		public IList<OutputItem> ProcessAssignOperation(AssignOperation assignOperation)
		{
			var result = new List<OutputItem>
			{
				new OutputItem(
					assignOperation.VariableName + EqualSign, 
					MeasureGroup.VariableNames),
				new OutputItem(
					GetVariableValueAsString(assignOperation.VariableValue), 
					MeasureGroup.VariableValues)
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
	}
}
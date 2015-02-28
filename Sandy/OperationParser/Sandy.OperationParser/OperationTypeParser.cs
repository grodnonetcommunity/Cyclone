using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
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

		public IList<Run> ProcessLoopOperation<T>(IList<T> operations, LoopOperation parent) where T : Operation
		{
			var result = new List<Run>();
			int maxWidth = CalculateMaxWidth(parent.Operations.SelectMany(o => o.Value).ToList());

			for (int i = 0; i < parent.GetTotalNumberOfIteration; i++)
			{
				//TODO for now only assign operations are supported
				StringBuilder variableValue = new StringBuilder();
				int variableValueLength = 0;
				var singleAssignValue = operations.FirstOrDefault(v => v.IterationNumber == i) as AssignOperation;
				if (singleAssignValue != null)
				{
					string variableValueAsString = GetVariableValueAsString(singleAssignValue.VariableValue);
					variableValue.Append(variableValueAsString);
					variableValueLength = variableValueAsString.Length;
				}
				for (int j = variableValueLength; j <= maxWidth; j++)
				{
					variableValue.Append(SpaceSign);
				}

				result.Add(new Run(variableValue.ToString()));
				result.Add(new Run(LoopSeparationToken));
			}
			return result;
		}

		public IList<Run> ProcessAssignOperation(AssignOperation assignOperation)
		{
			var result = new List<Run>
			{
				new Run(assignOperation.VariableName),
				new Run(EqualSign),
				new Run(GetVariableValueAsString(assignOperation.VariableValue))
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
			var value = variableValue as IFormattable;
			if (value != null)
			{
				return value.ToString();
			}

			//TODO looks like this covers all potential stuff
			//TODO maybe exception should also go here

			return string.Empty;
		}

		private int CalculateMaxWidth(IList<Operation> loopList)
		{
			int maxWidth = 0;
			foreach (var operation in loopList)
			{
				//TODO once again, only assign values here
				var assignOperation = operation as AssignOperation;
				if (assignOperation != null)
				{
					maxWidth = Math.Max(maxWidth,
						GetVariableValueAsString(assignOperation.VariableValue).Length);
				}
			}
			return maxWidth;
		}
	}
}
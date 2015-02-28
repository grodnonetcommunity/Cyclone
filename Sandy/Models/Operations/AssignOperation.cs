namespace AV.Cyclone.Sandy.Models.Operations
{
	public class AssignOperation : Operation
	{
		public string VariableName { get; set; }
		public object VariableValue { get; set; }
	}
}
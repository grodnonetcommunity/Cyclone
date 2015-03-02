namespace AV.Cyclone.Sandy.Models.Operations
{
	public abstract class Operation
	{
		public string FileName { get; set; }
		public int LineNumber { get; set; }

		#region TOREFACTOR
		//TODO This must be removed in future
		public Operation ParentOperation { get; set; }
		public int IterationNumber { get; set; }
		#endregion
	}
}
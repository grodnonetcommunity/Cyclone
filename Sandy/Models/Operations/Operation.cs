namespace AV.Cyclone.Sandy.Models.Operations
{
	public abstract class Operation
	{
		public string FileName { get; set; }
		public int LineNumber { get; set; }
	}
}
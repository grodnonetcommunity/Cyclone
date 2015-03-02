namespace AV.Cyclone.Katrina.Executor
{
    public class MethodOperationsBuilder : OperationBuilder
    {
        public MethodOperationsBuilder(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }
    }
}
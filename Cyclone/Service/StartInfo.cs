namespace AV.Cyclone.Service
{
    public class StartInfo
    {
        public string SolutionPath { get; set; }
        public string ActiveDocumentPath { get; set; }
        public int InitialLineNumber { get; set; }
        public int InitialColumnNumber { get; set; }
    }
}
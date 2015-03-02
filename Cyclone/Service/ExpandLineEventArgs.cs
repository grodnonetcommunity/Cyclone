namespace AV.Cyclone.Service
{
    public class ExpandLineEventArgs : CycloneEventArgs
    {
        public ExpandLineEventArgs() : base(CycloneEventsType.ExpandLines)
        {
        }

        public ExpandLineInfo ExpandLineInfo { get; set; }
    }

    public class ExpandLineInfo
    {
        public int LineNumber { get; set; }
        public double PreferedSize { get; set; }
    }
}
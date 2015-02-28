namespace AV.Cyclone.Service
{
    public class StartCycloneEventArgs : CycloneEventArgs
    {
        public StartCycloneEventArgs(StartInfo startInfoData) : base(CycloneEventsType.Start)
        {
            StartInfo = startInfoData;
        }

        public StartInfo StartInfo { get; }
    }
}
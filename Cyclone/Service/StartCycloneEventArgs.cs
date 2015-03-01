namespace AV.Cyclone.Service
{
    public class StartCycloneEventArgs : CycloneEventArgs
    {
        public StartCycloneEventArgs() : base(CycloneEventsType.Start)
        {
        }
    }
}
using System;

namespace AV.Cyclone.Service
{
    public class CycloneEventArgs : EventArgs
    {
        public readonly CycloneEventsType EventType;

        public CycloneEventArgs(CycloneEventsType eventsType)
        {
            EventType = eventsType;
        }
    }
}
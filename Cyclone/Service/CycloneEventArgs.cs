using System;

namespace AV.Cyclone.Service
{
    public class CycloneEventArgs : EventArgs
    {
        public readonly CycloneEventsType EventType;

        protected CycloneEventArgs(CycloneEventsType eventsType)
        {
            EventType = eventsType;
        }
    }
}
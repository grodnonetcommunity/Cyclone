using System;

namespace AV.Cyclone.Service
{
    public class CycloneService : ICycloneService
    {
        public event EventHandler<CycloneEventArgs> CycloneChanged;

        public void StartCyclone()
        {
            OnCycloneChanged(new StartCycloneEventArgs());
        }

        public void ExpandLine(int lineNumber, double preferedSize)
        {
            /*OnCycloneChanged(new ExpandLineEventArgs
            {
                ExpandLineInfo = new ExpandLineInfo
                {
                    LineNumber = lineNumber,
                    PreferedSize = preferedSize
                }
            });*/
        }

        protected virtual void OnCycloneChanged(CycloneEventArgs e)
        {
            var cycloneChanged = CycloneChanged;
            if (cycloneChanged != null)
                cycloneChanged.Invoke(this, e);
        }
    }
}
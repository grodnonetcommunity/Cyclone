using System;

namespace AV.Cyclone.Service
{
    public sealed class CycloneService : ICycloneService
    {
        private static readonly Lazy<CycloneService> lazy =
            new Lazy<CycloneService>(() => new CycloneService());

        private CycloneService()
        {
        }

        public static CycloneService Instance
        {
            get { return lazy.Value; }
        }

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

        private void OnCycloneChanged(CycloneEventArgs e)
        {
            CycloneChanged?.Invoke(this, e);
        }
    }
}
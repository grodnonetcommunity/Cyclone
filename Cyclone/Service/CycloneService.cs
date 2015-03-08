using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace AV.Cyclone.Service
{
    [Export(typeof(ICycloneService))]
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

        public void ExpandLine(List<ExpandLineInfo> list)
        {
            OnCycloneChanged(new ExpandLineEventArgs
            {
                ExpandLineInfos = list
            });
        }

        private void OnCycloneChanged(CycloneEventArgs e)
        {
            var cycloneChanged = CycloneChanged;
            if (cycloneChanged != null)
                cycloneChanged.Invoke(this, e);
        }
    }
}
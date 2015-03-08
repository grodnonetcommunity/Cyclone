using System;
using System.Collections.Generic;

namespace AV.Cyclone.Service
{
    public interface ICycloneService
    {
        event EventHandler<CycloneEventArgs> CycloneChanged;

        void StartCyclone();
    }
}
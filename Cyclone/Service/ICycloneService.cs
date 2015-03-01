using System;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AV.Cyclone.Service
{
    public interface ICycloneService
    {
        event EventHandler<CycloneEventArgs> CycloneChanged;
        void StartCyclone();
        void ExpandLine(int lineNumber, double preferedSize);
    }
}
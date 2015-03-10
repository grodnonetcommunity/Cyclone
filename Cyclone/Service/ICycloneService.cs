using System;
using System.Collections.Generic;
using AV.Cyclone.Sandy.Models;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Service
{
    public interface ICycloneService
    {
        event EventHandler<CycloneEventArgs> CycloneChanged;

        void StartCyclone();

        ICloudCollection GetClouds(IWpfTextView textView);
    }
}
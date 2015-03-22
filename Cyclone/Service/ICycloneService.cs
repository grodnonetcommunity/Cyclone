using System;
using AV.Cyclone.Sandy.Models;
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Service
{
    public interface ICycloneService
    {
        event EventHandler Changed;

        void StartCyclone(string solutionPath, string projectName, string filePath, int lineNumber);

        void StopCyclone();

        ICloudCollection GetClouds(IWpfTextView textView);

        void UpdateFile(ITextView textView, string content);
    }
}
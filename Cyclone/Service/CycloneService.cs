using System;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AV.Cyclone.Service
{
    public class CycloneService : ICycloneService
    {
        public event EventHandler<CycloneEventArgs> CycloneChanged;

        protected virtual void OnCycloneChanged(CycloneEventArgs e)
        {
            CycloneChanged?.Invoke(this, e);
        }

        public void StartCyclone(IVsTextView vTextView)
        {
            int initialLineNumber;
            int initialColumnNumber;
            vTextView.GetCaretPos(out initialLineNumber, out initialColumnNumber);
            var soultionFileName = ExamplesPackage.Dte.Solution.FileName;

            var fullName = ExamplesPackage.Dte.ActiveDocument.FullName;

            OnCycloneChanged(new StartCycloneEventArgs(new StartInfo
            {
                ActiveDocumentPath = fullName,
                SolutionPath = soultionFileName,
                InitialColumnNumber = initialColumnNumber,
                InitialLineNumber = initialLineNumber
            }));
        }

        public void ExpandLine(int lineNumber, double preferedSize)
        {
            OnCycloneChanged(new ExpandLineEventArgs
            {
                ExpandLineInfo = new ExpandLineInfo
                {
                    LineNumber = lineNumber,
                    PreferedSize = preferedSize
                }
            });
        }
    }
}
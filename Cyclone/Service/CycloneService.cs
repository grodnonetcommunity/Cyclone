using System;
using EnvDTE;
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
            var solutionPath = ExamplesPackage.Dte.Solution.FileName;

            var activeDocument = ExamplesPackage.Dte.ActiveDocument;
            var activeDocumentPath = activeDocument.FullName;
            var currentProjectName = activeDocument.ProjectItem.ContainingProject.Name;
            OnCycloneChanged(new StartCycloneEventArgs(new StartInfo
            {
                ActiveDocumentPath = activeDocumentPath,
                SolutionPath = solutionPath,
                ProjectName = currentProjectName,
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
using Microsoft.VisualStudio.Text.Editor;

namespace AV.Cyclone.Service
{
    public class CycloneServiceProvider
    {
        public static ICycloneService GetCycloneService(IWpfTextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty
                (() => new CycloneService());
        }
    }
}
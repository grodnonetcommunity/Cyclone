﻿using System.ComponentModel.Composition;
using System.Diagnostics;
using AV.Cyclone.Service;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace AV.Cyclone.EmtyLines
{
    [Export(typeof (IViewTaggerProvider))]
    [ContentType("CSharp")]
    [TagType(typeof (ErrorTag))]
    internal class ErrorTaggerProvider : IViewTaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            if (textView == null)
            {
                return null;
            }

            if (textView.TextBuffer != buffer)
            {
                return null;
            }

            Trace.Assert(textView is IWpfTextView);

            var imageAdornmentManager =
                textView.Properties.GetOrCreateSingletonProperty(
                    () =>
                        new EmptyLineAdornmentManager((IWpfTextView) textView,
                            CycloneServiceProvider.GetCycloneService()));
            return imageAdornmentManager as ITagger<T>;
        }
    }
}
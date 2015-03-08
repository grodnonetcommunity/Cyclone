using System;
using System.Collections.Generic;

namespace Cyclon.Controls
{
    public interface ITextViewService
    {
        event EventHandler LayoutChanged;

        double Scale { get; }

        int FirstVisibleLineNumber { get; }

        int LastVisibleLineNumber { get; }

        IEnumerable<LineInfo> GetVisibleLines();
    }
}
using System.Collections.Generic;
using Verse;

namespace Radiology;

internal interface IDefHyperlinkLister
{
    IEnumerable<DefHyperlink> hyperlinks();
}
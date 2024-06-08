using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

[HarmonyPatch(typeof(StatsReportUtility), "DescriptionEntry", typeof(Thing))]
internal class StatsReportUtility_DescriptionEntry
{
    private static StatDrawEntry Postfix(StatDrawEntry entry, Thing thing)
    {
        if (thing is not ThingWithComps t)
        {
            return entry;
        }

        var listers = t.Comps().OfType<IDefHyperlinkLister>();
        if (!listers.Any())
        {
            return entry;
        }

        Traverse.Create(entry).Field<IEnumerable<Dialog_InfoCard.Hyperlink>>("hyperlinks").Value =
            Dialog_InfoCard.DefsToHyperlinks(listLinks(thing, listers));

        return entry;
    }

    private static IEnumerable<DefHyperlink> listLinks(Thing thing, IEnumerable<IDefHyperlinkLister> listers)
    {
        if (thing.def.descriptionHyperlinks != null)
        {
            foreach (var l in thing.def.descriptionHyperlinks)
            {
                yield return l;
            }
        }

        foreach (var lister in listers)
        foreach (var l in lister.hyperlinks())
        {
            yield return l;
        }
    }
}
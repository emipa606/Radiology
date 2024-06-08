using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology;

public class CompHediffStorage : ThingComp, IDefHyperlinkLister
{
    public List<Hediff> hediffs = [];
    private BodyPartDef part;
    public List<BodyPartDef> parts = [];

    public IEnumerable<DefHyperlink> hyperlinks()
    {
        foreach (var hediff in hediffs)
        {
            if (!hediff.Visible)
            {
                continue;
            }

            yield return hediff.def;
        }
    }

    public override void PostExposeData()
    {
        Scribe_Collections.Look(ref hediffs, "hediffs", LookMode.Deep);
        Scribe_Collections.Look(ref parts, "parts", LookMode.Def);
    }

    public override string CompInspectStringExtra()
    {
        var builder = new StringBuilder();

        for (var i = 0; i < hediffs.Count; i++)
        {
            var hediff = hediffs[i];
            if (!hediff.Visible)
            {
                continue;
            }

            if (builder.Length != 0)
            {
                builder.Append("\n");
            }

            if (part != parts[i])
            {
                builder.Append(parts[i].LabelCap + ": ");
            }

            builder.Append(hediffs[i].def.LabelCap);
        }

        return builder.ToString();
    }

    public override void PostPostMake()
    {
        if (!Radiology.itemBodyParts.TryGetValue(parent.def, out part))
        {
            return;
        }

        // add a random mutation; if this item is spawned via operation, the mutation will be removed anyway
        var mutationDef = DefDatabase<MutationDef>.AllDefs
            .Where(x => x.affectedParts != null && x.affectedParts.Contains(part)).RandomElementWithFallback();

        var hediff = (Hediff)Activator.CreateInstance(mutationDef.hediffClass);
        hediff.def = mutationDef;
        hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
        hediff.PostMake();

        hediffs.Add(hediff);
        parts.Add(part);
    }
}
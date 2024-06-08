using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Radiology;
// death

public class CancerCompSpreads : CancerComp<CancerCompSpreadsDef>
{
    public override bool IsValid()
    {
        var growUpwards = def.growUpwards;
        var growDownwards = def.growDownwards;

        if (cancer.Part.parent == null)
        {
            growUpwards = false;
        }

        if (!cancer.Part.GetDirectChildParts().Any())
        {
            growDownwards = false;
        }

        return growUpwards || growDownwards;
    }

    public IEnumerable<BodyPartRecord> PotentialTargetParts()
    {
        if (cancer.Part.parent != null && def.growUpwards)
        {
            yield return cancer.Part.parent;
        }

        foreach (var childPart in cancer.Part.GetDirectChildParts())
        {
            yield return childPart;
        }
    }


    public IEnumerable<BodyPartRecord> TargetParts()
    {
        foreach (var part in PotentialTargetParts())
        {
            if (part == null)
            {
                continue;
            }

            var otherCancer = Cancer.GetCancerAt(cancer.pawn, part);
            if (otherCancer == null)
            {
                yield return part;
            }
        }
    }

    public override void Update(int passed)
    {
        if (passed < Rand.Range(0, (int)(def.mtthDays * 60000)))
        {
            return;
        }

        var part = TargetParts().RandomElementWithFallback();
        if (part == null)
        {
            return;
        }

        if (HediffMaker.MakeHediff(cancer.def, cancer.pawn, part) is not Cancer newCancer)
        {
            return;
        }

        if (!def.mutates)
        {
            newCancer.BecomeCopyOf(cancer);
        }

        cancer.pawn.health.AddHediff(newCancer, part);
    }
}
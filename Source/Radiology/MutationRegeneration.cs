using System.Linq;
using Verse;

namespace Radiology;

public class MutationRegeneration : Mutation<MutationRegenerationDef>
{
    private static readonly HediffDef Shredded = HediffDef.Named("Shredded");

    public bool RegenerateInjury()
    {
        var permanentInjuries = pawn.health.hediffSet.hediffs.OfType<Hediff_Injury>()
            .Where(x => x.IsPermanent() && x.Part != null);
        var injury = permanentInjuries.RandomElementWithFallback();

        var hediffComp_GetsPermanent = injury?.TryGetComp<HediffComp_GetsPermanent>();
        if (hediffComp_GetsPermanent == null)
        {
            return false;
        }

        hediffComp_GetsPermanent.IsPermanent = false;
        injury.Severity = injury.Part.def.hitPoints - 1;
        pawn.health.hediffSet.DirtyCache();

        RadiologyEffectSpawnerDef.Spawn(def.effectRegeneration, pawn);
        return true;
    }

    public void RegenerateBodyPart()
    {
        var injured = pawn.health.hediffSet.GetInjuredParts();
        var missing = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
        var potential = missing.Where(x =>
            x.Part != null && (
                x.Part.parent == null || !injured.Contains(x.Part.parent) &&
                !injured.Intersect(x.Part.parent.GetDirectChildParts()).Any()
            )
        );

        var hediff = potential.RandomElementWithFallback();
        if (hediff == null)
        {
            return;
        }

        var part = hediff.Part;
        pawn.health.hediffSet.hediffs.Remove(hediff);

        foreach (var subpart in part.GetDirectChildParts())
        {
            var missingPart =
                HediffMaker.MakeHediff(RimWorld.HediffDefOf.MissingBodyPart, pawn, subpart) as Hediff_MissingPart;
            pawn.health.hediffSet.hediffs.Add(missingPart);
        }

        if (HediffMaker.MakeHediff(Shredded, pawn, part) is Hediff_Injury injury)
        {
            injury.Severity = part.def.hitPoints - 1;
            pawn.health.hediffSet.hediffs.Add(injury);
        }

        pawn.health.hediffSet.DirtyCache();

        RadiologyEffectSpawnerDef.Spawn(def.effectRegeneration, pawn);
    }

    public override void Tick()
    {
        if (!pawn.IsHashIntervalTick(def.periodTicks))
        {
            return;
        }

        if (RegenerateInjury())
        {
            return;
        }

        if (def.healMissingParts)
        {
            RegenerateBodyPart();
        }
    }
}
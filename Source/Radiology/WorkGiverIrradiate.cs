using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Radiology;

public class WorkGiverIrradiate : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.RadiologyRadiationChamber);

    public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.RadiologyRadiationChamber);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        if (forced)
        {
            return false;
        }

        foreach (var chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
        {
            if (chamber.CanIrradiateNow(pawn) == null)
            {
                return false;
            }
        }

        return true;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t.Faction != pawn.Faction)
        {
            return false;
        }

        if (t is not Chamber chamber)
        {
            return false;
        }

        var reason = chamber.CanIrradiateNow(pawn);
        if (reason != null)
        {
            JobFailReason.Is(string.Format(reason.Translate(), pawn.LabelShortCap));
            return false;
        }

        if (chamber.IsForbidden(pawn))
        {
            return false;
        }

        LocalTargetInfo target = chamber;
        return pawn.CanReserve(target, 1, -1, null, forced);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        return new Job(JobDefOf.RadiologyIrradiate, t, 1500, true);
    }
}
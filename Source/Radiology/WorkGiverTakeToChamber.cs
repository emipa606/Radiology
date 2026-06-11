using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace Radiology;

public class WorkGiverTakeToChamber : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

    public override PathEndMode PathEndMode => PathEndMode.OnCell;

    public override Danger MaxPathDanger(Pawn pawn)
    {
        return Danger.Deadly;
    }

    public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
    {
        return pawn.Map.mapPawns.AllPawnsSpawned.Where(p => p.Downed || p.IsPrisoner);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        if (forced)
        {
            return false;
        }

        foreach (var chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
        {
            foreach (var target in pawn.Map.mapPawns.AllPawnsSpawned)
            {
                if (!target.Downed || !chamber.IsSelected(target))
                {
                    continue;
                }

                return false;
            }
        }

        return true;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (t is not Pawn target)
        {
            return false;
        }

        // Only work on downed pawns or incapacitated prisoners
        if (!target.Downed)
        {
            return false;
        }

        // Don't take pawns who are already being carried
        if (target.CurJobDef == JobDefOf.RadiologyTakeToChamber)
        {
            return false;
        }

        // Don't interfere with hostile pawns
        if (target.HostileTo(pawn))
        {
            return false;
        }

        // Prisoners can only be handled by wardens
        if (target.IsPrisoner && !pawn.workSettings.WorkIsActive(WorkTypeDefOf.Warden))
        {
            return false;
        }

        // Check if target is assigned to any chamber
        Chamber assignedChamber = null;
        foreach (var chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
        {
            if (!chamber.IsSelected(target))
            {
                continue;
            }

            var reason = chamber.CanIrradiateNow(target);
            if (reason != null && reason != "ChamberPawnIsHurt")
            {
                continue;
            }

            assignedChamber = chamber;
            break;
        }

        if (assignedChamber == null)
        {
            return false;
        }

        // Already in place for chamber irradiation, do not keep re-carrying.
        if (target.Position == assignedChamber.InteractionCell)
        {
            return false;
        }

        // Check if pawn can reserve both the target and the chamber
        if (!pawn.CanReserve(target, 1, -1, null, forced))
        {
            return false;
        }

        if (!pawn.CanReserve(assignedChamber, 1, -1, null, forced))
        {
            return false;
        }

        return pawn.CanReach(target, PathEndMode.OnCell, Danger.Deadly) &&
               pawn.CanReach(assignedChamber, PathEndMode.InteractionCell, Danger.Deadly);
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        var target = (Pawn)t;

        // Find the chamber this pawn is assigned to
        Chamber assignedChamber = null;
        foreach (var chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
        {
            if (!chamber.IsSelected(target))
            {
                continue;
            }

            var reason = chamber.CanIrradiateNow(target);
            if (reason != null && reason != "ChamberPawnIsHurt")
            {
                continue;
            }

            assignedChamber = chamber;
            break;
        }

        if (assignedChamber == null)
        {
            return null;
        }

        if (target.Position == assignedChamber.InteractionCell)
        {
            return null;
        }

        var job = JobMaker.MakeJob(JobDefOf.RadiologyTakeToChamber, target, assignedChamber);
        job.count = 1;
        return job;
    }
}
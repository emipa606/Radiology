using RimWorld;
using Verse;
using Verse.AI;

namespace Radiology;

public class WorkGiverIrradiatePrisoner : WorkGiver_Warden
{
    private static readonly string reasonSleeping = "ChamberSleeping";
    private static readonly string reasonDowned = "ChamberDowned";
    private static readonly string reasonPrisonerInaccessible = "ChamberPrisonerInaccessible";

    private bool CheckJob(Pawn pawn, Thing t, bool forced, out Chamber chamberRef, ref string reason)
    {
        var prisoner = t as Pawn;
        chamberRef = null;

        if (prisoner is { IsPrisoner: false })
        {
            return false;
        }

        if (!prisoner.Awake())
        {
            reason = reasonSleeping;
            return false;
        }

        if (prisoner is { Downed: true })
        {
            reason = reasonDowned;
            return false;
        }

        if (!ShouldTakeCareOfPrisoner(pawn, prisoner))
        {
            reason = reasonPrisonerInaccessible;
            return false;
        }

        if (prisoner?.CurJob?.GetCachedDriver(prisoner) is IPrisonerAllowedJob)
        {
            return false;
        }

        foreach (var chamber in pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Chamber>())
        {
            var currentReason = chamber.CanIrradiateNow(prisoner);
            if (currentReason == null && pawn.CanReserve(chamber, 1, -1, null, forced))
            {
                chamberRef = chamber;
                return true;
            }

            reason ??= currentReason;
        }

        return false;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        string reason = null;

        var res = CheckJob(pawn, t, forced, out _, ref reason);
        if (!res && reason != null)
        {
            JobFailReason.Is(string.Format(reason.Translate(), t.LabelShortCap));
        }

        return res;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        string reason = null;
        return !CheckJob(pawn, t, forced, out var chamber, ref reason)
            ? null
            : new Job(JobDefOf.RadiologyIrradiatePrisoner, t, chamber) { count = 1 };
    }
}
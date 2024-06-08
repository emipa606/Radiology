using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Radiology.Patch;

/// <summary>
///     Prevent pawns from equipping something if they have mutated body part apparel
/// </summary>
[HarmonyPatch(typeof(WorkGiver_Warden_TakeToBed), nameof(WorkGiver_Warden_TakeToBed.JobOnThing), typeof(Pawn),
    typeof(Thing), typeof(bool))]
public static class WorkGiver_Warden_TakeToBed_JobOnThing
{
    private static bool Prefix(Thing t, bool forced, ref Job __result)
    {
        if (t is not Pawn prisoner)
        {
            return true;
        }

        if (forced)
        {
            return true;
        }

        if (prisoner.CurJob?.GetCachedDriver(prisoner) is not IPrisonerAllowedJob)
        {
            return true;
        }

        __result = null;
        return false;
    }
}
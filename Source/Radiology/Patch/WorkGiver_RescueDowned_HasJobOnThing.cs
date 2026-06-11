using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

[HarmonyPatch(typeof(WorkGiver_RescueDowned), nameof(WorkGiver_RescueDowned.HasJobOnThing), typeof(Pawn),
    typeof(Thing), typeof(bool))]
public static class WorkGiver_RescueDowned_HasJobOnThing
{
    private static bool Prefix(Thing t, ref bool __result)
    {
        if (t is not Pawn downedPawn || !downedPawn.Downed)
        {
            return true;
        }

        foreach (var chamber in downedPawn.MapHeld?.listerBuildings.AllBuildingsColonistOfClass<Chamber>() ?? [])
        {
            if (!chamber.IsSelected(downedPawn) || downedPawn.Position != chamber.InteractionCell)
            {
                continue;
            }

            __result = false;
            return false;
        }

        return true;
    }
}

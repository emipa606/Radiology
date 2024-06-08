using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

/// <summary>
///     Forbid dropping of mutation apparel items.
/// </summary>
[HarmonyPatch(typeof(Pawn_ApparelTracker), nameof(Pawn_ApparelTracker.TryDrop),
    [typeof(Apparel), typeof(Apparel), typeof(IntVec3), typeof(bool)],
    [ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal])]
public static class Pawn_ApparelTracker_TryDrop
{
    private static bool Prefix(ref Apparel ap, out Apparel resultingAp, ref bool __result,
        Pawn_ApparelTracker __instance)
    {
        resultingAp = null;

        if (ap is not ApparelBodyPart)
        {
            return true;
        }

        var pawn = __instance.pawn;
        //var currentOutfit = pawn.outfits.CurrentOutfit;
        var currentOutfit = pawn.outfits.CurrentApparelPolicy;
        currentOutfit.filter.SetAllow(ap.def, true);

        __result = false;
        return false;
    }
}
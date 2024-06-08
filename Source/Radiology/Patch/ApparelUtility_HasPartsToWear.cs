using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

/// <summary>
///     Prevent pawns from equipping something if they have mutated body part apparel
/// </summary>
[HarmonyPatch(typeof(ApparelUtility), nameof(ApparelUtility.HasPartsToWear), typeof(Pawn), typeof(ThingDef))]
public static class ApparelUtility_HasPartsToWear
{
    private static bool Prefix(Pawn p, ThingDef apparel, ref bool __result)
    {
        if (apparel.thingClass.IsAssignableFrom(typeof(ApparelBodyPart)))
        {
            return true;
        }

        foreach (var apparelMutation in p.apparel.WornApparel.OfType<ApparelBodyPart>())
        {
            if (!apparel.apparel.bodyPartGroups.Intersect(apparelMutation.def.apparel.bodyPartGroups).Any())
            {
                continue;
            }

            __result = false;
            return false;
        }

        return true;
    }
}
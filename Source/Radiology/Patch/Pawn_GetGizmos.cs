using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace Radiology.Patch;

/// <summary>
///     Allows hediffs to add gizmos (should really be removed and re-implemented using existing comps mechanism)
/// </summary>
[HarmonyPatch(typeof(Pawn), nameof(Pawn.GetGizmos), [])]
public static class Pawn_GetGizmos
{
    private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> list, Pawn __instance)
    {
        foreach (var v in list)
        {
            yield return v;
        }

        foreach (var mutation in __instance.health.hediffSet.hediffs.Where(hediff => hediff is Mutation))
        {
            foreach (var gizmo in mutation.GetGizmos())
            {
                yield return gizmo;
            }
        }
    }
}
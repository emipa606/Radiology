using System.Linq;
using HarmonyLib;
using Verse;

namespace Radiology.Patch;

/// <summary>
///     Call PostLoad for mutations that need it.
/// </summary>
[HarmonyPatch(typeof(HediffSet), nameof(HediffSet.ExposeData), [])]
public static class HediffSet_ExposeData
{
    private static void Postfix(HediffSet __instance)
    {
        if (Scribe.mode != LoadSaveMode.ResolvingCrossRefs)
        {
            return;
        }

        foreach (var m in __instance.hediffs.OfType<Mutation>())
        {
            m.PostLoad();
        }
    }
}
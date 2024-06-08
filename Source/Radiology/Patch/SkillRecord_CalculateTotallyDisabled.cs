using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

/// <summary>
///     hook to check if a mutation disables a skill
/// </summary>
[HarmonyPatch(typeof(SkillRecord), "CalculateTotallyDisabled", [])]
public static class SkillRecord_CalculateTotallyDisabled
{
    private static void Postfix(ref bool __result, SkillRecord __instance, Pawn ___pawn)
    {
        foreach (var possibleMutation in ___pawn.health.hediffSet.hediffs.Where(hediff => hediff is MutationSetSkill))
        {
            var mutation = (MutationSetSkill)possibleMutation;
            __result = mutation.IsSkillDisabled(__instance.def);
            return;
        }
    }
}
using HarmonyLib;
using Verse;

namespace Radiology.Patch;

[HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.PostApplyDamage))]
internal class Pawn_HealthTracker_PostApplyDamage
{
    private static void Postfix(Pawn_HealthTracker __instance, DamageInfo dinfo, float totalDamageDealt)
    {
        if (dinfo.WeaponLinkedHediff is not MutationBloodHealerDef def)
        {
            return;
        }

        if (dinfo.Instigator is not Pawn pawn)
        {
            return;
        }

        var target = __instance.hediffSet.pawn;
        if (target?.RaceProps is not { IsFlesh: true })
        {
            return;
        }

        def.Heal(pawn, target, dinfo, totalDamageDealt);
    }
}
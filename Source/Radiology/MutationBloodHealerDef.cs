using System.Linq;
using UnityEngine;
using Verse;

namespace Radiology;

public class MutationBloodHealerDef : MutationDef
{
    public RadiologyEffectSpawnerDef effectRegeneration;
    public bool healMissingParts = false;

    public void Heal(Pawn pawn, Pawn target, DamageInfo dinfo, float totalDamageDealt)
    {
        RegenerateInjury(pawn, totalDamageDealt * 0.25f);
    }

    public void RegenerateInjury(Pawn pawn, float amount)
    {
        var injuries = pawn.health.hediffSet.hediffs.OfType<Hediff_Injury>()
            .Where(x => x.Severity > 0 && x.Part != null);
        var injury = injuries.RandomElementWithFallback();
        if (injury == null)
        {
            return;
        }

        var hediffComp_GetsPermanent = injury.TryGetComp<HediffComp_GetsPermanent>();
        if (hediffComp_GetsPermanent != null)
        {
            hediffComp_GetsPermanent.IsPermanent = false;
        }

        injury.Severity = Mathf.Max(injury.Severity - amount);
        pawn.health.hediffSet.DirtyCache();

        RadiologyEffectSpawnerDef.Spawn(effectRegeneration, pawn);
    }
}
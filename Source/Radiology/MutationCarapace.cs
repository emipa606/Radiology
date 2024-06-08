using UnityEngine;
using Verse;

namespace Radiology;

public class MutationCarapace : Mutation<MutationCarapaceDef>
{
    protected override ThingComp[] GetComps()
    {
        return [new CompCarapace { mutation = this }];
    }

    public void ApplyDamage(DamageInfo dinfo, out bool absorbed)
    {
        absorbed = false;

        if (Mathf.Abs(MathHelper.AngleDifference(dinfo.Angle + 180, pawn.Rotation.AsAngle)) < 45)
        {
            return;
        }

        RadiologyEffectSpawnerDef.Spawn(def.effectReflect, pawn, dinfo.Angle + 180);
        dinfo.SetAmount(dinfo.Amount * def.ratio);
    }
}
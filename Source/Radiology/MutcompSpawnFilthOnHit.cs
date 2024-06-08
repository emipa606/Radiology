using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

public class MutcompSpawnFilthOnHit : Mutcomp<MutcompSpawnFilthOnHitProps>
{
    public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
    {
        var count = Mathf.RoundToInt(totalDamageDealt * props.countPerDamage);
        FilthMaker.TryMakeFilth(Pawn.Position, Pawn.Map, props.filth, Pawn.LabelIndefinite(), count,
            FilthSourceFlags.Natural);
    }
}
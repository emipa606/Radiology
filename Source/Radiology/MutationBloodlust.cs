using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

internal class MutationBloodlust : MutationCapacityModifier<MutationBloodlustDef>
{
    protected override float Multiplier()
    {
        var position = pawn.Position;

        var amount = pawn.Map.listerThings.ThingsOfDef(RimWorld.ThingDefOf.Filth_Blood)
            .Where(x => x.Position.DistanceTo(position) < def.aoe)
            .OfType<Filth>()
            .Select(x => x.thickness)
            .Sum();


        return 1.0f * Mathf.Min(amount, def.maximumAmount) / def.maximumAmount;
    }
}
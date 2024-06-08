using RimWorld;
using UnityEngine;

namespace Radiology;

public class MutationNightDweller : MutationCapacityModifier<MutationNightDwellerDef>
{
    protected override float Multiplier()
    {
        var distanceFromMidday = Mathf.Abs(0.45f - GenLocalDate.DayPercent(pawn.Map));
        if (distanceFromMidday > 0.5f)
        {
            distanceFromMidday = 1.0f - distanceFromMidday;
        }

        var m = (distanceFromMidday * 4) - 1f;
        return m < 0 ? m * def.negativeMultiplier : m * def.positiveMultiplier;
    }
}
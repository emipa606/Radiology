using UnityEngine;
using Verse;

namespace Radiology;

public class MutationSimulateIngestion : Mutation<MutationSimulateIngestionDef>
{
    public override void Tick()
    {
        if (Find.TickManager.TicksGame % def.periodTicks != 0)
        {
            return;
        }

        var chance = def.chance;
        if (def.stoppedByHediff != null)
        {
            var stopper = pawn.health.hediffSet.GetFirstHediffOfDef(def.stoppedByHediff);
            if (stopper != null)
            {
                chance *= Mathf.Max(0, 1f - stopper.Severity);
            }
        }

        if (Rand.Value > chance)
        {
            return;
        }

        if (def.outcomeDoers == null)
        {
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach, can be modified during execution
        for (var index = 0; index < def.outcomeDoers.Count; index++)
        {
            var ingestionOutcomeDoer = def.outcomeDoers[index];
            //ingestionOutcomeDoer.DoIngestionOutcome(pawn, null);
            ingestionOutcomeDoer.DoIngestionOutcome(pawn, null, 1);
        }
    }
}
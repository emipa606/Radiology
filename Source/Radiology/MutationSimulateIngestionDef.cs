using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Radiology;

public class MutationSimulateIngestionDef : MutationDef
{
    public readonly float chance = 0.1f;

    public readonly int periodTicks = 600;
    public List<IngestionOutcomeDoer> outcomeDoers;
    public HediffDef stoppedByHediff;

    public MutationSimulateIngestionDef()
    {
        hediffClass = typeof(MutationSimulateIngestion);
    }
}
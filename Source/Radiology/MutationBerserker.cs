namespace Radiology;

internal class MutationBerserker : MutationCapacityModifier<MutationBerserkerDef>
{
    protected override float Multiplier()
    {
        return 1f - pawn.health.summaryHealth.SummaryHealthPercent;
    }
}
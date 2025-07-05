using Verse;

namespace Radiology;

public class CompFilterOperationModeTradeoffRemoveBurn : CompFilterOperationMode
{
    private readonly float multiplier = 5f;

    public CompFilterOperationModeTradeoffRemoveBurn()
    {
        Label = "RadiologyFilter5TimesBurn".Translate();
        Prerequisite = ResearchDefOf.RadiologyFilteringTradeoff;
    }

    public override void Modify(ref RadiationInfo info)
    {
        if (info.rare * multiplier < info.burn)
        {
            info.burn -= info.rare * multiplier;
            info.rare = 0;
        }
        else
        {
            info.burn = 0;
            info.rare -= info.burn / multiplier;
        }
    }
}
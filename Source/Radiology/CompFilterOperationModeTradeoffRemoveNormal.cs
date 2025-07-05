using Verse;

namespace Radiology;

public class CompFilterOperationModeTradeoffRemoveNormal : CompFilterOperationMode
{
    private readonly float multiplier = 5f;

    public CompFilterOperationModeTradeoffRemoveNormal()
    {
        Label = "RadiologyFilter5Times".Translate();
        Prerequisite = ResearchDefOf.RadiologyFilteringTradeoff;
    }

    public override void Modify(ref RadiationInfo info)
    {
        if (info.rare * multiplier < info.normal)
        {
            info.normal -= info.rare * multiplier;
            info.rare = 0;
        }
        else
        {
            info.normal = 0;
            info.rare -= info.normal / multiplier;
        }
    }
}
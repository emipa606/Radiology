namespace Radiology;

public class CompFilterOperationModeTradeoffRemoveNormal : CompFilterOperationMode
{
    public readonly float multiplier = 5f;

    public CompFilterOperationModeTradeoffRemoveNormal()
    {
        Label = "use rare radiation to filter out 5x normal radiation";
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
namespace Radiology;

public class CompFilterOperationModeAdvanced : CompFilterOperationMode
{
    public CompFilterOperationModeAdvanced()
    {
        Label = "filter out 40% of burn radiation and 20% of others";
        Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 0.6f;
        info.normal *= 0.8f;
        info.rare *= 0.8f;
    }
}
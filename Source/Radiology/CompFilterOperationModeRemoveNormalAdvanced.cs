namespace Radiology;

public class CompFilterOperationModeRemoveNormalAdvanced : CompFilterOperationMode
{
    public CompFilterOperationModeRemoveNormalAdvanced()
    {
        Label = "filter out 40% of normal radiation";
        Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.normal *= 0.6f;
    }
}
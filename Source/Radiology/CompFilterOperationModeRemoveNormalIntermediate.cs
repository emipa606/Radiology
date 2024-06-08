namespace Radiology;

public class CompFilterOperationModeRemoveNormalIntermediate : CompFilterOperationMode
{
    public CompFilterOperationModeRemoveNormalIntermediate()
    {
        Label = "filter out 30% of normal radiation";
        Prerequisite = ResearchDefOf.RadiologyFilteringIntermediate;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.normal *= 0.7f;
    }
}
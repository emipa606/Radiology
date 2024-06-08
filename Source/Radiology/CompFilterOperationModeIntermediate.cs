namespace Radiology;

public class CompFilterOperationModeIntermediate : CompFilterOperationMode
{
    public CompFilterOperationModeIntermediate()
    {
        Label = "filter out 30% of burn radiation and 15% of others";
        Prerequisite = ResearchDefOf.RadiologyFilteringIntermediate;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 0.7f;
        info.normal *= 0.85f;
        info.rare *= 0.85f;
    }
}
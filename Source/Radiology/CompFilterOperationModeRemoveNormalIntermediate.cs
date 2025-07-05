using Verse;

namespace Radiology;

public class CompFilterOperationModeRemoveNormalIntermediate : CompFilterOperationMode
{
    public CompFilterOperationModeRemoveNormalIntermediate()
    {
        Label = "RadiologyFilter30Percent".Translate();
        Prerequisite = ResearchDefOf.RadiologyFilteringIntermediate;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.normal *= 0.7f;
    }
}
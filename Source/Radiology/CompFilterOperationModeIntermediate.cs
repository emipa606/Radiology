using Verse;

namespace Radiology;

public class CompFilterOperationModeIntermediate : CompFilterOperationMode
{
    public CompFilterOperationModeIntermediate()
    {
        Label = "RadiologyFilter30PlusPercent".Translate();
        Prerequisite = ResearchDefOf.RadiologyFilteringIntermediate;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 0.7f;
        info.normal *= 0.85f;
        info.rare *= 0.85f;
    }
}
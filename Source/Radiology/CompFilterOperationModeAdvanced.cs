using Verse;

namespace Radiology;

public class CompFilterOperationModeAdvanced : CompFilterOperationMode
{
    public CompFilterOperationModeAdvanced()
    {
        Label = "RadiologyFilter40PlusPercent".Translate();
        Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 0.6f;
        info.normal *= 0.8f;
        info.rare *= 0.8f;
    }
}
using Verse;

namespace Radiology;

public class CompFilterOperationModeRemoveNormalAdvanced : CompFilterOperationMode
{
    public CompFilterOperationModeRemoveNormalAdvanced()
    {
        Label = "RadiologyFilter40Percent".Translate();
        Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.normal *= 0.6f;
    }
}
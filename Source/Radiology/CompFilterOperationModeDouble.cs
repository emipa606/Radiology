namespace Radiology;

public class CompFilterOperationModeDouble : CompFilterOperationMode
{
    public CompFilterOperationModeDouble()
    {
        Label = "double all radiation";
        Prerequisite = ResearchDefOf.RadiologyFilteringAdvanced;
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 2;
        info.normal *= 2;
        info.rare *= 2;
    }
}
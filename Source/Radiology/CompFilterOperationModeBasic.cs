using Verse;

namespace Radiology;

public class CompFilterOperationModeBasic : CompFilterOperationMode
{
    public CompFilterOperationModeBasic()
    {
        Label = "RadiologyFilter20PlusPercent".Translate();
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 0.8f;
        info.normal *= 0.9f;
        info.rare *= 0.9f;
    }
}
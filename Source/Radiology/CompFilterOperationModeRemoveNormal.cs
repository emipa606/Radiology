using Verse;

namespace Radiology;

public class CompFilterOperationModeRemoveNormal : CompFilterOperationMode
{
    public CompFilterOperationModeRemoveNormal()
    {
        Label = "RadiologyFilter20Percent".Translate();
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.normal *= 0.8f;
    }
}
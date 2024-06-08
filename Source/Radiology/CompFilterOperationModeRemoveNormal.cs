namespace Radiology;

public class CompFilterOperationModeRemoveNormal : CompFilterOperationMode
{
    public CompFilterOperationModeRemoveNormal()
    {
        Label = "filter out 20% of normal radiation";
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.normal *= 0.8f;
    }
}
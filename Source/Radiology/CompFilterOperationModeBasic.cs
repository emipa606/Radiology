namespace Radiology;

public class CompFilterOperationModeBasic : CompFilterOperationMode
{
    public CompFilterOperationModeBasic()
    {
        Label = "filter out 20% of burn radiation and 10% of others";
    }

    public override void Modify(ref RadiationInfo info)
    {
        info.burn *= 0.8f;
        info.normal *= 0.9f;
        info.rare *= 0.9f;
    }
}
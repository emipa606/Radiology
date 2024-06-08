namespace Radiology;

public class CancerCompSpreadsDef : CancerCompDef
{
    public readonly bool growDownwards = false;

    public readonly bool growUpwards = false;
    public readonly float mtthDays = 1f;
    public readonly bool mutates = false;

    public CancerCompSpreadsDef()
    {
        Init(typeof(CancerCompSpreads));
    }
}
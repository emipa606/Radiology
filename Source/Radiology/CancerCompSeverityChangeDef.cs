namespace Radiology;

public class CancerCompSeverityChangeDef : CancerCompDef
{
    public float changePerDay;

    public CancerCompSeverityChangeDef()
    {
        Init(typeof(CancerCompSeverityChange));
    }
}
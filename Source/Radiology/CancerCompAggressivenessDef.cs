namespace Radiology;

public class CancerCompAggressivenessDef : CancerCompDef
{
    public float offset;

    public CancerCompAggressivenessDef()
    {
        Init(typeof(CancerCompAggressiveness));
    }
}
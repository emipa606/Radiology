namespace Radiology;

public class CancerCompPainDef : CancerCompDef
{
    public float offset;

    public CancerCompPainDef()
    {
        Init(typeof(CancerCompPain));
    }
}
namespace Radiology;

public class CancerCompDestroyBodyPartDef : CancerCompDef
{
    public float destroyAtSeverity;

    public CancerCompDestroyBodyPartDef()
    {
        Init(typeof(CancerCompDestroyBodyPart));
    }
}
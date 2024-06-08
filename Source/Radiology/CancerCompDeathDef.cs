namespace Radiology;

public class CancerCompDeathDef : CancerCompDef
{
    public float lethalSeverity;

    public CancerCompDeathDef()
    {
        Init(typeof(CancerCompDeath));
    }
}
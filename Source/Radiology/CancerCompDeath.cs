namespace Radiology;

public class CancerCompDeath : CancerComp<CancerCompDeathDef>
{
    public override void Update(int passed)
    {
        cancer.lethalSeverity = def.lethalSeverity;
    }
}
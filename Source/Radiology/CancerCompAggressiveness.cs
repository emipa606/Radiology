namespace Radiology;

public class CancerCompAggressiveness : CancerComp<CancerCompAggressivenessDef>
{
    public override void Update(int passed)
    {
        cancer.stage.socialFightChanceFactor = cancer.Severity * def.offset;
    }
}
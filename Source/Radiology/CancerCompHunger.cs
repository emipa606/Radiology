namespace Radiology;

public class CancerCompHunger : CancerComp<CancerCompHungerDef>
{
    public override object[] DescriptionArgs => [(int)(cancer.Severity * def.offset * 100)];

    public override void Update(int passed)
    {
        cancer.stage.hungerRateFactorOffset = cancer.Severity * def.offset;
    }
}
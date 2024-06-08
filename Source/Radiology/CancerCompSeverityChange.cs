namespace Radiology;

public class CancerCompSeverityChange : CancerComp<CancerCompSeverityChangeDef>
{
    public override void Update(int passed)
    {
        cancer.Severity += passed * def.changePerDay / 60000;
    }
}
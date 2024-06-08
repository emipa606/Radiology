namespace Radiology;

public class CancerCompPain : CancerComp<CancerCompPainDef>
{
    public override void Update(int passed)
    {
        cancer.stage.painOffset = cancer.Severity * def.offset;
    }
}
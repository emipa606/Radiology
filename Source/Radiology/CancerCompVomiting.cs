namespace Radiology;

public class CancerCompVomiting : CancerComp<CancerCompVomitingDef>
{
    public override void Update(int passed)
    {
        cancer.stage.vomitMtbDays = def.mtbDays;
    }
}
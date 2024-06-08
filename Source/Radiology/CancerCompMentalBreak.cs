namespace Radiology;

public class CancerCompMentalBreak : CancerComp<CancerCompMentalBreakDef>
{
    public override void Update(int passed)
    {
        cancer.stage.mentalBreakMtbDays = def.mtbDays;
    }
}
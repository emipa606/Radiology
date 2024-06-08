namespace Radiology;

public class CancerCompPartEfficiency : CancerComp<CancerCompPartEfficiencyDef>
{
    public override object[] DescriptionArgs => [(int)(-Offset * 100)];

    public float Offset => def.offset * cancer.Severity;

    public override void Update(int passed)
    {
        cancer.stage.partEfficiencyOffset = Offset;
    }
}
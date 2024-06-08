namespace Radiology;

public class CancerCompDestroyBodyPart : CancerComp<CancerCompDestroyBodyPartDef>
{
    public override object[] DescriptionArgs => [cancer.Part.Label, (int)(def.destroyAtSeverity * 100)];

    public override void Update(int passed)
    {
        cancer.stage.destroyPart = cancer.Severity >= def.destroyAtSeverity;
    }
}
using Verse;

namespace Radiology;

public class CancerCompCapacityOffset : CancerComp<CancerCompCapacityOffsetDef>
{
    public override object[] DescriptionArgs => [def.capacity.label, (int)(-Offset * 100)];

    public float Offset => def.offset * cancer.Severity;

    public override void Update(int passed)
    {
        cancer.stage.capMods.Add(new PawnCapacityModifier { capacity = def.capacity, offset = Offset });
    }
}
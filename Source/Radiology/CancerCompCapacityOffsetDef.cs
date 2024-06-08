using Verse;

namespace Radiology;

public class CancerCompCapacityOffsetDef : CancerCompDef
{
    public PawnCapacityDef capacity;

    public float offset;

    public CancerCompCapacityOffsetDef()
    {
        Init(typeof(CancerCompCapacityOffset));
    }
}
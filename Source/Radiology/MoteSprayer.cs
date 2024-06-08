using Verse;

namespace Radiology;

public class MoteSprayer
{
    public readonly float initialSpread = 0f;
    public readonly float reflectChance = 0f;
    public readonly bool skip = false;
    public ThingDef mote;
    public FloatRange offset;
    public FloatRange scaleRange;
    public FloatRange speed;
    public float spread;
}
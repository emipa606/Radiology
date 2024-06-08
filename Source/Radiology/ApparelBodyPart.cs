using RimWorld;

namespace Radiology;

internal class ApparelBodyPart : Apparel
{
    public override float GetSpecialApparelScoreOffset()
    {
        return 10000f;
    }
}
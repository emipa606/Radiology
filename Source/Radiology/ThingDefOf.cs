using RimWorld;
using Verse;

namespace Radiology;

[DefOf]
public static class ThingDefOf
{
    public static ThingDef RadiologyRadiationChamber;

    static ThingDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
    }
}
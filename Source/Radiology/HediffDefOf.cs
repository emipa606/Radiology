using RimWorld;
using Verse;

namespace Radiology;

[DefOf]
public static class HediffDefOf
{
    public static HediffDef RadiologyRadiation;
    public static MutationDef MutationSuperSpeed;
    public static CancerDef RadiologyCancer;

    static HediffDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
    }
}
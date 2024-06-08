using RimWorld;
using Verse;

namespace Radiology;

[DefOf]
public static class ResearchDefOf
{
    public static ResearchProjectDef RadiologyFilteringIntermediate;
    public static ResearchProjectDef RadiologyFilteringTradeoff;
    public static ResearchProjectDef RadiologyFilteringAdvanced;

    static ResearchDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(ResearchDefOf));
    }
}
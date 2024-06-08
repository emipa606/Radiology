using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace Radiology;

public static class PawnHelper
{
    private static readonly FieldInfo compsField =
        typeof(ThingWithComps).GetField("comps", BindingFlags.NonPublic | BindingFlags.Instance);

    public static List<ThingComp> Comps(this ThingWithComps thing)
    {
        return compsField.GetValue(thing) as List<ThingComp>;
    }

    public static bool IsShielded(this Pawn pawn)
    {
        if (pawn?.apparel == null)
        {
            return false;
        }

        var damageTest = new DamageInfo(DamageDefOf.Bomb, 0f);
        foreach (var apparel in pawn.apparel.WornApparel)
        {
            if (apparel.CheckPreAbsorbDamage(damageTest))
            {
                return true;
            }
        }

        return false;
    }
}
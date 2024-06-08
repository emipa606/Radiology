using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

/// <summary>
///     Let a nuilding be both facility and affected by facilities when placing it.
/// </summary>
[HarmonyPatch(typeof(CompAffectedByFacilities), nameof(CompAffectedByFacilities.CanPotentiallyLinkTo_Static),
    typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4))]
//typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(ThingDef), typeof(IntVec3), typeof(Rot4), typeof(Map))]
public static class CompAffectedByFacilities_CanPotentiallyLinkTo_Static
{
    public static IAdvancedFacilityConnector GetCompProperties(ThingDef def)
    {
        foreach (var properties in def.comps)
        {
            if (properties is IAdvancedFacilityConnector t)
            {
                return t;
            }
        }

        return null;
    }

    private static void Postfix(ref ThingDef facilityDef, ref IntVec3 facilityPos, ref Rot4 facilityRot,
        ref ThingDef myDef, ref IntVec3 myPos, ref Rot4 myRot, ref bool __result)
    {
        var self = GetCompProperties(facilityDef);
        if (self == null)
        {
            self = GetCompProperties(myDef);
        }

        if (self == null)
        {
            return;
        }

        var res = self.CanLinkTo(__result, facilityDef, facilityPos, facilityRot, myDef, myPos, myRot);
        __result = res;
    }
}
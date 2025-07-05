using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using Verse;

namespace Radiology;

internal static class HelperCompFacility
{
    private static FieldInfo linkedBuildingsField;

    public static List<Thing> LinkedBuildings(this CompFacility facility)
    {
        if (linkedBuildingsField == null)
        {
            linkedBuildingsField =
                typeof(CompFacility).GetField("linkedBuildings", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        return linkedBuildingsField?.GetValue(facility) as List<Thing>;
    }

    private static T Linked<T>(this CompFacility facility) where T : Thing
    {
        foreach (var thing in facility.LinkedBuildings())
        {
            if (thing is T res)
            {
                return res;
            }
        }

        return null;
    }

    public static T Linked<T>(this ThingWithComps thing) where T : Thing
    {
        var comp = thing.GetComp<CompFacility>();

        return comp?.Linked<T>();
    }
}
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

    public static IRadiationReciever LinkedRadiationReciever(this ThingWithComps thing)
    {
        var facility = thing.GetComp<CompFacility>();
        if (facility == null)
        {
            return null;
        }

        foreach (var linkedThing in facility.LinkedBuildings())
        {
            if (linkedThing is IRadiationReciever res)
            {
                return res;
            }

            if (linkedThing is not Building linkedBuilding)
            {
                continue;
            }

            foreach (var comp in linkedBuilding.AllComps)
            {
                res = comp as IRadiationReciever;
                if (res != null)
                {
                    return res;
                }
            }
        }

        return null;
    }

    public static T Linked<T>(this CompFacility facility) where T : Thing
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
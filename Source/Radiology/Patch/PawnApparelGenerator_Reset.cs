using System.Collections.Generic;
using HarmonyLib;
using RimWorld;

namespace Radiology.Patch;

[HarmonyPatch(typeof(PawnApparelGenerator), nameof(PawnApparelGenerator.Reset), [])]
internal class PawnApparelGenerator_Reset
{
    private static void Postfix(ref List<ThingStuffPair> ___allApparelPairs)
    {
        ___allApparelPairs.RemoveAll(x => typeof(ApparelBodyPart).IsAssignableFrom(x.thing.thingClass));
    }
}
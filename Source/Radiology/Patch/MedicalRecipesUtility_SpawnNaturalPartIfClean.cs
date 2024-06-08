using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

[HarmonyPatch(typeof(MedicalRecipesUtility), nameof(MedicalRecipesUtility.SpawnNaturalPartIfClean))]
internal class MedicalRecipesUtility_SpawnNaturalPartIfClean
{
    private static bool Prefix(ref Thing __result, Pawn pawn, BodyPartRecord part, IntVec3 pos, Map map)
    {
        if (!MedicalRecipesUtility_IsCleanAndDroppable.HasAnyMutations(pawn, part))
        {
            return true;
        }

        if (!Radiology.bodyPartItems.TryGetValue(part.def, out var thingDef))
        {
            return true;
        }

        var thing = GenSpawn.Spawn(thingDef, pos, map);
        var comp = thing.TryGetComp<CompHediffStorage>();
        if (comp != null)
        {
            comp.parts.Clear();
            comp.hediffs.Clear();

            foreach (var hediff in pawn.health.hediffSet.hediffs.Where(x => HealthHelper.IsParent(part, x.Part)))
            {
                comp.parts.Add(hediff.Part.def);
                comp.hediffs.Add(hediff);
            }
        }

        __result = thing;
        return false;
    }
}
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

[HarmonyPatch(typeof(MedicalRecipesUtility), nameof(MedicalRecipesUtility.IsCleanAndDroppable))]
internal class MedicalRecipesUtility_IsCleanAndDroppable
{
    private static bool Prefix(ref bool __result, Pawn pawn, BodyPartRecord part)
    {
        if (pawn.Dead || pawn.RaceProps.Animal || !HasAnyMutations(pawn, part) ||
            !Radiology.bodyPartItems.ContainsKey(part.def))
        {
            return true;
        }

        __result = true;
        return false;
    }

    public static bool HasAnyMutations(Pawn pawn, BodyPartRecord part)
    {
        return (from x in pawn.health.hediffSet.hediffs.OfType<Mutation>()
            where x.Part == part && !x.def.isBad
            select x).Any();
    }
}
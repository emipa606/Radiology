using HarmonyLib;
using RimWorld;
using Verse;

namespace Radiology.Patch;

[HarmonyPatch(typeof(PawnApparelGenerator), nameof(PawnApparelGenerator.GenerateStartingApparelFor), typeof(Pawn),
    typeof(PawnGenerationRequest))]
internal class PawnApparelGenerator_GenerateStartingApparelFor
{
    private static bool reset;

    private static bool Prefix()
    {
        if (reset)
        {
            return true;
        }

        reset = true;

        PawnApparelGenerator.Reset();

        return true;
    }
}
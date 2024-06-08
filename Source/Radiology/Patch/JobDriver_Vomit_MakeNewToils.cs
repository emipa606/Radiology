using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse.AI;

namespace Radiology.Patch;

/// <summary>
///     Hook to call vomiting mutation when the pawn is vomiting
/// </summary>
[HarmonyPatch(typeof(JobDriver_Vomit), "MakeNewToils", [])]
public static class JobDriver_Vomit_MakeNewToils
{
    private static IEnumerable<Toil> Postfix(IEnumerable<Toil> list, JobDriver_Vomit __instance)
    {
        var mutations =
            (IEnumerable<MutationVomitItems>)__instance.pawn.health.hediffSet.hediffs.Where(hediff =>
                hediff is MutationVomitItems);
        if (!mutations.Any())
        {
            return list;
        }

        foreach (var toil in list)
        {
            toil.AddFinishAction(delegate
            {
                foreach (var mutation in mutations)
                {
                    mutation.Vomiting(__instance.job.targetA.Cell);
                }
            });
        }

        return list;
    }
}
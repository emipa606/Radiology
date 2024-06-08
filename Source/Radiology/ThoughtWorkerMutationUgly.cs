using System.Linq;
using RimWorld;
using Verse;

namespace Radiology;

internal class ThoughtWorkerMutationUgly : ThoughtWorker
{
    protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
    {
        if (!p.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(p, other) || other.def != p.def)
        {
            return false;
        }

        var mutations = other.health.hediffSet.hediffs.Where(x =>
            x is Mutation mutation && mutation.def.beauty < 0);

        if (!mutations.Any())
        {
            return false;
        }

        var beauty = mutations.Sum(x => ((Mutation)x).def.beauty);
        var impact = beauty / 5;
        if (impact >= 0)
        {
            return false;
        }

        var reason = string.Join(", ", mutations.Select(x => x.def.LabelCap).ToArray());

        return ThoughtState.ActiveAtStage(-impact - 1, reason);
    }
}
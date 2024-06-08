using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Radiology;

internal class JobDriverIrradiate : JobDriver, IPrisonerAllowedJob
{
    private Chamber Chamber => job.GetTarget(TargetIndex.A).Thing as Chamber;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (Chamber == null)
        {
            return false;
        }

        return pawn.CanReserve(job.targetA) && pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOn(() => Chamber.CanIrradiateNow(pawn) != null);
        //           AddFinishAction(delegate () { if (pawn.IsPrisoner) pawn.SetForbidden(false); });

        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
            .FailOnDespawnedOrNull(TargetIndex.A);
        var work = new Toil
        {
            defaultCompleteMode = ToilCompleteMode.Never
        };
        work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
        work.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

        work.tickAction = delegate
        {
            if (pawn.IsHashIntervalTick(60))
            {
                Chamber.Irradiate(work.actor, 60);
            }
        };
        yield return work;
    }
}
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace Radiology;

public class JobDriver_TakeToChamber : JobDriver
{
    private const TargetIndex TakeeIndex = TargetIndex.A;
    private const TargetIndex ChamberIndex = TargetIndex.B;

    private Pawn Takee => (Pawn)job.GetTarget(TargetIndex.A).Thing;
    private Chamber Chamber => (Chamber)job.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        Takee.ClearAllReservations();
        if (pawn.Reserve(Takee, job, 1, -1, null, errorOnFailed))
        {
            return pawn.Reserve(Chamber, job, 1, -1, null, errorOnFailed);
        }

        return false;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.A);
        this.FailOnDestroyedOrNull(TargetIndex.B);
        this.FailOnAggroMentalStateAndHostile(TargetIndex.A);

        // Fail if chamber is no longer valid
        this.FailOn(() =>
        {
            var reason = Chamber.CanIrradiateNow(Takee);
            // Allow if chamber is ready, or if only issue is pawn being hurt
            return reason != null && reason != "ChamberPawnIsHurt";
        });

        // Fail if takee is no longer downed
        this.FailOn(() => !Takee.Downed && !Takee.InMentalState);

        // Add finish action to drop pawn if job is interrupted
        AddFinishAction(delegate
        {
            if (pawn.carryTracker.CarriedThing != null)
            {
                pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out _);
            }
        });

        // Go to the takee
        var goToTakee = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch)
            .FailOnDespawnedNullOrForbidden(TargetIndex.A)
            .FailOnDespawnedNullOrForbidden(TargetIndex.B)
            .FailOn(() => !pawn.CanReach(Chamber, PathEndMode.InteractionCell, Danger.Deadly))
            .FailOnSomeonePhysicallyInteracting(TargetIndex.A);

        // If already carrying, skip to going to chamber
        yield return Toils_Jump.JumpIf(goToTakee, () => pawn.IsCarryingPawn(Takee));

        yield return goToTakee;

        // Pick up the takee
        var startCarrying = Toils_Haul.StartCarryThing(TargetIndex.A);
        yield return startCarrying;

        // Go to the chamber
        var goToChamber = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.InteractionCell)
            .FailOn(() => !pawn.IsCarryingPawn(Takee));

        yield return goToChamber;

        // Place the takee at the chamber's interaction cell
        var placeTakee = ToilMaker.MakeToil("placeTakee");
        placeTakee.initAction = delegate
        {
            if (pawn.carryTracker.CarriedThing != Takee)
            {
                return;
            }

            var position = Chamber.InteractionCell;
            pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out _);
        };
        yield return placeTakee;

        // Trigger irradiation immediately after placement
        var triggerIrradiation = ToilMaker.MakeToil("triggerIrradiation");
        triggerIrradiation.initAction = delegate
        {
            var reason = Chamber.CanIrradiateNow(Takee);
            if (reason != null && reason != "ChamberPawnIsHurt")
            {
                return;
            }

            Chamber.Irradiate(Takee, 60);
            Chamber.Unselect(Takee);
        };
        triggerIrradiation.defaultCompleteMode = ToilCompleteMode.Instant;
        yield return triggerIrradiation;
    }

    public override string GetReport()
    {
        if (pawn.IsCarryingPawn(Takee))
        {
            return "RadiologyCarryingTo".Translate(Takee.LabelShortCap, Chamber.LabelShortCap);
        }

        return base.GetReport();
    }
}
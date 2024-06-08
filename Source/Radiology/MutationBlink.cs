using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Radiology;

public class MutationBlink : Mutation<MutationBlinkDef>
{
    public int cooldown;

    public override void Tick()
    {
        base.Tick();

        if (cooldown > 0)
        {
            cooldown--;
        }

        if (pawn.Map == null)
        {
            return;
        }

        if (!MathHelper.CheckMtthDays(def.mtthDays))
        {
            return;
        }

        BlinkRandomly();
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref cooldown, "cooldown");
    }

    public override IEnumerable<Gizmo> GetMutationGizmos()
    {
        if (!def.controlled)
        {
            yield break;
        }

        var enabled = cooldown <= 0 && pawn.Faction == Faction.OfPlayer;

        if (def.aimed)
        {
            yield return new Command_Target_Location
            {
                defaultLabel = "RadiologyMutationBlinkCommand".Translate(),
                defaultDesc = "RadiologyMutationBlinkTargetedDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("Radiology/Icons/BlinkIcon"),
                action = delegate(LocalTargetInfo target) { BlinkTargeted(target.Cell); },
                targetingParams = new TargetingParameters
                {
                    canTargetLocations = true
                },
                //disabled = !enabled
                Disabled = !enabled
            };
        }
        else
        {
            yield return new Command_Action
            {
                defaultLabel = "RadiologyMutationBlinkCommand".Translate(),
                defaultDesc = "RadiologyMutationBlinkDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("Radiology/Icons/BlinkIcon"),
                action = BlinkRandomly,
                //disabled = !enabled
                Disabled = !enabled
            };
        }
    }

    private void BlinkRandomly()
    {
        BlinkRandomly(pawn.Position.ToVector3(), def.radius);
    }

    private void BlinkRandomly(Vector3 position, float radius)
    {
        var attempts = 5;
        while (--attempts > 0)
        {
            var targetf = position + (Vector3.right.RotatedBy(Rand.Range(0f, 360f)) * Rand.Range(1f, radius));
            var target = targetf.ToIntVec3();

            if (!target.Walkable(pawn.Map))
            {
                continue;
            }

            Blink(target);
            break;
        }
    }

    private void BlinkTargeted(IntVec3 target)
    {
        var distanceToTarget = target.DistanceTo(pawn.Position);
        if (distanceToTarget <= def.radius && !target.Impassable(pawn.Map))
        {
            Blink(target);
            return;
        }

        var pos = pawn.Position.ToVector3();
        var dest = target.ToVector3();
        var dir = (dest - pos).normalized;
        pos += dir * Mathf.Min(distanceToTarget, def.radius);
        for (var distance = distanceToTarget; distance >= 0; distance -= 1)
        {
            var newTarget = pos.ToIntVec3();
            if (!newTarget.Impassable(pawn.Map))
            {
                Blink(newTarget);
                return;
            }

            pos -= dir;
        }

        Blink(pawn.Position);
    }

    private void Blink(IntVec3 target)
    {
        cooldown = def.cooldownTicks;

        RadiologyEffectSpawnerDef.Spawn(def.effectOut, pawn);

        pawn.jobs.StartJob(new Job(RimWorld.JobDefOf.Wait, 1), JobCondition.InterruptForced, null, true, false);
        pawn.SetPositionDirect(target);
        pawn.Drawer.tweener.ResetTweenedPosToRoot();

        RadiologyEffectSpawnerDef.Spawn(def.effectIn, pawn);
    }
}
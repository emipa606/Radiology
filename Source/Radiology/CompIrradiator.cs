using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology;

public class CompIrradiator : ThingComp
{
    public readonly List<float> motesReflectAt = [];
    private Building linked;

    private CompPowerTrader powerComp;

    private Sustainer soundSustainer;

    public int ticksCooldown;
    public new CompPropertiesIrradiator props => base.props as CompPropertiesIrradiator;

    public virtual CompPropertiesIrradiator MoteProps => props;

    private IEnumerable<ThingWithComps> GetFacilitiesBetweenThisAndTarget(Building target)
    {
        var rot = Rotation();

        IEnumerable<Thing> list =
            parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading
                .OrderBy(thing =>
                    rot == Rot4.North ? thing.Position.z :
                    rot == Rot4.South ? -thing.Position.z :
                    rot == Rot4.East ? thing.Position.x :
                    -thing.Position.x
                );

        foreach (var v in list)
        {
            if (v is not ThingWithComps thing)
            {
                continue;
            }

            if (rot.IsHorizontal && !MathHelper.IsBetween(thing.Position.x, parent.Position.x, target.Position.x))
            {
                continue;
            }

            if (!rot.IsHorizontal && !MathHelper.IsBetween(thing.Position.z, parent.Position.z, target.Position.z))
            {
                continue;
            }

            yield return thing;
        }
    }

    private IEnumerable<T> GetModifiers<T, X>(Building target) where X : class where T : ThingComp
    {
        foreach (var thing in GetFacilitiesBetweenThisAndTarget(target))
        {
            foreach (var comp in thing.GetComps<T>())
            {
                if (comp is X)
                {
                    yield return comp;
                }
            }
        }
    }

    private Rot4 Rotation()
    {
        if (linked == null)
        {
            return Rot4.Invalid;
        }

        var a = parent.Position;
        var b = linked.Position;

        var dx = b.x - a.x;
        var dz = b.z - a.z;

        if (Math.Abs(dx) >= Math.Abs(dz))
        {
            return dx > 0 ? Rot4.West : Rot4.East;
        }

        return dz > 0 ? Rot4.North : Rot4.South;
    }

    protected virtual void CreateRadiation(RadiationInfo info, int ticks)
    {
        info.burn = props.burn.perSecond.RandomInRange;
        info.normal = props.mutate.perSecond.RandomInRange;
        info.rare = props.mutateRare.perSecond.RandomInRange;
    }

    public void Irradiate(Building target, RadiationInfo info, int ticks)
    {
        if (!info.visited.Add(this))
        {
            return;
        }

        linked = target;
        if (linked == null)
        {
            return;
        }

        CreateRadiation(info, ticks);
        if (info.Empty())
        {
            return;
        }

        ticksCooldown = ticks;
        var playSound = props.soundIrradiate != null && info.visited.Count(x => x.props.soundIrradiate != null) < 2;
        soundSustainer = playSound
            ? props.soundIrradiate.TrySpawnSustainer(SoundInfo.InMap(parent, MaintenanceType.PerTick))
            : null;

        motesReflectAt.Clear();
        foreach (var comp in GetModifiers<ThingComp, IRadiationModifier>(linked))
        {
            if (comp is CompBlocker)
            {
                motesReflectAt.Add((Rotation().IsHorizontal ? comp.parent.Position.x : comp.parent.Position.z) + 0.5f);
            }

            if (info.secondHand)
            {
                continue;
            }

            var modifier = comp as IRadiationModifier;
            modifier?.Modify(ref info);
        }
    }

    public virtual string CanIrradiateNow(Pawn pawn)
    {
        return powerComp is not { PowerOn: true } ? "IrradiatorNoPower" : null;
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        powerComp = parent.TryGetComp<CompPowerTrader>();
    }

    public void SpawnRadiationMote()
    {
        if (linked == null)
        {
            return;
        }


        var moteProps = MoteProps;
        if (moteProps?.motes == null || !moteProps.motes.Any())
        {
            return;
        }

        var def = moteProps.motes.RandomElement();
        if (def.skip)
        {
            return;
        }

        if (def.speed.max < 0.5f)
        {
            if (Rand.RangeInclusive(0, 5) > linked.Position.DistanceTo(parent.Position))
            {
                return;
            }
        }

        if (ThingMaker.MakeThing(def.mote) is not MoteRadiation moteThrown)
        {
            return;
        }

        var origin = parent.ExactPosition();
        var destination = linked.ExactPosition();
        origin.y = destination.y;
        var sideways = (destination - origin).normalized.RotatedBy(90);

        origin += sideways * Rand.Range(-def.initialSpread, def.initialSpread);
        destination += sideways * Rand.Range(-def.spread, def.spread);

        var positionPercent = Mathf.Sqrt(Rand.Range(0f, 1f));
        var position = def.offset.LerpThroughRange(positionPercent);
        var scale = def.scaleRange.RandomInRange;

        var dir = destination - origin;
        var startOffset = dir * position;
        var angle = startOffset.AngleFlat();

        moteThrown.exactPosition = origin + startOffset;
        moteThrown.exactRotation = angle;
        moteThrown.reflectAt = motesReflectAt;
        moteThrown.reflectChance = def.reflectChance;

        var rot = Rotation();
        if (rot.IsHorizontal)
        {
            moteThrown.deathLocation = linked.ExactPosition().x;
            moteThrown.isHorizontal = true;
            moteThrown.reflectIndex = rot == Rot4.West ? 0 : motesReflectAt.Count - 1;
            moteThrown.reflectIndexChange = rot == Rot4.West ? 1 : -1;
        }
        else
        {
            moteThrown.deathLocation = linked.ExactPosition().z;
            moteThrown.isHorizontal = false;
            moteThrown.reflectIndex = rot == Rot4.North ? 0 : motesReflectAt.Count - 1;
            moteThrown.reflectIndexChange = rot == Rot4.North ? 1 : -1;
        }


        moteThrown.Scale = scale;
        moteThrown.SetVelocity(angle, def.speed.RandomInRange);
        GenSpawn.Spawn(moteThrown, parent.Position, parent.Map);
    }

    public override void CompTick()
    {
        ConsumePower();

        if (ticksCooldown <= 0)
        {
            return;
        }

        soundSustainer?.Maintain();

        SpawnRadiationMote();

        ticksCooldown--;
    }

    public void ConsumePower()
    {
        if (powerComp == null)
        {
            return;
        }

        if (ticksCooldown <= 0)
        {
            powerComp.PowerOutput = -powerComp.Props.PowerConsumption;
        }
        else
        {
            powerComp.PowerOutput = -props.powerConsumption;
        }
    }
}
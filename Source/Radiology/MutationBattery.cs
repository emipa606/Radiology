using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

public class MutationBattery : MutationCapacityModifier<MutationBatteryDef>
{
    private float charge;

    private Vector3 halfNeg = new Vector3(-0.5f, 0f, -0.5f);
    private Building previousBuilding;

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref charge, "charge");
    }

    protected override float Multiplier()
    {
        Drain();

        var res = (charge / def.capacity * 2) - 1f;

        charge -= def.discargePerSecond;
        if (charge < 0)
        {
            charge = 0;
        }

        return res;
    }

    public void Drain()
    {
        Building building = null;
        if (previousBuilding != null && previousBuilding.Position.DistanceTo(pawn.Position) < def.range)
        {
            building = previousBuilding;
        }

        if (building == null)
        {
            building = pawn.Map.listerBuildings.allBuildingsColonistElecFire.FirstOrDefault(x =>
                x.Position.DistanceTo(pawn.Position) < def.range);
        }

        previousBuilding = building;

        var power = building?.GetComp<CompPower>();
        if (power?.PowerNet == null)
        {
            return;
        }

        var amount = Mathf.Min(power.PowerNet.AvailablePower(), def.drain);
        var canTake = def.efficiency < 0.0001 ? def.capacity : (def.capacity - charge) / def.efficiency;

        if (amount > canTake)
        {
            amount = canTake;
        }

        if (amount < def.drain / 2)
        {
            return;
        }

        power.PowerNet.Drain(amount);
        charge += amount * def.efficiency;

        RadiologyEffectSpawnerDef.Spawn(def.effectDraining, pawn.Map, pawn.TrueCenter(),
            (building.Position - pawn.Position).AngleFlat);
        RadiologyEffectSpawnerDef.Spawn(def.effectDrained, pawn.Map, building.RandomPointNearTrueCenter(),
            (pawn.Position - building.Position).AngleFlat);
    }
}
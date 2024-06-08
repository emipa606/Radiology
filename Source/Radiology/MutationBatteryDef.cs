using Verse;

namespace Radiology;

public class MutationBatteryDef : MutationDef
{
    public readonly float efficiency = 1.0f;
    public float capacity;
    public float discargePerSecond;
    public float drain;
    public RadiologyEffectSpawnerDef effectDrained;

    public RadiologyEffectSpawnerDef effectDraining;

    public float range;

    public MutationBatteryDef()
    {
        hediffClass = typeof(MutationBattery);
    }
}
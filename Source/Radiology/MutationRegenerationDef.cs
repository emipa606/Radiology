using Verse;

namespace Radiology;

public class MutationRegenerationDef : MutationDef
{
    public readonly bool healMissingParts = false;

    public readonly int periodTicks = 600;
    public RadiologyEffectSpawnerDef effectRegeneration;

    public MutationRegenerationDef()
    {
        hediffClass = typeof(MutationRegeneration);
    }
}
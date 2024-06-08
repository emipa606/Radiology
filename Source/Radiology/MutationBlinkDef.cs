using Verse;

namespace Radiology;

public class MutationBlinkDef : MutationDef
{
    public readonly int cooldownTicks = 240;

    public readonly float mtthDays = -1;
    public bool aimed;
    public bool controlled;
    public RadiologyEffectSpawnerDef effectIn;

    public RadiologyEffectSpawnerDef effectOut;
    public float radius;

    public MutationBlinkDef()
    {
        hediffClass = typeof(MutationBlink);
    }
}
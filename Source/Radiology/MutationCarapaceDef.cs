using Verse;

namespace Radiology;

public class MutationCarapaceDef : MutationDef
{
    public RadiologyEffectSpawnerDef effectReflect;

    public float ratio;

    public MutationCarapaceDef()
    {
        hediffClass = typeof(MutationCarapace);
    }
}
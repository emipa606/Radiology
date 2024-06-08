using System.Collections.Generic;
using Verse;

namespace Radiology;

public class MutationVomitItemsDef : MutationDef
{
    public IntRange count;
    public FloatRange damage;

    public RadiologyEffectSpawnerDef effect;
    public float hurtChance;

    public List<BodyPartDef> hurtParts;

    public ThingDef item;

    public MutationVomitItemsDef()
    {
        hediffClass = typeof(MutationVomitItems);
    }
}
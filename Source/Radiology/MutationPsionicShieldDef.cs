using System.Collections.Generic;
using Verse;

namespace Radiology;

public class MutationPsionicShieldDef : MutationDef
{
    public RadiologyEffectSpawnerDef effectAbsorbed;
    public RadiologyEffectSpawnerDef effectBroken;
    public RadiologyEffectSpawnerDef effectRestored;

    public float health;

    public List<DamageDef> protectsAgainst;

    public int regenerationDelayTicks;
    public float regenratedPerSecond;
    public List<DamageDef> uselessAgainst;

    public MutationPsionicShieldDef()
    {
        hediffClass = typeof(MutationPsionicShield);
    }
}
using Verse;

namespace Radiology;

public class MutationPsionicShield : Mutation<MutationPsionicShieldDef>
{
    public float health;
    public int regenerationDelay;

    protected override ThingComp[] GetComps()
    {
        return [new CompPsionicShield { mutation = this }];
    }

    public void ApplyDamage(DamageInfo dinfo, out bool absorbed)
    {
        absorbed = false;

        if (def.protectsAgainst != null && !def.protectsAgainst.Contains(dinfo.Def))
        {
            return;
        }

        if (def.uselessAgainst != null && def.uselessAgainst.Contains(dinfo.Def))
        {
            return;
        }

        regenerationDelay = def.regenerationDelayTicks;

        if (health == 0 && def.health != 0)
        {
            return;
        }

        if (dinfo.Amount <= health || def.health == 0)
        {
            health -= dinfo.Amount;
            absorbed = true;

            RadiologyEffectSpawnerDef.Spawn(def.effectAbsorbed, pawn, dinfo.Angle + 180);
        }
        else
        {
            dinfo.SetAmount(dinfo.Amount - health);
            health = 0;
        }

        if (health == 0 && def.health != 0)
        {
            RadiologyEffectSpawnerDef.Spawn(def.effectBroken, pawn, dinfo.Angle + 180);
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref health, "health");
        Scribe_Values.Look(ref regenerationDelay, "regenerationDelay");
    }

    public override void Tick()
    {
        base.Tick();

        if (regenerationDelay > 0)
        {
            regenerationDelay--;
        }
        else
        {
            if (health == 0 && def.health != 0 && def.regenratedPerSecond != 0)
            {
                RadiologyEffectSpawnerDef.Spawn(def.effectRestored, pawn);
            }

            health += def.regenratedPerSecond / 60;
            if (health > def.health)
            {
                health = def.health;
            }
        }
    }
}
using RimWorld;
using Verse;

namespace Radiology;

public class MutationVolatile : Mutation<MutationVolatileDef>
{
    private float healthy = -1;

    public override void Tick()
    {
        base.Tick();

        if (!pawn.IsHashIntervalTick(60))
        {
            return;
        }

        var health = pawn.health.hediffSet.GetPartHealth(Part);
        if (health > healthy)
        {
            healthy = health;
        }

        if (!(health < healthy))
        {
            return;
        }

        GenExplosion.DoExplosion(pawn.Position, pawn.Map, 6, DamageDefOf.Bomb, pawn);
        healthy = health;
    }
}
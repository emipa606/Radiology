using System.Linq;
using RimWorld;
using Verse;

namespace Radiology;

public sealed class MutationVomitItems : Mutation<MutationVomitItemsDef>
{
    public void Vomiting(IntVec3 cell)
    {
        if (def.item == null)
        {
            return;
        }

        var thing = ThingMaker.MakeThing(def.item);
        thing.stackCount = def.count.RandomInRange;
        GenPlace.TryPlaceThing(thing, cell, pawn.Map, ThingPlaceMode.Direct);

        RadiologyEffectSpawnerDef.Spawn(def.effect, pawn);

        if (def.hurtParts == null)
        {
            return;
        }

        foreach (var unused in def.hurtParts)
        {
            foreach (var part in pawn.health.hediffSet.GetNotMissingParts().Where(x => def.hurtParts.Contains(x.def)))
            {
                if (Rand.Value > def.hurtChance)
                {
                    continue;
                }

                var dinfo = new DamageInfo(DamageDefOf.Cut, def.damage.RandomInRange, 999999f, -1f, thing, part);
                pawn.TakeDamage(dinfo);
            }
        }
    }
}
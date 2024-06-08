using System.Collections.Generic;
using Radiology;
using UnityEngine;

namespace Verse;

public class RadiologyEffectSpawnerDef : Def
{
    public List<RadiologyEffectSpawnerDef> alsoSpawn;
    public FloatRange offset;
    public List<AutomaticSubEffect> subEffects;

    public static void Spawn(RadiologyEffectSpawnerDef effect, Map map, Vector3 position, float angle = 0f)
    {
        effect?.Spawn(map, position, angle);
    }

    public static void Spawn(RadiologyEffectSpawnerDef effect, Pawn pawn, float angle = 0f)
    {
        Spawn(effect, pawn.Map, pawn.ExactPosition(), angle);
    }

    public void Spawn(Map map, Vector3 position, float angle)
    {
        if (map == null)
        {
            return;
        }

        var origin = position + (new Vector3(1f, 0f, 0f).RotatedBy(Rand.Range(0f, 360f)) * offset.RandomInRange);

        if (subEffects != null)
        {
            foreach (var subEffect in subEffects)
            {
                subEffect.Spawn(map, origin, angle);
            }
        }

        if (alsoSpawn == null)
        {
            return;
        }

        foreach (var spawner in alsoSpawn)
        {
            spawner.Spawn(map, origin, angle);
        }
    }
}
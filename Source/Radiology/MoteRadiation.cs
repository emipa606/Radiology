using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Radiology;

internal class MoteRadiation : MoteThrown
{
    internal float deathLocation;
    internal bool isHorizontal;
    internal List<float> reflectAt;
    internal float reflectChance;
    internal int reflectIndex;
    internal int reflectIndexChange;

    private void Kill()
    {
        spawnTick = Find.TickManager.TicksGame - (int)((def.mote.Lifespan - def.mote.fadeOutTime) * 60);
    }

    protected override Vector3 NextExactPosition(float deltaTime)
    {
        var result = exactPosition + (velocity * deltaTime);

        if (isHorizontal && !MathHelper.IsSameSign(exactPosition.x - deathLocation, result.x - deathLocation))
        {
            Kill();
            return result;
        }

        if (!isHorizontal && !MathHelper.IsSameSign(exactPosition.z - deathLocation, result.z - deathLocation))
        {
            Kill();
            return result;
        }

        var reflect = false;
        if (reflectAt != null && reflectIndex >= 0 && reflectIndex < reflectAt.Count)
        {
            var coord = reflectAt[reflectIndex];
            if (isHorizontal)
            {
                if (!MathHelper.IsSameSign(exactPosition.x - coord, result.x - coord))
                {
                    reflectIndex += reflectIndexChange;
                    if (Rand.Range(0f, 1f) < reflectChance)
                    {
                        velocity.x = -velocity.x;
                        reflect = true;
                    }
                }
            }
            else
            {
                if (!MathHelper.IsSameSign(exactPosition.z - coord, result.z - coord))
                {
                    reflectIndex += reflectIndexChange;
                    if (Rand.Range(0f, 1f) < reflectChance)
                    {
                        velocity.z = -velocity.z;
                        reflect = true;
                    }
                }
            }
        }

        if (!reflect)
        {
            return result;
        }

        result = exactPosition + (velocity * deltaTime);
        exactRotation = velocity.ToAngleFlat() + 90;
        Kill();

        return result;
    }
}
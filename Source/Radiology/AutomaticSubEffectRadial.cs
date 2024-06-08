using UnityEngine;
using Verse;

namespace Radiology;

public class AutomaticSubEffectRadial : AutomaticSubEffect
{
    public readonly float arc = 360f;
    public ThingDef mote;
    public int moteCount;
    public FloatRange radius;
    public FloatRange rotationRate;
    public FloatRange scale;
    public float speed;

    public override void Spawn(Map map, Vector3 origin, float initialAngle)
    {
        if (map == null)
        {
            return;
        }

        for (var i = 0; i < moteCount; i++)
        {
            if (ThingMaker.MakeThing(mote) is not MoteThrown moteThrown)
            {
                return;
            }

            moteThrown.thingIDNumber = -1 - i;

            var angle = initialAngle + Rand.Range(-arc / 2, arc / 2);
            var dir = new Vector3(0f, 0f, 1f).RotatedBy(angle);

            var magnitude = Rand.Range(0f, 1f);
            var actualScale = scale.LerpThroughRange(magnitude);
            moteThrown.exactPosition = origin + (dir * radius.LerpThroughRange(magnitude));
            moteThrown.exactRotation = angle;
            moteThrown.Scale = actualScale;
            moteThrown.SetVelocity(angle, magnitude * speed);
            moteThrown.rotationRate = rotationRate.RandomInRange;
            GenSpawn.Spawn(moteThrown, origin.ToIntVec3(), map);
            moteThrown.spawnTick -= (int)(magnitude * 0.75f * (mote.mote.fadeOutTime * 60));
        }
    }
}
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology;

public class AutomaticSubEffectSound : AutomaticSubEffect
{
    public SoundDef sound;

    public override void Spawn(Map map, Vector3 position, float angle)
    {
        sound.PlayOneShot(new TargetInfo(position.ToIntVec3(), map));
    }
}
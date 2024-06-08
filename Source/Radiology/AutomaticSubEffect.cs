using UnityEngine;
using Verse;

namespace Radiology;

public abstract class AutomaticSubEffect
{
    public abstract void Spawn(Map map, Vector3 position, float angle);
}
using Verse;

namespace Radiology;

public class MutcompSpawnFilthOnHitProps : MutcompProps<MutcompSpawnFilthOnHit>
{
    public readonly float countPerDamage = 1;
    public ThingDef filth;
}
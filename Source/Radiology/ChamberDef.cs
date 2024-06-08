using System.Collections.Generic;
using Verse;

namespace Radiology;

public class ChamberDef : ThingDef
{
    public List<AffectedBodyPart> bodyParts;
    public RadiologyEffectSpawnerDef burnEffect;
    public FloatRange burnThreshold;

    private Dictionary<BodyPartDef, float> cachedPartsMap;
    public FloatRange mutateThreshold;

    public Dictionary<BodyPartDef, float> PartsMap
    {
        get
        {
            if (cachedPartsMap != null)
            {
                return cachedPartsMap;
            }

            cachedPartsMap = new Dictionary<BodyPartDef, float>();
            if (bodyParts == null)
            {
                return cachedPartsMap;
            }

            foreach (var x in bodyParts)
            {
                cachedPartsMap[x.part] = x.part.hitPoints;
            }

            return cachedPartsMap;
        }
    }

    public float GetPartWeight(Pawn pawn, BodyPartRecord x)
    {
        return x.def.IsSolid(x, pawn.health.hediffSet.hediffs) ? 0f : PartsMap.TryGetValue(x.def);
    }
}
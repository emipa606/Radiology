using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

public class PlaceWorkerShowFacilitiesConnections : PlaceWorker
{
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
    {
        var currentMap = Find.CurrentMap;
        if (def.HasComp(typeof(CompAffectedByFacilities)))
        {
            CompAffectedByFacilities.DrawLinesToPotentialThingsToLinkTo(def, center, rot, currentMap);
        }

        if (def.HasComp(typeof(CompFacility)))
        {
            CompFacility.DrawLinesToPotentialThingsToLinkTo(def, center, rot, currentMap);
        }
    }
}
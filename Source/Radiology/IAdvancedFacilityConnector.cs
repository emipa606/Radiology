﻿using Verse;

namespace Radiology;

public interface IAdvancedFacilityConnector
{
    bool CanLinkTo(bool baseResult, ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef,
        IntVec3 myPos, Rot4 myRot);
}
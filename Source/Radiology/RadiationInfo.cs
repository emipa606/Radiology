using System.Collections.Generic;
using Verse;

namespace Radiology;

public class RadiationInfo
{
    public float burn;
    public ChamberDef chamberDef;
    public float normal;
    public BodyPartRecord part;
    public Pawn pawn;
    public float rare;
    public bool secondHand;

    public HashSet<CompIrradiator> visited;

    public void Add(RadiationInfo info)
    {
        burn += info.burn;
        normal += info.normal;
        rare += info.rare;
    }

    public bool Empty()
    {
        return burn <= 0 && normal <= 0 && rare <= 0;
    }

    public void CopyFrom(RadiationInfo other)
    {
        chamberDef = other.chamberDef;
        pawn = other.pawn;
        part = other.part;
        secondHand = other.secondHand;
        visited = other.visited;
    }
}
using System.Collections.Generic;
using Verse;

namespace Radiology;

public class CompPropertiesIrradiator : CompProperties
{
    public RadiationInflurence burn;

    public List<MoteSprayer> motes;
    public RadiationInflurence mutate;
    public RadiationInflurence mutateRare;
    public float powerConsumption;

    public SoundDef soundIrradiate;

    public CompPropertiesIrradiator()
    {
        compClass = typeof(CompIrradiator);
    }

    public override void ResolveReferences(ThingDef parentDef)
    {
        base.ResolveReferences(parentDef);

        if (burn == null)
        {
            burn = new RadiationInflurence();
        }

        if (mutate == null)
        {
            mutate = new RadiationInflurence();
        }
    }
}
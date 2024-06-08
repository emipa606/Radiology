using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Radiology;

internal class CompRadiationExtender : CompIrradiator, IRadiationReciever
{
    public CompPropertiesIrradiator moteProps;

    public override CompPropertiesIrradiator MoteProps => moteProps;
    public Building Building => parent as Building;

    private IEnumerable<CompIrradiator> GetIrradiators()
    {
        foreach (var v in parent.GetComp<CompAffectedByFacilities>().LinkedFacilitiesListForReading)
        {
            if (v is not ThingWithComps thing)
            {
                continue;
            }

            foreach (var comp in thing.GetComps<CompIrradiator>())
            {
                yield return comp;
            }
        }
    }

    protected override void CreateRadiation(RadiationInfo fullInfo, int ticks)
    {
        foreach (var comp in GetIrradiators().InRandomOrder())
        {
            var info = new RadiationInfo();
            info.CopyFrom(fullInfo);
            comp.Irradiate(parent as Building, info, ticks);
            if (info.Empty())
            {
                continue;
            }

            moteProps = comp.MoteProps;

            fullInfo.Add(info);
        }
    }
}
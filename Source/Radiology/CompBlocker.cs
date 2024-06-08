using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

internal class CompBlocker : ThingComp, ISelectMultiple<BodyPartRecord>, IRadiationModifier
{
    public readonly HashSet<BodyPartRecord> allParts = [];
    private readonly BodyDef bodyDef = BodyDefOf.Human;

    public HashSet<BodyPartRecord> enabledParts = [];

    private CompFacility facility;
    private CompPowerTrader powerComp;

    public new CompPropertiesBlocker props => base.props as CompPropertiesBlocker;

    public void Modify(ref RadiationInfo info)
    {
        if (info.part == null || !enabledParts.Contains(info.part) || !(Rand.Range(0.0f, 1.0f) < props.blockChance))
        {
            return;
        }

        info.burn = 0;
        info.normal = 0;
        info.rare = 0;
    }

    public IEnumerable<BodyPartRecord> All()
    {
        return allParts;
    }

    public string Label(BodyPartRecord obj)
    {
        return obj.LabelCap;
    }

    public bool IsSelected(BodyPartRecord obj)
    {
        return enabledParts.Contains(obj);
    }

    public void Select(BodyPartRecord obj)
    {
        enabledParts.Add(obj);
    }

    public void Unselect(BodyPartRecord obj)
    {
        enabledParts.Remove(obj);
    }

    private void assignAllParts()
    {
        allParts.Clear();

        var irradiators = facility.LinkedBuildings();
        if (irradiators.FirstOrDefault(x =>
                (x as Building)?.GetComp<CompIrradiator>() != null) is not Building building)
        {
            return;
        }

        var comp = building.GetComp<CompIrradiator>();
        var chamber = comp.parent.Linked<Chamber>();
        if (chamber == null)
        {
            return;
        }

        var partsThatCanBeIrradiated = chamber.def.bodyParts.Select(x => x.part);
        var parts = bodyDef.AllParts.Where(x => partsThatCanBeIrradiated.Contains(x.def));

        foreach (var v in parts)
        {
            allParts.Add(v);
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (parent.Faction != Faction.OfPlayer)
        {
            yield break;
        }

        assignAllParts();
        if (!allParts.Any())
        {
            yield break;
        }

        yield return new Command_Action
        {
            defaultLabel = "CommandRadiologyAssignRadiationBlockingLabel".Translate(),
            defaultDesc = "CommandRadiologyAssignRadiationBlockingDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("Radiology/Icons/SetupBlocking"),
            action = delegate
            {
                Find.WindowStack.Add(new DialogSelectMultiple<BodyPartRecord>(this)
                {
                    LabelSelected = "RadiologyFilterEnabled",
                    LabelNotSelected = "RadiologyFilterDisabled",
                    SelectedLimit = props.blockedBodyPartLimit
                });
            },
            hotKey = KeyBindingDefOf.Misc3
        };
    }

    public override string CompInspectStringExtra()
    {
        var list = string.Join(", ", enabledParts.Select(x => x.Label).ToArray());
        if (list.Length == 0)
        {
            list = "RadiologyFilterNone".Translate();
        }

        return string.Format("RadiologyFilterBlocking".Translate(), list);
    }

    public override void PostExposeData()
    {
        Scribe_Collections.Look(ref enabledParts, "enabledParts", LookMode.BodyPart);
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        powerComp = parent.TryGetComp<CompPowerTrader>();
        facility = parent.TryGetComp<CompFacility>();
    }
}
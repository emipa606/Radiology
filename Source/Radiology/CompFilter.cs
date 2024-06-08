using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

internal class CompFilter : ThingComp, IRadiationModifier, ISelectMultiple<CompFilterOperationMode>
{
    public static readonly CompFilterOperationMode[] operationModes =
    [
        new CompFilterOperationModeBasic(),
        new CompFilterOperationModeIntermediate(),
        new CompFilterOperationModeAdvanced(),
        new CompFilterOperationModeRemoveNormal(),
        new CompFilterOperationModeRemoveNormalIntermediate(),
        new CompFilterOperationModeRemoveNormalAdvanced(),
        new CompFilterOperationModeTradeoffRemoveBurn(),
        new CompFilterOperationModeTradeoffRemoveNormal(),
        new CompFilterOperationModeDouble()
    ];

    private int mode = -1;
    public new CompPropertiesFilter props => base.props as CompPropertiesFilter;

    private CompFilterOperationMode Mode => mode >= 0 && mode < operationModes.Length ? operationModes[mode] : null;

    public void Modify(ref RadiationInfo info)
    {
        var m = Mode;

        m?.Modify(ref info);
    }

    public IEnumerable<CompFilterOperationMode> All()
    {
        return operationModes.Where(x => x.Available);
    }

    public string Label(CompFilterOperationMode obj)
    {
        return obj.Label.CapitalizeFirst();
    }

    public bool IsSelected(CompFilterOperationMode obj)
    {
        return Mode == obj;
    }

    public void Select(CompFilterOperationMode obj)
    {
        mode = operationModes.FirstIndexOf(x => x == obj);
    }

    public void Unselect(CompFilterOperationMode obj)
    {
        mode = -1;
    }

    public override string CompInspectStringExtra()
    {
        var m = Mode;
        var label = m == null ? "RadiologyFilterOperationModeNone".Translate().RawText : m.Label;
        return string.Format("RadiologyFilterOperationMode".Translate(), label);
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (parent.Faction != Faction.OfPlayer)
        {
            yield break;
        }

        yield return new Command_Action
        {
            defaultLabel = "RadiologyFilterOperationModeLabel".Translate(),
            defaultDesc = "RadiologyFilterOperationModeDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("Radiology/Icons/SetupFiltering"),
            action = delegate
            {
                Find.WindowStack.Add(new DialogSelectMultiple<CompFilterOperationMode>(this)
                {
                    LabelSelected = "RadiologyFilterOperationModeSelected",
                    LabelNotSelected = "RadiologyFilterOperationModeNotSelected",
                    SelectedLimit = 1
                });
            },
            hotKey = KeyBindingDefOf.Misc3
        };
    }

    public override void PostExposeData()
    {
        Scribe_Values.Look(ref mode, "mode", -1);
    }
}
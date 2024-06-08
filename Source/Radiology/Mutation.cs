using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Radiology;

public class Mutation : HediffWithComps
{
    private List<ThingComp> thingComps;
    public new MutationDef def => base.def as MutationDef;

    private void CreateThingComps()
    {
        if (thingComps != null)
        {
            return;
        }

        var thingCompsFound = GetComps();
        if (thingCompsFound == null)
        {
            return;
        }

        thingComps = [];
        var list = pawn.Comps();

        foreach (var comp in thingCompsFound)
        {
            comp.parent = pawn;
            thingComps.Add(comp);
            list.Add(comp);

            if (pawn.Map != null)
            {
                comp.PostSpawnSetup(false);
            }
        }
    }

    private void RemoveThingComps()
    {
        if (thingComps == null)
        {
            return;
        }

        var list = pawn.Comps();

        foreach (var comp in thingComps)
        {
            list.RemoveAll(x => x == comp);
        }

        thingComps = null;
    }

    private void InitializeThingComps(bool remove)
    {
        var thingCompsFound = GetComps();
        if (thingCompsFound == null)
        {
            return;
        }

        var list = pawn.Comps();
        foreach (var comp in thingCompsFound)
        {
            if (remove)
            {
                list.RemoveAll(x => x.GetType() == comp.GetType() && x.props == comp.props);
                continue;
            }

            var existing = list.FirstOrDefault(x => x.GetType() == comp.GetType() && x.props == comp.props);
            if (existing != null)
            {
                continue;
            }

            comp.parent = pawn;
            list.Add(comp);

            if (pawn.Map != null)
            {
                comp.PostSpawnSetup(false);
            }
        }
    }

    private void CreateApparel()
    {
        InitializeApparel(false);
    }

    private void RemoveApparel()
    {
        InitializeApparel(true);
    }

    private void InitializeApparel(bool remove)
    {
        if (def.apparel == null)
        {
            return;
        }

        var list = pawn.apparel.WornApparel;
        foreach (var apparelDef in def.apparel)
        {
            // this adds tools to the apparel so that damage capabilities are showin in the inventory tooltip
            foreach (var comp in def.comps.OfType<HediffCompProperties_VerbGiver>())
            {
                foreach (var tool in comp.tools)
                {
                    if (apparelDef.tools == null)
                    {
                        apparelDef.tools = [];
                    }

                    if (!apparelDef.tools.Contains(tool))
                    {
                        apparelDef.tools.Add(tool);
                    }
                }
            }

            var existing = list.FirstOrDefault(x => x.def == apparelDef);

            if (existing != null && remove)
            {
                pawn.apparel.Remove(existing);

                existing.Destroy();
            }
            else if (existing == null && !remove)
            {
                if (ThingMaker.MakeThing(apparelDef) is Apparel apparel)
                {
                    pawn.apparel.Wear(apparel, true, true);
                }
            }
        }
    }

    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);

        RadiologyEffectSpawnerDef.Spawn(def.SpawnEffect(pawn), pawn);

        CreateThingComps();
        CreateApparel();
    }

    public override void PostRemoved()
    {
        RemoveThingComps();
        RemoveApparel();
    }

    public override void ExposeData()
    {
        base.ExposeData();
    }

    internal void PostLoad()
    {
        CreateThingComps();
        CreateApparel();
    }

    // XXX base game has GetGizmos for HediffWithComps now so this may not be needed; keeping for now
    public virtual IEnumerable<Gizmo> GetMutationGizmos()
    {
        yield break;
    }

    protected virtual ThingComp[] GetComps()
    {
        return null;
    }
}

public class Mutation<T> : Mutation where T : MutationDef
{
    public new T def => base.def as T;
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

public class Chamber : Building, ISelectMultiple<Pawn>, IRadiationReciever
{
    public readonly RadiationTracker radiationTracker = new RadiationTracker();
    public HashSet<Pawn> assigned = [];
    public Pawn currentUser;
    public float damageThreshold = 0.5f;

    private CompAffectedByFacilities facilitiesComp;
    public MutationDef lastMutation;
    public int lastMutationTick;
    private CompPowerTrader powerComp;

    public int ticksCooldown;
    public new ChamberDef def => base.def as ChamberDef;

    public Building Building => this;

    public IEnumerable<Pawn> All()
    {
        return Spawned ? Map.mapPawns.FreeColonistsAndPrisonersSpawned : Enumerable.Empty<Pawn>();
    }

    string ISelectMultiple<Pawn>.Label(Pawn obj)
    {
        return (obj.IsPrisoner ? "Prisoner: " : "Colonist: ") + obj.LabelCap;
    }

    public bool IsSelected(Pawn obj)
    {
        return assigned.Contains(obj);
    }

    public void Select(Pawn obj)
    {
        assigned.Add(obj);
    }

    public void Unselect(Pawn obj)
    {
        assigned.Remove(obj);
    }

    public string CanIrradiateNow(Pawn pawn = null)
    {
        if (powerComp is not { PowerOn: true })
        {
            return "ChamberNoPower";
        }

        if (pawn != currentUser && ticksCooldown > 0)
        {
            return "ChamberCooldown";
        }

        var room = Position.GetRoom(Find.CurrentMap);
        if (room == null || room.PsychologicallyOutdoors)
        {
            return "ChamberNoRoom";
        }

        var count = 0;
        string intermediateProblem = null;
        foreach (var comp in GetIrradiators())
        {
            var problem = comp.CanIrradiateNow(pawn);
            if (problem == null)
            {
                count++;
            }
            else
            {
                intermediateProblem = problem;
            }
        }

        if (count == 0)
        {
            return intermediateProblem ?? "ChamberNoIrradiator";
        }

        if (pawn != null && !IsHealthyEnoughForIrradiation(pawn))
        {
            return "ChamberPawnIsHurt";
        }

        if (pawn != null && !assigned.Contains(pawn))
        {
            return "ChamberPawnNotAssigned";
        }

        if (pawn != null && pawn.apparel.WornApparel.Any(apparel =>
                apparel.def.statBases.Any(modifier => modifier.stat == StatDefOf.EnergyShieldEnergyMax)))
        {
            return "ChamberShieldbelt";
        }

        return null;
    }

    private IEnumerable<CompIrradiator> GetIrradiators()
    {
        foreach (var v in facilitiesComp.LinkedFacilitiesListForReading)
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

    public override void Tick()
    {
        base.Tick();

        if (ticksCooldown > 0)
        {
            ticksCooldown--;
        }
    }

    public BodyPartRecord GetBodyPart(Pawn pawn)
    {
        if (pawn == null)
        {
            return null;
        }

        pawn.health.hediffSet.GetNotMissingParts()
            .TryRandomElementByWeight(x => def.GetPartWeight(pawn, x), out var res);
        return res;
    }

    public void Irradiate(Pawn pawn, int ticksCooldown)
    {
        this.ticksCooldown = ticksCooldown;
        currentUser = pawn;

        radiationTracker.Clear();

        var room = Position.GetRoom(Map);
        var actualPawn = Map.mapPawns.AllPawns.Where(x => x.GetRoom() == room).RandomElementWithFallback(pawn);

        foreach (var comp in GetIrradiators())
        {
            var info = new RadiationInfo
            {
                chamberDef = def, pawn = actualPawn, part = GetBodyPart(actualPawn), secondHand = actualPawn != pawn,
                visited = []
            };
            comp.Irradiate(this, info, ticksCooldown);
            radiationTracker.burn += info.burn;
            radiationTracker.normal += info.normal;
            radiationTracker.rare += info.rare;

            if (actualPawn.IsShielded())
            {
                continue;
            }

            var radiation = RadiationHelper.GetHediffRadition(info.part, info.pawn);
            if (radiation == null)
            {
                continue;
            }

            radiation.burn += info.burn;
            radiation.normal += info.normal;
            radiation.rare += info.rare;

            ApplyBurn(actualPawn, radiation);
            ApplyMutation(actualPawn, radiation);
        }
    }


    public void ApplyBurn(Pawn pawn, HediffRadiation radiation)
    {
        var burnAmount = def.burnThreshold.RandomInRange;
        if (radiation.burn < burnAmount)
        {
            return;
        }

        radiation.burn -= burnAmount;

        var dinfo = new DamageInfo(DamageDefOf.Burn, burnAmount, 999999f, -1f, this, radiation.Part);
        pawn.TakeDamage(dinfo);

        RadiologyEffectSpawnerDef.Spawn(def.burnEffect, pawn);
    }

    public void ApplyMutation(Pawn pawn, HediffRadiation radiation)
    {
        if (radiation.normal + radiation.rare <= def.mutateThreshold.RandomInRange)
        {
            return;
        }

        var ratio = radiation.rare / (radiation.normal + radiation.rare);
        radiation.rare = 0;
        radiation.normal = 0;

        SpawnMutation(pawn, radiation.Part, ratio, radiation);
    }

    public void SpawnMutation(Pawn pawn, BodyPartRecord part, float ratio, HediffRadiation radiation = null)
    {
        var mutatedParts = RadiationHelper.MutatePawn(pawn, part, ratio, out var mutation);
        lastMutation = mutation.def;
        lastMutationTick = Find.TickManager.TicksGame;
        if (mutatedParts == null)
        {
            return;
        }

        foreach (var radiationHediff in pawn.health.hediffSet.hediffs.Where(hediff => hediff is HediffRadiation))
        {
            var anotherRadiation = (HediffRadiation)radiationHediff;
            if (!mutatedParts.Contains(anotherRadiation.Part) || radiation == anotherRadiation)
            {
                continue;
            }

            anotherRadiation.normal -= def.mutateThreshold.min * (1f - ratio);
            anotherRadiation.rare -= def.mutateThreshold.min * ratio;
        }
    }


    public bool IsHealthyEnoughForIrradiation(Pawn pawn)
    {
        var pawnParts = pawn.health.hediffSet.GetNotMissingParts();
        var parts = def.bodyParts.Join(pawnParts, left => left.part, right => right.def, (_, right) => right);

        foreach (var part in parts)
        {
            var health = PawnCapacityUtility.CalculatePartEfficiency(pawn.health.hediffSet, part);
            if (health < damageThreshold)
            {
                return false;
            }
        }

        return true;
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Collections.Look(ref assigned, "assigned", LookMode.Reference);
        Scribe_Values.Look(ref ticksCooldown, "ticksCooldown");
        Scribe_Values.Look(ref damageThreshold, "damageThreshold");
        Scribe_References.Look(ref currentUser, "currentUser");
        Scribe_Defs.Look(ref lastMutation, "lastMutation");
        Scribe_Values.Look(ref lastMutationTick, "lastMutationTick");
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        powerComp = GetComp<CompPowerTrader>();
        facilitiesComp = GetComp<CompAffectedByFacilities>();
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var g in base.GetGizmos())
        {
            yield return g;
        }

        if (Faction != Faction.OfPlayer)
        {
            yield break;
        }

        yield return new Command_Action
        {
            defaultLabel = "ChamberTestRunLabel".Translate(),
            defaultDesc = "ChamberTestRunDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("Radiology/Icons/TestRun"),
            //disabled = ticksCooldown > 0,
            Disabled = ticksCooldown > 0,
            action = delegate { Irradiate(null, 60); },
            hotKey = KeyBindingDefOf.Misc3
        };

        yield return new Command_Action
        {
            defaultLabel = "CommandRadiologyAssignChamberLabel".Translate(),
            defaultDesc = "CommandRadiologyAssignChamberDesc".Translate(),
            icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
            action = delegate
            {
                Find.WindowStack.Add(new DialogSelectMultiple<Pawn>(this)
                {
                    LabelSelected = "ChamberDialogAssigned",
                    LabelNotSelected = "ChamberDialogNotAssigned"
                });
            },
            hotKey = KeyBindingDefOf.Misc3
        };


        if (Prefs.DevMode && currentUser is { Spawned: true })
        {
            yield return new Command_Action
            {
                defaultLabel = $"Dev: mutate {currentUser.Name}",
                action = delegate { SpawnMutation(currentUser, GetBodyPart(currentUser), 0.5f); }
            };
        }
    }

    public override string GetInspectString()
    {
        var stringBuilder = new StringBuilder().Append(base.GetInspectString());

        var problem = CanIrradiateNow();
        if (problem == null)
        {
            return stringBuilder.ToString();
        }

        if (stringBuilder.Length != 0)
        {
            stringBuilder.AppendLine();
        }

        stringBuilder.Append(string.Format("ChamberUnavailable".Translate(), problem.Translate()));

        return stringBuilder.ToString();
    }

    public class RadiationTracker
    {
        public float burn;
        public float normal;
        public float rare;

        public void Clear()
        {
            burn = 0;
            normal = 0;
            rare = 0;
        }
    }
}
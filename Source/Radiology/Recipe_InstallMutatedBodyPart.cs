using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Radiology;

public class Recipe_InstallMutatedBodyPart : Recipe_Surgery
{
    private static void Info(string text)
    {
        if (Prefs.DevMode)
        {
            Log.Message($"[Radiology][MutatedInstallRecipe] {text}");
        }
    }

    private ThingDef ExpectedMutatedPartDef()
    {
        var bodyPartDef = recipe.appliedOnFixedBodyParts?.FirstOrDefault();
        return bodyPartDef == null ? null : DefDatabase<ThingDef>.GetNamedSilentFail($"Mutated{bodyPartDef.defName}");
    }

    private Thing FindMutatedPartOnMap(ThingDef expectedDef, Pawn billDoer, Map map)
    {
        if (expectedDef == null || map == null)
        {
            return null;
        }

        return map.listerThings.ThingsOfDef(expectedDef)
            .FirstOrDefault(t => !t.Position.Fogged(map) && (billDoer == null || !t.IsForbidden(billDoer)));
    }

    public override AcceptanceReport AvailableReport(Thing thing, BodyPartRecord part = null)
    {
        var baseReport = base.AvailableReport(thing, part);
        if (!baseReport.Accepted)
        {
            return baseReport;
        }

        if (thing is not Pawn pawn)
        {
            return false;
        }

        var expectedDef = ExpectedMutatedPartDef();
        var hasItem = FindMutatedPartOnMap(expectedDef, null, pawn.MapHeld) != null;
        Info($"AvailableReport pawn={pawn.LabelShortCap} recipe={recipe.defName} expected={expectedDef?.defName ?? "null"} hasItem={hasItem}");

        return hasItem ? AcceptanceReport.WasAccepted : false;
    }

    public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
    {
        var result = MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, record =>
        {
            if (record.parent != null && !pawn.health.hediffSet.GetNotMissingParts().Contains(record.parent))
            {
                return false;
            }

            return !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) ||
                   pawn.health.hediffSet.HasDirectlyAddedPartFor(record);
        }).ToList();

        Info($"GetPartsToApplyOn pawn={pawn.LabelShortCap} recipe={recipe.defName} resultCount={result.Count}");
        return result;
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        if (billDoer == null)
        {
            return;
        }

        var expectedDef = ExpectedMutatedPartDef();
        var mutatedThing = FindMutatedPartOnMap(expectedDef, billDoer, billDoer.MapHeld ?? pawn.MapHeld);
        var comp = mutatedThing?.TryGetComp<CompHediffStorage>();

        if (comp == null)
        {
            Info($"ApplyOnPawn aborted: missing mutated item {expectedDef?.defName ?? "null"}");
            return;
        }

        if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
        {
            return;
        }

        TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
        MedicalRecipesUtility.RestorePartAndSpawnAllPreviousParts(pawn, part, billDoer.Position, billDoer.Map);
        if (IsViolationOnPawn(pawn, part, Faction.OfPlayer))
        {
            ThoughtUtility.GiveThoughtsForPawnOrganHarvested(pawn, billDoer);
            ReportViolation(pawn, billDoer, pawn.HomeFaction, -70);
        }

        for (var i = 0; i < comp.hediffs.Count; i++)
        {
            var hediff = comp.hediffs[i];
            var installedPart = comp.parts[i];
            var target = pawn.RaceProps.body.AllParts.FirstOrDefault(x =>
                x.def == installedPart && HealthHelper.IsParent(part, x));

            hediff.pawn = pawn;
            pawn.health.AddHediff(hediff, target);
        }

        if (mutatedThing.stackCount > 1)
        {
            mutatedThing.SplitOff(1).Destroy();
        }
        else
        {
            mutatedThing.Destroy();
        }
    }

    public override bool IsViolationOnPawn(Pawn pawn, BodyPartRecord part, Faction billDoerFaction)
    {
        if ((pawn.Faction == billDoerFaction || pawn.Faction == null) && !pawn.IsQuestLodger())
        {
            return false;
        }

        return HealthUtility.PartRemovalIntent(pawn, part) == BodyPartRemovalIntent.Harvest;
    }
}

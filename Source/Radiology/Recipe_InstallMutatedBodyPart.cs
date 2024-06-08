using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Radiology;

public class Recipe_InstallMutatedBodyPart : Recipe_InstallArtificialBodyPart
{
    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        var comp = ingredients.Select(x => x.TryGetComp<CompHediffStorage>()).Where(x => x != null)
            .RandomElementWithFallback();

        if (comp == null)
        {
            return;
        }

        if (billDoer == null)
        {
            return;
        }

        if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
        {
            return;
        }

        TaleRecorder.RecordTale(TaleDefOf.DidSurgery, [
            billDoer,
            pawn
        ]);
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
            var target =
                pawn.RaceProps.body.AllParts.FirstOrDefault(x =>
                    x.def == installedPart && HealthHelper.IsParent(part, x));

            hediff.pawn = pawn;
            pawn.health.AddHediff(hediff, target);
        }
    }
}
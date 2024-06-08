using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Radiology;

public class RecipeRedoDiagnosis : Recipe_Surgery
{
    public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
    {
        foreach (var cancerHediff in pawn.health.hediffSet.hediffs.Where(hediff => hediff is Cancer))
        {
            var cancer = (Cancer)cancerHediff;
            if (!cancer.diagnosed)
            {
                continue;
            }

            if (cancer.Part == null)
            {
                continue;
            }

            yield return cancer.Part;
        }
    }

    public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
    {
        var hediff = pawn.health.hediffSet.hediffs.Find(x => x is Cancer && x.Part == part && x.Visible);
        if (hediff is not Cancer cancer)
        {
            return;
        }

        var medicine = ingredients.FirstOrDefault();
        var quality = TendUtility.CalculateBaseTendQuality(billDoer, pawn, medicine?.def);

        cancer.Tended(quality, quality);
    }
}
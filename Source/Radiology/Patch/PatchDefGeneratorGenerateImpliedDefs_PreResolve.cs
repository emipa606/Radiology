using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Radiology.Patch;

public static class PatchDefGeneratorGenerateImpliedDefs_PreResolve
{
    private static readonly HashSet<string> ignoreParts = ["Brain", "Head"];

    public static void Postfix()
    {
        var installHeart = DefDatabase<RecipeDef>.GetNamed("InstallNaturalHeart");

        foreach (var part in DefDatabase<MutationDef>.AllDefs.Where(x => x.affectedParts != null)
                     .SelectMany(x => x.affectedParts).Distinct())
        {
            var def = new ThingDef
            {
                isTechHediff = true,
                category = ThingCategory.Item,
                thingClass = typeof(ThingWithComps),
                thingCategories = [ThingCategoryDefOf.BodyParts],
                graphicData = new GraphicData
                    { graphicClass = typeof(Graphic_Single), texPath = "Radiology/Items/MutatedBodyPartBox" },
                uiIconPath = "Radiology/Items/MutatedBodyPartBox",
                useHitPoints = true,
                selectable = true
            };
            def.SetStatBaseValue(StatDefOf.MaxHitPoints, 50f);
            def.SetStatBaseValue(StatDefOf.Flammability, 0.7f);
            def.SetStatBaseValue(StatDefOf.MarketValue, 500f);
            def.SetStatBaseValue(StatDefOf.Mass, 1f);
            def.SetStatBaseValue(StatDefOf.SellPriceFactor, 1.0f);
            def.altitudeLayer = AltitudeLayer.Item;
            def.tickerType = TickerType.Never;
            def.alwaysHaulable = true;
            def.rotatable = false;
            def.pathCost = 15;
            def.drawGUIOverlay = true;
            def.modContentPack = Radiology.modContentPack;
            def.tradeability = Tradeability.None;
            def.defName = $"Mutated{part.defName}";
            def.label = $"Mutated {part.label}";
            def.description = $"Mutated {part.label}";
            def.comps.Add(new CompProperties_Forbiddable());
            def.comps.Add(new CompProperties_Glower { glowColor = new ColorInt(0, 255, 0), glowRadius = 1 });
            def.comps.Add(new CompProperties { compClass = typeof(CompHediffStorage) });


            var recipe = new RecipeDef
            {
                defName = $"InstallMutated{part.defName}",
                label = $"install mutated {part.label}",
                description = $"Install a mutated {part.label}.",
                descriptionHyperlinks = [def],
                jobString = $"Installing mutated {part.label}.",
                workerClass = typeof(Recipe_InstallMutatedBodyPart),
                targetsBodyPart = true,
                dontShowIfAnyIngredientMissing = true,
                effectWorking = installHeart.effectWorking,
                soundWorking = installHeart.soundWorking,
                workSpeedStat = installHeart.workSpeedStat,
                workSkill = installHeart.workSkill,
                workSkillLearnFactor = installHeart.workSkillLearnFactor,
                workAmount = installHeart.workAmount,
                anesthetize = installHeart.anesthetize,
                deathOnFailedSurgeryChance = installHeart.deathOnFailedSurgeryChance,
                surgerySuccessChanceFactor = 1.2f,
                modContentPack = Radiology.modContentPack
            };

            var medicine = new IngredientCount();
            medicine.SetBaseCount(2);
            medicine.filter.SetAllow(RimWorld.ThingDefOf.MedicineIndustrial, true);
            recipe.ingredients.Add(medicine);

            var mutatedPart = new IngredientCount();
            mutatedPart.filter.SetAllow(def, true);
            recipe.ingredients.Add(mutatedPart);

            recipe.recipeUsers = [..installHeart.recipeUsers];
            recipe.appliedOnFixedBodyParts = [part];
            recipe.skillRequirements = [new SkillRequirement { minLevel = 8, skill = SkillDefOf.Medicine }];

            def.descriptionHyperlinks = [recipe];

            if (!ignoreParts.Contains(part.defName))
            {
                Radiology.bodyPartItems[part] = def;
            }

            Radiology.itemBodyParts[def] = part;

            DefGenerator.AddImpliedDef(def);
            DefGenerator.AddImpliedDef(recipe);
        }
    }
}
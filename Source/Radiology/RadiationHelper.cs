using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Radiology;

public static class RadiationHelper
{
    private static List<MutationDef> allMutations;

    public static List<MutationDef> Mutations
    {
        get
        {
            if (allMutations != null)
            {
                return allMutations;
            }

            allMutations = [];
            allMutations.AddRange(GenDefDatabase.GetAllDefsInDatabaseForDef(typeof(MutationDef)).Cast<MutationDef>());
            return allMutations;
        }
    }

    private static IEnumerable<BodyPartRecord> GetChildBodyParts(BodyPartRecord part)
    {
        if (part == null)
        {
            yield break;
        }

        yield return part;

        foreach (var childPart in part.GetDirectChildParts())
        {
            foreach (var x in GetChildBodyParts(childPart))
            {
                yield return x;
            }
        }
    }


    private static IEnumerable<BodyPartRecord> WhichPartsMutationIsAllowedOn(
        Pawn pawn,
        MutationDef def,
        BodyPartRecord part,
        Dictionary<string, HashSet<BodyPartDef>> excludedPartDefsForTag,
        Dictionary<string, HashSet<BodyPartRecord>> excludedPartsForTag
    )
    {
        IEnumerable<BodyPartRecord> allParts;
        if (def.affectedParts == null)
        {
            var list = new List<BodyPartRecord>();
            list.AddRange(GetChildBodyParts(part));
            allParts = list;
        }
        else
        {
            allParts = pawn.health.hediffSet.GetNotMissingParts().Where(x => def.affectedParts.Contains(x.def));
        }

        allParts = allParts.Where(x => !x.def.IsSolid(x, pawn.health.hediffSet.hediffs));

        foreach (var tag in def.exclusives)
        {
            if (def.affectsAllParts)
            {
                if (excludedPartDefsForTag.TryGetValue(tag, out var excluded))
                {
                    allParts = allParts.Where(x => !excluded.Contains(x.def));
                }
            }
            else
            {
                if (excludedPartsForTag.TryGetValue(tag, out var excluded))
                {
                    allParts = allParts.Except(excluded);
                }
            }
        }

        return allParts;
    }

    private static void info(string text)
    {
        if (Prefs.DevMode)
        {
            Log.Message($"[Radiation]: {text}");
        }
    }

    public static IEnumerable<BodyPartRecord> MutatePawn(Pawn pawn, BodyPartRecord part, float rareRatio,
        out Mutation mutationResult)
    {
        mutationResult = null;

        info($"Finding mutation for part: {part}");
        info($"Rare ratio: {rareRatio}");

        var excludedPartDefsForTag = new Dictionary<string, HashSet<BodyPartDef>>();
        var excludedPartsForTag = new Dictionary<string, HashSet<BodyPartRecord>>();
        var excludedGlobalTags = new HashSet<string>();

        foreach (var mutationHediff in pawn.health.hediffSet.hediffs.Where(hediff => hediff is Mutation))
        {
            var existingMutation = (Mutation)mutationHediff;
            foreach (var tag in existingMutation.def.exclusives)
            {
                var setDef = excludedPartDefsForTag.TryGetValue(tag);
                if (setDef == null)
                {
                    excludedPartDefsForTag[tag] = setDef = [];
                }

                setDef.Add(existingMutation.Part.def);

                var set = excludedPartsForTag.TryGetValue(tag);
                if (set == null)
                {
                    excludedPartsForTag[tag] = set = [];
                }

                set.Add(existingMutation.Part);
            }

            excludedGlobalTags.AddRange(existingMutation.def.exclusivesGlobal);
        }

        info($"Excluded parts for tags: {Debug.AsText(excludedPartsForTag)}");
        info($"Excluded defs for tags: {Debug.AsText(excludedPartDefsForTag)}");

        var mutations = Mutations.Where(x => x.relatedParts == null || x.relatedParts.Contains(part.def));

        info($"All applicable mutations: {Debug.AsText(mutations)}");

        var allowedMutations = new Dictionary<MutationDef, IEnumerable<BodyPartRecord>>();
        foreach (var mutation in mutations)
        {
            if (mutation.exclusivesGlobal.Intersect(excludedGlobalTags).Any())
            {
                continue;
            }

            var parts = WhichPartsMutationIsAllowedOn(pawn, mutation, part, excludedPartDefsForTag,
                excludedPartsForTag);
            info($"  {mutation}: to {Debug.AsText(parts)}");

            if (!parts.Any())
            {
                continue;
            }

            allowedMutations[mutation] = parts;
        }

        info($"All allowed mutations: {Debug.AsText(allowedMutations)}");
        if (allowedMutations.Count == 0)
        {
            info("No mutation possible!");
            return null;
        }

        var mutationDef =
            allowedMutations.Keys.RandomElementByWeight(x => (float)Math.Pow(x.likelihood, 1f - rareRatio));
        info($"Chose mutation: {mutationDef}");

        var applicableParts = allowedMutations[mutationDef];
        info($"Can be applied to body parts: {Debug.AsText(applicableParts)}");

        if (allowedMutations.Count == 0)
        {
            info("No matching body parts!");
            return null;
        }

        var chosenPart = applicableParts.RandomElement();
        info($"Chose part: {chosenPart}");

        var chosenParts = pawn.health.hediffSet.GetNotMissingParts()
            .Where(x => mutationDef.affectsAllParts ? x.def == chosenPart.def : x == chosenPart);

        foreach (var partToMutate in chosenParts)
        {
            info($"Adding to part: {partToMutate}");

            if (pawn.health.hediffSet.hediffs.Any(x => x is Mutation && x.def == mutationDef && x.Part == partToMutate))
            {
                info("  But it already has this mutation!");
                continue;
            }

            if (HediffMaker.MakeHediff(mutationDef, pawn, partToMutate) is not Mutation mutation)
            {
                continue;
            }

            mutationResult = mutation;
            pawn.health.AddHediff(mutation, partToMutate);
        }


        return chosenParts;
    }


    public static HediffRadiation GetHediffRadition(BodyPartRecord part, Pawn pawn)
    {
        if (part == null)
        {
            return null;
        }

        foreach (var v in pawn.health.hediffSet.hediffs.Where(radiationHediff => radiationHediff is HediffRadiation))
        {
            if (v.Part == part)
            {
                return (HediffRadiation)v;
            }
        }

        if (HediffMaker.MakeHediff(HediffDefOf.RadiologyRadiation, pawn, part) is not HediffRadiation hediff)
        {
            return null;
        }

        pawn.health.AddHediff(hediff);
        return hediff;
    }
}
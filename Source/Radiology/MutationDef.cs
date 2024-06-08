using System.Collections.Generic;
using Verse;

namespace Radiology;

public class MutationDef : HediffDef
{
    /// this decides whether all body part records with specified def are affected by mutation, or just one (ie two lungs vs one lung)
    public readonly bool affectsAllParts = false;

    public readonly int beauty = 0;

    public readonly GraphicData icon = new GraphicData();

    public readonly float likelihood = 1.0f;
    public List<BodyPartDef> affectedParts;

    public List<ThingDef> apparel;

    public string exclusive;
    public string exclusiveGlobal;
    public List<string> exclusives;
    public List<string> exclusivesGlobal;

    public List<BodyPartDef> relatedParts;

    public RadiologyEffectSpawnerDef spawnEffect;
    public RadiologyEffectSpawnerDef spawnEffectFemale;

    public HediffStage stage;

    public MutationDef()
    {
        hediffClass = typeof(Mutation);
    }

    public RadiologyEffectSpawnerDef SpawnEffect(Pawn pawn)
    {
        return pawn.gender == Gender.Female && spawnEffectFemale != null ? spawnEffectFemale : spawnEffect;
    }

    public override void PostLoad()
    {
        base.PostLoad();

        if (exclusives == null)
        {
            exclusives = [];
        }

        if (exclusive != null)
        {
            exclusives.Add(exclusive);
        }

        if (exclusivesGlobal == null)
        {
            exclusivesGlobal = [];
        }

        if (exclusiveGlobal != null)
        {
            exclusivesGlobal.Add(exclusiveGlobal);
        }

        if (stage != null)
        {
            if (stages == null)
            {
                stages = [];
            }

            stages.Add(stage);
        }

        icon.graphicClass = typeof(Graphic_Single);
        icon.texPath = $"Radiology/Mutations/{defName}";
    }
}
using System.Collections.Generic;
using Verse;

namespace Radiology;

public abstract class MutationCapacityModifier<T> : Mutation<T> where T : MutationDef
{
    private int lastTick;
    private List<PawnCapacityModifier> modifiers;

    public override HediffStage CurStage
    {
        get
        {
            if (base.CurStage == null)
            {
                return null;
            }

            var stage = def.stages[0];

            if (Find.TickManager.TicksGame < lastTick + 60 || pawn.Map == null)
            {
                return stage;
            }

            lastTick = Find.TickManager.TicksGame;

            if (modifiers == null)
            {
                modifiers = [];
                foreach (var v in stage.capMods)
                {
                    modifiers.Add(new PawnCapacityModifier { capacity = v.capacity, offset = v.offset });
                }
            }

            var m = Multiplier();

            for (var i = 0; i < modifiers.Count; i++)
            {
                if (i >= stage.capMods.Count)
                {
                    break;
                }

                if (stage.capMods[i].capacity != modifiers[i].capacity)
                {
                    Log.Error(
                        $"Non-matching capacity for MutationCapacityModifier: [{i}] {stage.capMods[i].capacity} != {modifiers[i].capacity}");
                    break;
                }

                stage.capMods[i].offset = modifiers[i].offset * m;
            }

            pawn.health.capacities.Notify_CapacityLevelsDirty();

            return stage;
        }
    }

    protected abstract float Multiplier();
}
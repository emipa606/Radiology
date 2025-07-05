using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

internal class CompMutationMonitor : ThingComp
{
    private static readonly float dx = 0f / 64;
    private static readonly float dz = -10f / 64;
    private static readonly Vector3 scale = new(45f / 64, 1, 33f / 64);
    private readonly Color colorBad = new ColorInt(178, 80, 8).ToColor;

    private readonly Color colorGood = new ColorInt(79, 187, 37).ToColor;
    private readonly Color colorInactive = new ColorInt(141, 141, 141).ToColor;
    private readonly int durationFadeoff = 60 * 3;

    private readonly int durationFlashing = 60 * 5;
    private Chamber chamber;

    private void updateValues()
    {
        chamber = parent.Linked<Chamber>();
    }

    public override string CompInspectStringExtra()
    {
        updateValues();
        if (chamber?.lastMutation == null || chamber.lastMutationTick == 0)
        {
            return "";
        }

        return "RadiologyLastMutation".Translate(chamber.lastMutation.LabelCap,
            (Find.TickManager.TicksGame - chamber.lastMutationTick).ToStringTicksToPeriod());
    }

    public override void PostDraw()
    {
        updateValues();
        if (chamber?.lastMutation == null)
        {
            return;
        }

        var vec = parent.ExactPosition();
        vec.x += -dx;
        vec.y += 1;
        vec.z += -dz;

        var elapsed = Find.TickManager.TicksGame - chamber.lastMutationTick;
        float colorAmount;

        if (elapsed < durationFlashing + durationFadeoff)
        {
            const float add = 0.5f;
            var multi = 0.5f;
            if (elapsed > durationFlashing)
            {
                var v = 1f - (1f * (elapsed - durationFlashing) / durationFlashing);
                multi *= v;
            }

            var x = (Mathf.Sin(elapsed / 6f) + 1) * 0.5f;
            colorAmount = add + (multi * x);
        }
        else
        {
            colorAmount = 0.5f;
        }

        var baseColor = chamber.lastMutation.isBad ? colorBad : colorGood;

        DrawHelper.DrawMesh(vec, 0, scale, chamber.lastMutation.icon.Graphic.MatSingle,
            DrawHelper.Mix(baseColor, colorInactive, colorAmount));
    }
}
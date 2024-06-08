using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

internal class CompMutationMonitor : ThingComp
{
    public static readonly float dx = 0f / 64;
    public static readonly float dz = -10f / 64;
    public static Vector3 scale = new Vector3(45f / 64, 1, 33f / 64);
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

        var elasped = Find.TickManager.TicksGame - chamber.lastMutationTick;
        float colorAmount;

        if (elasped < durationFlashing + durationFadeoff)
        {
            var add = 0.5f;
            var mult = 0.5f;
            if (elasped > durationFlashing)
            {
                var v = 1f - (1f * (elasped - durationFlashing) / durationFlashing);
                mult *= v;
            }

            var x = (Mathf.Sin(elasped / 6f) + 1) * 0.5f;
            colorAmount = add + (mult * x);
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
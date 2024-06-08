using UnityEngine;
using Verse;

namespace Radiology;

[StaticConstructorOnStartup]
internal class CompRadiationMonitor : ThingComp
{
    private static readonly Vector2 barSize = new Vector2(30f / 64, 12f / 64);

    public static Material unfilledMat;
    public static Material[] filledMat;

    private readonly float[] values = [0, 0, 0];

    private void updateValues()
    {
        var chamber = parent.Linked<Chamber>();
        if (chamber == null)
        {
            values[0] *= 0.9f;
            values[1] *= 0.9f;
            values[2] *= 0.9f;
            return;
        }

        var tr = chamber.radiationTracker;
        var sum = tr.burn + tr.normal + tr.rare;
        if (sum == 0)
        {
            sum = 1;
        }

        values[0] = tr.burn / sum;
        values[1] = tr.normal / sum;
        values[2] = tr.rare / sum;
    }

    public override string CompInspectStringExtra()
    {
        updateValues();

        return
            $"{string.Format("RadiologyRadiationBurn".Translate(), (int)(100 * values[0]))}\n{string.Format("RadiologyRadiationNormal".Translate(), (int)(100 * values[1]))}\n{string.Format("RadiologyRadiationRare".Translate(), (int)(100 * values[2]))}";
    }

    public override void PostDraw()
    {
        var chamber = parent.Linked<Chamber>();
        if (chamber == null)
        {
            return;
        }

        updateValues();

        if (unfilledMat == null)
        {
            unfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(107 / 255f, 107 / 255f, 107 / 255f));
            filledMat =
            [
                SolidColorMaterials.SimpleSolidColorMaterial(new Color(255 / 255f, 110 / 255f, 26 / 255f)),
                SolidColorMaterials.SimpleSolidColorMaterial(new Color(29 / 255f, 179 / 255f, 139 / 255f)),
                SolidColorMaterials.SimpleSolidColorMaterial(new Color(169 / 255f, 58 / 255f, 255 / 255f))
            ];
        }

        for (var i = 0; i < 3; i++)
        {
            var r = default(GenDraw.FillableBarRequest);
            r.center = parent.DrawPos + new Vector3((-8f + (14 * i)) / 64, 0.1f, -4f / 64);
            r.size = barSize;
            r.fillPercent = values[i];
            r.filledMat = filledMat[i];
            r.unfilledMat = unfilledMat;
            r.margin = 0;
            r.rotation = Rot4.West;
            GenDraw.DrawFillableBar(r);
        }
    }
}
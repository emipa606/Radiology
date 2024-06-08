using UnityEngine;
using Verse;

namespace Radiology;

[StaticConstructorOnStartup]
public class GizmoPsionicShieldStatus : Gizmo
{
    private static readonly int ID = 10984688;

    private static readonly Texture2D FullShieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(81f / 255, 13f / 255, 255f / 255));

    private static readonly Texture2D RegenShieldBarTex =
        SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f));

    private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);

    public MutationPsionicShield mutation;

    public GizmoPsionicShieldStatus()
    {
        Order = -200f;
    }

    public override float GetWidth(float maxWidth)
    {
        return 140f;
    }

    public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
    {
        var overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
        Find.WindowStack.ImmediateWindow(ID, overRect, WindowLayer.GameUI, delegate
        {
            var rect = overRect.AtZero().ContractedBy(6f);

            var rectLabel = rect;
            rectLabel.height = overRect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rectLabel, mutation.LabelCap);

            var rectBar = rect;
            rectBar.yMin = overRect.height / 2f;
            var fillPercent = mutation.health / Mathf.Max(1f, mutation.def.health);
            Widgets.FillableBar(rectBar, fillPercent, FullShieldBarTex, EmptyShieldBarTex, false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rectBar, $"{mutation.health:F0} / {mutation.def.health:F0}");

            var rechargeFillPercent = mutation.regenerationDelay / Mathf.Max(1f, mutation.def.regenerationDelayTicks);
            if (rechargeFillPercent > 0)
            {
                var rectRecharge = rect;
                rectRecharge.height = rectBar.height / 12f;
                rectRecharge.y = rectBar.y - ((rectBar.height - rectRecharge.height) / 2f);
                Widgets.FillableBar(rectRecharge, 1f - rechargeFillPercent, RegenShieldBarTex, EmptyShieldBarTex,
                    false);
            }

            Text.Anchor = TextAnchor.UpperLeft;
        });
        return new GizmoResult(GizmoState.Clear);
    }
}
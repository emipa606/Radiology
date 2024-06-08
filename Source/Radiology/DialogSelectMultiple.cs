using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology;

internal class DialogSelectMultiple<T> : Window
{
    private const float EntryHeight = 35f;
    private readonly ISelectMultiple<T> items;

    private Vector2 scrollPosition;

    public DialogSelectMultiple(ISelectMultiple<T> items)
    {
        this.items = items;

        doCloseButton = true;
        doCloseX = true;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
    }

    public string LabelSelected { get; set; }
    public string LabelNotSelected { get; set; }
    public int SelectedLimit { get; set; } = -1;

    public override Vector2 InitialSize => new Vector2(620f, 500f);

    public override void DoWindowContents(Rect inRect)
    {
        Text.Font = GameFont.Small;
        var outRect = new Rect(inRect);
        outRect.yMin += 20f;
        outRect.yMax -= 40f;
        outRect.width -= 16f;
        var viewRect = new Rect(0f, 0f, outRect.width - 16f, (items.All().Count() * 35f) + 100f);
        Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
        try
        {
            var num = 0f;
            var somethingSelected = false;
            var countSelected = 0;
            var selected = items.All().Where(x => items.IsSelected(x));

            foreach (var item in selected)
            {
                somethingSelected = true;
                var rect = new Rect(0f, num, viewRect.width * 0.6f, 32f);
                Widgets.Label(rect, items.Label(item));
                rect.x = rect.xMax;
                rect.width = viewRect.width * 0.4f;
                if (Widgets.ButtonText(rect, LabelSelected.Translate(), true, false))
                {
                    items.Unselect(item);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    return;
                }

                num += 35f;
                countSelected++;
            }

            if (somethingSelected)
            {
                num += 15f;
            }

            var active = SelectedLimit == -1 || countSelected < SelectedLimit;

            foreach (var item in items.All().Except(selected))
            {
                var rect2 = new Rect(0f, num, viewRect.width * 0.6f, 32f);
                Widgets.Label(rect2, items.Label(item));
                rect2.x = rect2.xMax;
                rect2.width = viewRect.width * 0.4f;
                string label = LabelNotSelected.Translate();
                if (active && Widgets.ButtonText(rect2, label, true, false))
                {
                    items.Select(item);
                    SoundDefOf.Click.PlayOneShotOnCamera();
                    break;
                }

                num += 35f;
            }
        }
        finally
        {
            Widgets.EndScrollView();
        }
    }
}
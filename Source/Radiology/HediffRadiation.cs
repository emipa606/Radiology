using Verse;

namespace Radiology;

public class HediffRadiation : HediffWithComps
{
    public float burn;
    public float normal;
    public float rare;

    public override string TipStringExtra =>
        $"{base.TipStringExtra}Burn (debug): {burn}\nNormal (debug): {normal}\nRare (debug): {rare}\n";

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref normal, "normal");
        Scribe_Values.Look(ref rare, "rare");
        Scribe_Values.Look(ref burn, "burn");
    }

    public override void Tick()
    {
        base.Tick();

        burn -= 0.001f;
        if (burn < 0)
        {
            burn = 0;
        }
    }
}
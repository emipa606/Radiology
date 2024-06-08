namespace Radiology;

public class MutationNightDwellerDef : MutationDef
{
    public float negativeMultiplier;

    public float positiveMultiplier;

    public MutationNightDwellerDef()
    {
        hediffClass = typeof(MutationNightDweller);
    }
}
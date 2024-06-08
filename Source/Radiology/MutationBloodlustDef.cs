namespace Radiology;

public class MutationBloodlustDef : MutationDef
{
    public readonly int maximumAmount = 1;
    public float aoe;

    public MutationBloodlustDef()
    {
        hediffClass = typeof(MutationBloodlust);
    }
}
using Verse;

namespace Radiology;

public class CompCarapace : ThingComp
{
    public MutationCarapace mutation;

    //public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        mutation.ApplyDamage(dinfo, out absorbed);
    }
}
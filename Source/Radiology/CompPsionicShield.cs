using System.Collections.Generic;
using Verse;

namespace Radiology;

public class CompPsionicShield : ThingComp
{
    public MutationPsionicShield mutation;

    //public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
    {
        mutation.ApplyDamage(dinfo, out absorbed);
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        if (mutation.def.health == 0)
        {
            yield break;
        }

        if (Find.Selector.SingleSelectedThing == mutation.pawn)
        {
            yield return new GizmoPsionicShieldStatus
            {
                mutation = mutation
            };
        }
    }
}
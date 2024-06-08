using Verse;

namespace Radiology;

public class MutationGlower : Mutation<MutationGlowerDef>
{
    protected override ThingComp[] GetComps()
    {
        return [new CompOrganicGlower { props = def.glow }];
    }
}
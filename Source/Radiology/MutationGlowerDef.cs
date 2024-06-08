using RimWorld;

namespace Radiology;

public class MutationGlowerDef : MutationDef
{
    public CompProperties_Glower glow;

    public MutationGlowerDef()
    {
        hediffClass = typeof(MutationGlower);
    }
}
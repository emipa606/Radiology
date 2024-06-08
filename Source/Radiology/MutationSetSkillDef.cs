using System.Collections.Generic;

namespace Radiology;

public class MutationSetSkillDef : MutationDef
{
    public List<MutationSetSkillRecord> skills;

    public MutationSetSkillDef()
    {
        hediffClass = typeof(MutationSetSkill);
    }
}
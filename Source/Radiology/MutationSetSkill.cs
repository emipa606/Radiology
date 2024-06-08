using System.Text;
using RimWorld;
using Verse;

namespace Radiology;

public class MutationSetSkill : Mutation<MutationSetSkillDef>
{
    public override string TipStringExtra
    {
        get
        {
            var builder = new StringBuilder();
            builder.Append(base.TipStringExtra);

            foreach (var v in def.skills)
            {
                if (v.add != 0)
                {
                    builder.AppendLine($"{v.skill.LabelCap}: {v.add}");
                }
                else if (v.setTo != -1)
                {
                    builder.AppendLine($"{v.skill.LabelCap}: {v.setTo}");
                }
                else
                {
                    builder.AppendLine($"{v.skill.LabelCap}: {"RadiologyTooltipSkillDisabled".Translate()}");
                }
            }

            return builder.ToString();
        }
    }

    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);

        foreach (var v in def.skills)
        {
            var rec = pawn.skills.GetSkill(v.skill);
            if (rec == null)
            {
                return;
            }

            if (v.add != 0)
            {
                rec.Level += v.add;
            }
            else if (v.setTo != -1)
            {
                rec.Level = v.setTo;
            }
            else
            {
                rec.Notify_SkillDisablesChanged();
            }
        }
    }

    public override void PostRemoved()
    {
        base.PostRemoved();

        foreach (var v in def.skills)
        {
            var rec = pawn.skills.GetSkill(v.skill);
            if (rec == null)
            {
                return;
            }

            if (v.add != 0)
            {
                rec.Level -= v.add;
            }
            else if (v.setTo != -1)
            {
                rec.Level = 0;
            }
            else
            {
                rec.Notify_SkillDisablesChanged();
            }
        }
    }

    public bool IsSkillDisabled(SkillDef skill)
    {
        var rec = def.skills.FirstOrDefault(x => x.skill == skill);
        if (rec == null)
        {
            return false;
        }

        return rec.setTo == -1 && rec.add == 0;
    }
}
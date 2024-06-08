using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Radiology;

public class Cancer : Mutation
{
    internal readonly HediffStage stage = new HediffStage();
    public List<CancerComp> actualComps = [];

    public List<CancerComp> apparentComps = [];
    public bool apparentGrowth;
    public bool diagnosed;

    private int lastUpdateTick = -1;

    public float lethalSeverity = -1f;
    private int nextUpdateTick;
    private new CancerDef def => base.def as CancerDef;

    public override HediffStage CurStage
    {
        get
        {
            UpdateStage();

            return stage;
        }
    }

    public override string LabelInBrackets => !diagnosed && !pawn.Dead
        ? "RadiologyCancerNotDiagnosed".Translate().RawText
        : apparentGrowth || pawn.Dead
            ? $"{(int)(Severity * 100)}%"
            : null;

    public override string TipStringExtra
    {
        get
        {
            var stringBuilder = new StringBuilder();

            HashSet<CancerCompDef> actualCompsMap = null;

            var isDead = pawn.Dead;
            if (isDead)
            {
                actualCompsMap = [];
                foreach (var comp in actualComps)
                {
                    actualCompsMap.Add(comp.def);
                }
            }

            if (diagnosed || isDead)
            {
                foreach (var comp in apparentComps)
                {
                    var args = comp.DescriptionArgs;
                    stringBuilder.Append("- ");
                    stringBuilder.Append(
                        args == null ? comp.def.description : string.Format(comp.def.description, args));
                    if (isDead)
                    {
                        if (actualCompsMap.Contains(comp.def))
                        {
                            actualCompsMap.Remove(comp.def);
                        }
                        else
                        {
                            stringBuilder.Append("RadiologyCancerMisdiagnosed".Translate());
                        }
                    }
                    else if (comp.doctorIsUnsure)
                    {
                        stringBuilder.Append("RadiologyCancerDoctorUnsure".Translate());
                    }

                    stringBuilder.AppendLine();
                }
            }

            if (isDead)
            {
                foreach (var comp in actualComps)
                {
                    if (!actualCompsMap.Contains(comp.def))
                    {
                        continue;
                    }

                    var args = comp.DescriptionArgs;
                    stringBuilder.Append("- ");
                    stringBuilder.Append(
                        args == null ? comp.def.description : string.Format(comp.def.description, args));
                    stringBuilder.Append("RadiologyCancerMissed".Translate());
                    stringBuilder.AppendLine();
                }
            }
            else if (diagnosed && !apparentGrowth)
            {
                stringBuilder.Append("- ");
                stringBuilder.AppendLine("RadiologyCancerNotGrowing".Translate());
            }


            return stringBuilder.ToString();
        }
    }

    public static Cancer GetCancerAt(Pawn pawn, BodyPartRecord part)
    {
        if (part == null)
        {
            return null;
        }

        return pawn.health.hediffSet.hediffs.FirstOrDefault(x => x is Cancer && x.Part == part) as Cancer;
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref diagnosed, "diagnosed");
        Scribe_Values.Look(ref apparentGrowth, "apparentGrowth");
        Scribe_Collections.Look(ref apparentComps, "apparentComps", LookMode.Deep);
        Scribe_Collections.Look(ref actualComps, "actualComps", LookMode.Deep);

        if (Scribe.mode == LoadSaveMode.LoadingVars)
        {
            InitializeComps(apparentComps);
            InitializeComps(actualComps);
        }

        UpdateStage();
    }

    public void BecomeCopyOf(Cancer other)
    {
        diagnosed = other.diagnosed;
        apparentGrowth = other.apparentGrowth;
        Severity = other.Severity;

        foreach (var comp in other.apparentComps)
        {
            apparentComps.Add(comp.CreateCopy());
        }

        foreach (var comp in other.actualComps)
        {
            actualComps.Add(comp.CreateCopy());
        }

        UpdateStage();
    }

    private void InitializeComps(List<CancerComp> list)
    {
        foreach (var comp in list)
        {
            comp.cancer = this;
        }
    }

    public CancerComp CreateComp(CancerCompDef def)
    {
        if (Activator.CreateInstance(def.compClass) is not CancerComp comp)
        {
            return null;
        }

        comp.def = def;
        comp.cancer = this;

        return comp;
    }

    public void CreateComps()
    {
        apparentGrowth = false;
        diagnosed = false;
        actualComps.Clear();
        apparentComps.Clear();

        var list = def.symptomsPossible.Select(x => x);

        var count = def.symptomsCount.Lerped(Rand.Value * Rand.Value);
        for (var i = 0; i < count; i++)
        {
            if (!list.Any())
            {
                break;
            }

            var randomElementByWeight = list.RandomElementByWeight(x => x.weight);
            var comp = CreateComp(randomElementByWeight);
            if (comp == null)
            {
                continue;
            }

            if (!comp.IsValid())
            {
                continue;
            }

            actualComps.Add(comp);

            list = list.Where(x => x.tag != randomElementByWeight.tag);
        }
    }

    public override void PostAdd(DamageInfo? dinfo)
    {
        base.PostAdd(dinfo);

        if (actualComps.Any())
        {
            return;
        }

        CreateComps();
        Severity = def.initialSeverityRange.RandomInRange;
    }

    private void UpdateStage()
    {
        var tick = Find.TickManager.TicksGame;
        if (tick < nextUpdateTick)
        {
            return;
        }

        var passed = lastUpdateTick == -1 ? 0 : tick - lastUpdateTick;
        lastUpdateTick = tick;
        nextUpdateTick = tick + 600;

        stage.capMods.Clear();
        foreach (var comp in actualComps.OfType<CancerComp>())
        {
            comp.Update(passed);
        }
    }

    public override bool TendableNow(bool ignoreTimer = false)
    {
        return !diagnosed;
    }

    public override void Tended(float quality, float maxQuality, int batchPosition = 0)
    {
        diagnosed = true;

        var apparentTags = new HashSet<string>();
        apparentComps.Clear();

        foreach (var comp in actualComps)
        {
            var score = def.diagnoseDifficulty.RandomInRange;
            var mistake = score > quality;
            var unsure = Math.Abs(score - quality) < def.diagnoseUnsureWindow;

            if (!mistake)
            {
                var copy = comp.CreateCopy();
                copy.doctorIsUnsure = unsure;

                apparentComps.Add(comp);
                apparentTags.Add(comp.def.tag);
                continue;
            }

            // 50% to diagnose wrongly, 50% to not notice
            if (!(Rand.Value < 0.5f))
            {
                continue;
            }

            var mistakenDef = def.symptomsPossible.Where(x => !apparentTags.Contains(x.tag))
                .RandomElementWithFallback();
            if (mistakenDef == null)
            {
                continue;
            }

            var mistakenComp = CreateComp(mistakenDef);
            if (mistakenComp == null)
            {
                continue;
            }

            mistakenComp.doctorIsUnsure = unsure;
            apparentComps.Add(mistakenComp);
            apparentTags.Add(mistakenComp.def.tag);
        }

        apparentGrowth = apparentTags.Contains("growthSpeed");
    }

    public override bool CauseDeathNow()
    {
        return lethalSeverity >= 0f && Severity >= lethalSeverity;
    }
}
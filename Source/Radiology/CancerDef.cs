using System.Collections.Generic;
using Verse;

namespace Radiology;

public class CancerDef : MutationDef
{
    public FloatRange diagnoseDifficulty;
    public float diagnoseUnsureWindow;
    public FloatRange initialSeverityRange;
    public IntRange symptomsCount;

    public List<CancerCompDef> symptomsPossible;

    public CancerDef()
    {
        hediffClass = typeof(Cancer);
    }
}
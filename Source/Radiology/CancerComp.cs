using System;
using Verse;

namespace Radiology;

public abstract class CancerComp : IExposable
{
    public Cancer cancer;
    public CancerCompDef def;
    public bool doctorIsUnsure;

    public virtual object[] DescriptionArgs => null;

    public void ExposeData()
    {
        Scribe_Defs.Look(ref def, "def");
        Scribe_Values.Look(ref doctorIsUnsure, "doctorIsUnsure");
    }

    public virtual bool IsValid()
    {
        return true;
    }

    public abstract void Update(int passed);

    public abstract CancerComp CreateCopy();
}

public abstract class CancerComp<T> : CancerComp where T : CancerCompDef
{
    protected new T def => base.def as T;

    public override CancerComp CreateCopy()
    {
        if (Activator.CreateInstance(def.compClass) is not CancerComp copy)
        {
            return null;
        }

        copy.def = def;
        copy.cancer = cancer;

        return copy;
    }
}
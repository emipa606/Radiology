using System;
using Verse;

namespace Radiology;

public abstract class CompFilterOperationMode : IRadiationModifier
{
    public string Label;
    protected ResearchProjectDef Prerequisite;

    public virtual bool Available => Prerequisite?.IsFinished ?? true;

    public virtual void Modify(ref RadiationInfo info)
    {
        throw new NotImplementedException();
    }
}
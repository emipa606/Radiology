using System;

namespace Verse;

public class RadiologyCancerCompDef : Def
{
    public readonly float weight = 1.0f;
    public Type compClass;
    public string tag;

    protected void Init(Type t)
    {
        compClass = t;
        tag = t.Name;
    }
}
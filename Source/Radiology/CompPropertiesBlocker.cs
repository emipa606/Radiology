using Verse;

namespace Radiology;

public class CompPropertiesBlocker : CompProperties
{
    public readonly float blockChance = 0.75f;

    public readonly int blockedBodyPartLimit = 2;

    public CompPropertiesBlocker()
    {
        compClass = typeof(CompBlocker);
    }
}
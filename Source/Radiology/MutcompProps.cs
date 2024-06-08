using Verse;

namespace Radiology;

public class MutcompProps<T> : HediffCompProperties where T : HediffComp
{
    protected MutcompProps()
    {
        compClass = typeof(T);
    }
}
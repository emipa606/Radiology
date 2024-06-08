using Verse;

namespace Radiology;

public class Mutcomp<T> : HediffComp where T : HediffCompProperties
{
    protected new T props => base.props as T;
}
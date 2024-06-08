using System.Reflection;
using Verse;

namespace Radiology;

[StaticConstructorOnStartup]
public static class RadiologyPatch
{
    static RadiologyPatch()
    {
        Radiology.harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
}
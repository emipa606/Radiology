using System.Collections.Generic;
using HarmonyLib;
using Radiology.Patch;
using RimWorld;
using Verse;

namespace Radiology;

public class Radiology : Mod
{
    public static readonly Harmony harmony = new Harmony("com.github.automatic1111.radiology");
    public static ModContentPack modContentPack;
    public static readonly Dictionary<BodyPartDef, ThingDef> bodyPartItems = new Dictionary<BodyPartDef, ThingDef>();
    public static readonly Dictionary<ThingDef, BodyPartDef> itemBodyParts = new Dictionary<ThingDef, BodyPartDef>();

    public Radiology(ModContentPack pack) : base(pack)
    {
        modContentPack = pack;

        harmony.Patch(
            AccessTools.Method(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve"),
            null,
            new HarmonyMethod(typeof(PatchDefGeneratorGenerateImpliedDefs_PreResolve),
                nameof(PatchDefGeneratorGenerateImpliedDefs_PreResolve.Postfix))
        );
    }
}
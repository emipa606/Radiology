using System;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology;

public class Command_Target_Location : Command
{
    public Action<LocalTargetInfo> action;

    public TargetingParameters targetingParams;

    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
        Find.Targeter.BeginTargeting(targetingParams, delegate(LocalTargetInfo target) { action(target); }, null, null);
    }

    public override bool InheritInteractionsFrom(Gizmo other)
    {
        return false;
    }
}
using RimWorld;
using Verse;

namespace Radiology;

public class CompOrganicGlower : CompGlower
{
    private Map previousMap;
    private IntVec3 previousPosition;

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
    }

    public override void PostDeSpawn(Map map)
    {
    }

    public void Register(Map map)
    {
        //map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
        map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlagDefOf.Things);
        map.glowGrid.RegisterGlower(this);
    }

    public void Unregister(Map map)
    {
        map.glowGrid.DeRegisterGlower(this);
        if (parent != null)
        {
            //map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlag.Things);
            map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlagDefOf.Things);
        }

        if (parent != null)
        {
            //map.glowGrid.MarkGlowGridDirty(parent.Position);
            map.glowGrid.DirtyCache(parent.Position);
        }
    }

    public override void CompTick()
    {
        base.CompTick();

        if (parent == null)
        {
            return;
        }

        var map = parent.Map;

        if (previousMap != map)
        {
            if (previousMap != null)
            {
                Unregister(previousMap);
            }

            if (map != null)
            {
                Register(map);
            }

            previousMap = map;
        }

        if (map == null || previousPosition == parent.Position)
        {
            return;
        }

        //map.glowGrid.MarkGlowGridDirty(parent.Position);
        map.glowGrid.DirtyCache(parent.Position);

        previousPosition = parent.Position;
    }
}
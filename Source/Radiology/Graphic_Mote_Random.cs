using UnityEngine;
using Verse;

namespace Radiology;

[StaticConstructorOnStartup]
public class Graphic_Mote_Random : Graphic_Random
{
    protected static readonly MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

    protected virtual bool ForcePropertyBlock => false;

    public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
    {
        DrawMoteInternal(loc, rot, thingDef, thing, 0);
    }

    public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
    {
        var mote = (Mote)thing;
        var alpha = mote.Alpha;
        if (!(alpha > 0f))
        {
            return;
        }

        var instanceColor = Color * mote.instanceColor;
        instanceColor.a *= alpha;
        //var exactScale = mote.exactScale;
        var exactScale = mote.ExactScale;
        exactScale.x *= data.drawSize.x;
        exactScale.z *= data.drawSize.y;
        var matrix = default(Matrix4x4);
        matrix.SetTRS(mote.DrawPos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
        var matSingle = MaterialFor(thing);

        if (!ForcePropertyBlock && instanceColor.IndistinguishableFrom(matSingle.color))
        {
            Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, layer, null, 0);
        }
        else
        {
            propertyBlock.SetColor(ShaderPropertyIDs.Color, instanceColor);
            Graphics.DrawMesh(MeshPool.plane10, matrix, matSingle, layer, null, 0, propertyBlock);
        }
    }

    public Material MaterialFor(Thing thing)
    {
        if (thing == null)
        {
            return MatSingle;
        }

        return subGraphics[(thing.thingIDNumber < 0 ? -thing.thingIDNumber : thing.thingIDNumber) % subGraphics.Length]
            .MatSingle;
    }


    public override string ToString()
    {
        return string.Concat([
            "Mote(path=",
            path,
            ", shader=",
            Shader,
            ", color=",
            color,
            ", colorTwo=unsupported)"
        ]);
    }
}
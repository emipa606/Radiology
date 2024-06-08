using RimWorld;
using UnityEngine;
using Verse;

namespace Radiology;

public class CompPropertiesAdvancedFacility : CompProperties_Facility, IAdvancedFacilityConnector
{
    public readonly bool mustBeFaced = false;
    public readonly bool mustBeFacing = false;

    public CompPropertiesAdvancedFacility()
    {
        maxSimultaneous = 999;
    }

    public bool CanLinkTo(bool baseResult, ThingDef facilityDef, IntVec3 facilityPos, Rot4 facilityRot, ThingDef myDef,
        IntVec3 myPos, Rot4 myRot)
    {
        if (!baseResult)
        {
            return false;
        }

        GetCoordinates(myDef, myPos, myRot, out var mx1, out var mz1, out var mx2, out var mz2);

        GetCoordinates(facilityDef, facilityPos, facilityRot, out var fx1, out var fz1, out var fx2, out var fz2);

        if (!myDef.rotatable && facilityDef.rotatable)
        {
            if (facilityRot.IsHorizontal)
            {
                myRot = fx1 > mx1 ? Rot4.East : Rot4.West;
            }
            else
            {
                myRot = fz1 > mz1 ? Rot4.North : Rot4.South;
            }
        }

        Vector2 vec;
        int dx;
        int dy;
        if (mustBeFacing)
        {
            if (facilityDef.rotatable)
            {
                vec = facilityRot.AsVector2;
                dx = (int)vec.x;
                dy = (int)vec.y;

                if (dx != 0)
                {
                    if (!MathHelper.IsSameSign(dx, mx1 - fx1))
                    {
                        return false;
                    }

                    if (!MathHelper.IsBetween(fz1, mz1, mz2) && !MathHelper.IsBetween(fz2, mz1, mz2))
                    {
                        return false;
                    }
                }

                if (dy != 0)
                {
                    if (!MathHelper.IsSameSign(dy, mz1 - fz1))
                    {
                        return false;
                    }

                    if (!MathHelper.IsBetween(fx1, mx1, mx2) && !MathHelper.IsBetween(fx2, mx1, mx2))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return MathHelper.IsBetween(fz1, mz1, mz2) || MathHelper.IsBetween(fz2, mz1, mz2) ||
                       MathHelper.IsBetween(fx1, mx1, mx2) || MathHelper.IsBetween(fx2, mx1, mx2);
            }
        }

        if (!mustBeFaced)
        {
            return true;
        }

        vec = myRot.AsVector2;
        dx = (int)vec.x;
        dy = (int)vec.y;

        if (dx != 0)
        {
            if (!MathHelper.IsSameSign(dx, fx1 - mx1))
            {
                return false;
            }

            if (!MathHelper.IsBetween(fz1, mz1, mz2) && !MathHelper.IsBetween(fz2, mz1, mz2))
            {
                return false;
            }
        }

        if (dy == 0)
        {
            return true;
        }

        if (!MathHelper.IsSameSign(dy, fz1 - mz1))
        {
            return false;
        }

        return MathHelper.IsBetween(fx1, mx1, mx2) || MathHelper.IsBetween(fx2, mx1, mx2);
    }

    private static IntVec2 RotatedSize(ThingDef def, Rot4 rot)
    {
        return !rot.IsHorizontal ? def.size : new IntVec2(def.size.z, def.size.x);
    }


    private void GetCoordinates(ThingDef def, IntVec3 pos, Rot4 rot, out int x1, out int z1, out int x2, out int z2)
    {
        x1 = pos.x;
        z1 = pos.z;
        x2 = x1;
        z2 = z1;

        int w, h;
        if (rot.IsHorizontal)
        {
            w = def.size.z;
            h = def.size.x;
        }
        else
        {
            w = def.size.x;
            h = def.size.z;
        }

        int w1 = w / 2, w2 = w - w1;
        int h1 = h / 2, h2 = h - h1;


        switch (rot.AsInt)
        {
            case 0:
                x1 -= w2 - 1;
                x2 += w1;
                z1 -= h2 - 1;
                z2 += h1;
                break;
            case 1:
                x1 -= w2 - 1;
                x2 += w1;
                z1 -= h1;
                z2 += h2 - 1;
                break;
            case 2:
                x1 -= w1;
                x2 += w2 - 1;
                z1 -= h1;
                z2 += h2 - 1;
                break;
            case 3:
                x1 -= w1;
                x2 += w2 - 1;
                z1 -= h2 - 1;
                z2 += h1;
                break;
        }
    }
}
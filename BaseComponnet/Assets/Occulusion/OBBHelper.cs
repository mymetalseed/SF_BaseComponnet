using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class OBBHelper
{
    
    public static bool OverlapOBB_OBB(OBB obb1,OBB obb2)
    {
        if (ProjectionIsNotIntersect(obb1, obb2, obb1.XAxis))
        {
            return false;
        }
        if (ProjectionIsNotIntersect(obb1, obb2, obb1.YAxis))
        {
            return false;
        }
        if (ProjectionIsNotIntersect(obb1, obb2, obb1.ZAxis))
        {
            return false;
        }
        if (ProjectionIsNotIntersect(obb1, obb2, obb2.XAxis))
        {
            return false;
        }
        if (ProjectionIsNotIntersect(obb1, obb2, obb2.YAxis))
        {
            return false;
        }
        if (ProjectionIsNotIntersect(obb1, obb2, obb2.ZAxis))
        {
            return false;
        }

        return true;
    }

    public static bool ProjectionIsNotIntersect(OBB x,OBB y,Vector3 axis)
    {
        Span<float> x_p = stackalloc float[8];
        Span<float> y_p = stackalloc float[8];
        x_p[0] = GetSignProjectValue(x.P0, axis);
        x_p[1] = GetSignProjectValue(x.P1, axis);
        x_p[2] = GetSignProjectValue(x.P2, axis);
        x_p[3] = GetSignProjectValue(x.P3, axis);
        x_p[4] = GetSignProjectValue(x.P4, axis);
        x_p[5] = GetSignProjectValue(x.P5, axis);
        x_p[6] = GetSignProjectValue(x.P6, axis);
        x_p[7] = GetSignProjectValue(x.P7, axis);
        
        y_p[0] = GetSignProjectValue(y.P0, axis);
        y_p[1] = GetSignProjectValue(y.P1, axis);
        y_p[2] = GetSignProjectValue(y.P2, axis);
        y_p[3] = GetSignProjectValue(y.P3, axis);
        y_p[4] = GetSignProjectValue(y.P4, axis);
        y_p[5] = GetSignProjectValue(y.P5, axis);
        y_p[6] = GetSignProjectValue(y.P6, axis);
        y_p[7] = GetSignProjectValue(y.P7, axis);

        float xMin = x_p[0], xMax = x_p[0];
        float yMin = y_p[0], yMax = y_p[0];

        for(int i = 1; i < 8; ++i)
        {
            xMin = MathF.Min(xMin, x_p[i]);
            xMax = MathF.Max(xMax, x_p[i]);
            yMin = MathF.Min(yMin, y_p[i]);
            yMax = MathF.Max(yMax, y_p[i]);
        }

        if (yMin >= xMin && yMin <= xMax) return false;
        if (yMax >= xMin && yMax <= xMax) return false;
        if (xMin >= yMin && xMin <= yMax) return false;
        if (xMax >= yMin && xMax <= yMax) return false;

        return true;
    }

    public static float GetSignProjectValue(Vector3 point,Vector3 axis)
    {
        Vector3 projecPoint = Vector3.Project(point, axis);
        float result = projecPoint.magnitude * Mathf.Sign(Vector3.Dot(projecPoint, axis));
        return result;
    }

    public static OBB GetOBBFromMeshFilter(MeshFilter mf)
    {
        OBB obb = new OBB();
        obb.XAxis = mf.transform.right;
        obb.YAxis = mf.transform.forward;
        obb.ZAxis = mf.transform.up;

        Vector3 halfSize = mf.mesh.bounds.size/2;
        Vector4 center = new Vector4(mf.mesh.bounds.center.x, mf.mesh.bounds.center.y, mf.mesh.bounds.center.z, 0);
        Matrix4x4 modelMat = mf.transform.localToWorldMatrix;

        obb.P0 = modelMat * (center + new Vector4(halfSize.x, halfSize.y, halfSize.z,1.0f));
        obb.P1 = modelMat * (center + new Vector4(-halfSize.x, halfSize.y, halfSize.z, 1.0f));
        obb.P2 = modelMat * (center + new Vector4(halfSize.x, -halfSize.y, halfSize.z, 1.0f));
        obb.P3 = modelMat * (center + new Vector4(halfSize.x, halfSize.y, -halfSize.z, 1.0f));
        obb.P4 = modelMat * (center + new Vector4(-halfSize.x, -halfSize.y, halfSize.z, 1.0f));
        obb.P5 = modelMat * (center + new Vector4(-halfSize.x, halfSize.y, -halfSize.z, 1.0f));
        obb.P6 = modelMat * (center + new Vector4(-halfSize.x, -halfSize.y, -halfSize.z, 1.0f));
        obb.P7 = modelMat * (center + new Vector4(halfSize.x, -halfSize.y, -halfSize.z, 1.0f));

        return obb;
    }

    public static Bounds GetAABBFromMeshFilter(MeshFilter mf)
    {
        Bounds bounds = new Bounds();
        Vector4 ori = new Vector4(
            mf.mesh.bounds.center.x,
            mf.mesh.bounds.center.y,
            mf.mesh.bounds.center.z,
            1.0f
        );
        bounds.center = mf.transform.localToWorldMatrix * ori;
        bounds.size = Vector3.Scale(mf.mesh.bounds.size, mf.transform.lossyScale);
        return bounds;
    }

    public static OBB GetOBBFromAABB(Bounds bounds)
    {
        OBB obb = new OBB();
        obb.XAxis = Vector3.right;
        obb.YAxis = Vector3.forward;
        obb.ZAxis = Vector3.up;

        Vector3 halfSize = bounds.size / 2;

        obb.P0 = new Vector4(bounds.center.x + halfSize.x ,bounds.center.y +  halfSize.y ,bounds.center.z +  halfSize.z, 1.0f);
        obb.P1 = new Vector4(bounds.center.x + -halfSize.x,bounds.center.y +  halfSize.y ,bounds.center.z +  halfSize.z, 1.0f);
        obb.P2 = new Vector4(bounds.center.x + halfSize.x ,bounds.center.y +  -halfSize.y,bounds.center.z +  halfSize.z, 1.0f);
        obb.P3 = new Vector4(bounds.center.x + halfSize.x ,bounds.center.y +  halfSize.y ,bounds.center.z +  -halfSize.z, 1.0f);
        obb.P4 = new Vector4(bounds.center.x + -halfSize.x,bounds.center.y +  -halfSize.y,bounds.center.z +  halfSize.z, 1.0f);
        obb.P5 = new Vector4(bounds.center.x + -halfSize.x,bounds.center.y +  halfSize.y ,bounds.center.z +  -halfSize.z, 1.0f);
        obb.P6 = new Vector4(bounds.center.x + -halfSize.x,bounds.center.y +  -halfSize.y,bounds.center.z +  -halfSize.z, 1.0f);
        obb.P7 = new Vector4(bounds.center.x + halfSize.x ,bounds.center.y + -halfSize.y ,bounds.center.z + -halfSize.z, 1.0f);

        return obb;
    }

    public static bool OverlapAABB_OBB(Bounds bounds,OBB obb)
    {
        OBB other = GetOBBFromAABB(bounds);
        return OverlapOBB_OBB(other, obb);
    }

    public static bool OverlapAABB_AABB(Bounds bounds,Bounds bounds2)
    {
        return bounds.Intersects(bounds2);
    }

}

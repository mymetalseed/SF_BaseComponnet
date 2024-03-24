using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoundsType
{
    AABB=1,
    OBB=2
}

public class MeshTest : MonoBehaviour
{
    public MeshFilter mf;

    List<Vector4> eightPoint;

    public bool collision = false;
    public BoundsType bt = BoundsType.OBB;

    private void Awake()
    {
        mf = GetComponent<MeshFilter>();
        eightPoint = new List<Vector4>();

        Bounds center = mf.mesh.bounds;
        Vector3 sz = center.size/2;
        Vector4 ct = new Vector4(center.center.x, center.center.y, center.center.z, 0.0f);

        eightPoint.Add(ct + new Vector4(sz.x, sz.y, sz.z,1.0f));
        eightPoint.Add(ct + new Vector4(sz.x, -sz.y, sz.z, 1.0f));
        eightPoint.Add(ct + new Vector4(sz.x, sz.y, -sz.z, 1.0f));
        eightPoint.Add(ct + new Vector4(sz.x, -sz.y, -sz.z, 1.0f));
        eightPoint.Add(ct + new Vector4(-sz.x, sz.y, sz.z, 1.0f));
        eightPoint.Add(ct + new Vector4(-sz.x, -sz.y, sz.z, 1.0f));
        eightPoint.Add(ct + new Vector4(-sz.x, -sz.y, -sz.z, 1.0f));
        eightPoint.Add(ct + new Vector4(-sz.x, sz.y, -sz.z, 1.0f));

        MeshCollisionManager.Instance.Registe(this);
        collision = false;
    }

    private void Update()
    {
        //Debug.Log(mf.mesh.bounds.ToString());
    }

    private void OnDrawGizmos()
    {
        if (eightPoint != null && mf != null && Application.isPlaying)
        {
            Matrix4x4 modelMatrix = mf.transform.localToWorldMatrix;
            Gizmos.color = collision ? Color.green : Color.red;
            for (int i = 0; i < eightPoint.Count; ++i)
            {
                Gizmos.DrawSphere(modelMatrix*eightPoint[i], 0.2f);
            }

        }
    }
}

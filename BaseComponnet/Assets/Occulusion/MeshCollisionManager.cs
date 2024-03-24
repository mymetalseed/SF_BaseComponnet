using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class MeshCollisionManager : MonoBehaviour
{
    private static MeshCollisionManager m_instance;
    public static MeshCollisionManager Instance {
        get
        {
            if(m_instance == null)
            {
                GameObject obj = new GameObject("MeshCollisionManager");
                m_instance = obj.AddComponent<MeshCollisionManager>();
                m_instance.id = 0;
            }
            return m_instance;
        }
    }

    private int id = 0;

    HashSet<MeshTest> dict = new HashSet<MeshTest>();
    List<MeshTest> tl = new List<MeshTest>();

    public void Registe(MeshTest mf)
    {
        if (!dict.Contains(mf))
        {
            dict.Add(mf);
        }
    }

    private void Update()
    {
        collisionTest();
    }

    public void collisionTest()
    {
        tl.Clear();
        foreach(MeshTest mt in dict)
        {
            tl.Add(mt);
        }
        for(int i = 0; i < tl.Count; ++i)
        {
            bool flag = false;
            for(int j = 0; j < tl.Count; ++j)
            {
                if (i != j)
                {
                    OBB obbI = OBBHelper.GetOBBFromMeshFilter(tl[i].mf);
                    OBB obbJ = OBBHelper.GetOBBFromMeshFilter(tl[j].mf);
                    
                    if (OBBHelper.OverlapOBB_OBB(obbI, obbJ))
                    {
                        flag = true;
                        break;
                    }
                    
                }
            }
            tl[i].collision = flag;
        }
    }
}

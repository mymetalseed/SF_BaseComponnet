using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BVHData
{
    public Vector3 min;
    public Vector3 max;

    public int leftIdx;
    public int rightIdx;

    public int triangleIdx; //�������Ҷ�ӽڵ�����-1
    public int triangleCount;

    public bool IsLeaf => triangleIdx >= 0; 
}

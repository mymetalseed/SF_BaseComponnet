using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    /// <summary>
    /// �Ĳ����ڵ�����
    /// </summary>
    public interface IQuadTreeBody
    {
        Vector2 Position { get; }
        bool QuadTreeIgnore { get; }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    /// <summary>
    /// 四叉树节点数据
    /// </summary>
    public interface IQuadTreeBody
    {
        Vector2 Position { get; }
        bool QuadTreeIgnore { get; }
    }
}
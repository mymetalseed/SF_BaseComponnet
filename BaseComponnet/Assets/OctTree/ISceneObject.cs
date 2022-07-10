using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// 接入八叉树的动态显示与隐藏的物体实现接口
    /// </summary>
    public interface ISceneObject 
    {
        /// <summary>
        /// 该物体的包围盒
        /// </summary>
        Bounds Bounds { get; }

        /// <summary>
        /// 该物体进入显示区域时调用(在这里处理物体的加载或显示)
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        bool OnShow(Transform parent);

        /// <summary>
        /// 该物体离开显示区域时调用(在这里处理物体的卸载或隐藏)
        /// </summary>
        void OnHide();
    }

    public interface ISOLinkedListNode
    {
        Dictionary<uint, System.Object> GetNodes();
        LinkedListNode<T> GetLinkedListNode<T>(uint morton) where T : ISceneObject;
        void SetLinkedListNode<T>(uint morton, LinkedListNode<T> node);
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    public delegate void TriggerHandle<T>(T trigger);
    
    /// <summary>
    /// �������ӿ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISeperateTree<T> where T : ISceneObject,ISOLinkedListNode
    {
        /// <summary>
        /// ���ĸ��ڵ��Χ��
        /// </summary>
        Bounds Bounds { get; }

        /// <summary>
        /// ����������
        /// </summary>
        int MaxDepth { get; }

        void Add(T item);

        void Clear();

        bool Contains(T item);

        void Remove(T item);

        void Trigger(IDetector detector, TriggerHandle<T> handle);

#if UNITY_EDITOR
        void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj);
#endif

    }

    public struct TreeCullingCode
    {
        public int leftbottomback;
        public int leftbottomforward;
        public int lefttopback;
        public int lefttopforward;
        public int rightbottomback;
        public int rightbottomforward;
        public int righttopback;
        public int righttopforward;

        public bool IsCulled()
        {
            return (leftbottomback & leftbottomforward & lefttopback & lefttopforward
                    & rightbottomback & rightbottomforward & righttopback & righttopforward) != 0;
        }
    }
}

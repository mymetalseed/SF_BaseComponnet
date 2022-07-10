using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OctTree
{
    public enum TreeType
    {
        /// <summary>
        /// 线性八叉树
        /// </summary>
        LinearOcTree,
        /// <summary>
        /// 八叉树
        /// </summary>
        OcTree,
    }

    /// <summary>
    /// 场景物件加载器
    /// </summary>
    public class SceneObjectLoadController : MonoBehaviour
    {
        private WaitForEndOfFrame m_WaitForFrame;

        /// <summary>
        /// 当前场景资源四叉树/八叉树
        /// </summary>
        private ISeperateTree<SceneObject> m_Tree;

        /// <summary>
        /// 刷新时间
        /// </summary>
        private float m_RefreshTime;

        /// <summary>
        /// 销毁时间
        /// </summary>
        private float m_DestoryRefreshTime;

        private Vector3 m_OldRefreshPosition;
        private Vector3 m_OldDestoryRefreshPosition;

        /// <summary>
        /// 异步任务队列
        /// </summary>
        private Queue<SceneObject> m_ProcessTaskQueue;

        /// <summary>
        /// 已加载的物体列表(频繁移除与添加使用双向链表)
        /// </summary>
        private LinkedList<SceneObject> m_LoadedObjectLinkedList;

        /// <summary>
        /// 待销毁的物体列表
        /// </summary>
        private PriorityQueue<SceneObject> m_PreDestoryObjectQueue;

        private TriggerHandle<SceneObject> m_TriggerHandle;

        private bool m_IsTaskRunning;

        private bool m_IsInitialized;

        private int m_MaxCreateCount;
        private int m_MinCreateCount;
        private float m_MaxRefreshTime;
        private float m_MaxDestoryTime;
        private bool m_Asyn;

        private IDetector m_CurrentDetector;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="center">场景区域中心</param>
        /// <param name="size">场景区域大小</param>
        /// <param name="asyn">是否异步</param>
        /// <param name="maxCreateCount">最大创建数量</param>
        /// <param name="minCreateCount">最小创建数量</param>
        /// <param name="maxRefreshTime">更新区域时间间隔</param>
        /// <param name="maxDestroyTime">检查销毁时间间隔</param>
        /// <param name="treeType">树的类型</param>
        /// <param name="quadTreeDepth">四叉树深度</param>
        public void Init(Vector3 center,Vector3 size,bool asyn,int maxCreateCount, int minCreateCount, float maxRefreshTime, float maxDestroyTime, TreeType treeType, int quadTreeDepth = 5)
        {
            if (m_IsInitialized)
                return;
            switch (treeType)
            {
                case TreeType.OcTree:
                    m_Tree = new Scen
            }
        }

    }
}


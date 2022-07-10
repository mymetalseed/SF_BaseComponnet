using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// 场景树(非线性结构)
    /// </summary>
    public class SceneTree<T> : ISeperateTree<T> where T : ISceneObject, ISOLinkedListNode
    {
        /// <summary>
        /// 最大深度
        /// </summary>
        protected int m_MaxDepth;

        protected SceneTreeNode<T> m_Root;

        public Bounds Bounds
        {
            get
            {
                if (m_Root != null)
                    return m_Root.Bounds;
                return default(Bounds);
            }
        }

        public int MaxDepth
        {
            get { return m_MaxDepth; }
        }

        public SceneTree(Vector3 center,Vector3 size,int maxDepth,bool ocTree)
        {
            this.m_MaxDepth = maxDepth;
            this.m_Root = new SceneTreeNode<T>(new Bounds(center, size), 0, ocTree ? 8 : 4);
        }

        public void Add(T item)
        {
            //在0深度处插入
            m_Root.Insert(item, 0, m_MaxDepth);
        }

        public void Clear()
        {
            m_Root.Clear();
        }

        public bool Contains(T item)
        {
            return m_Root.Contains(item);
        }
#if UNITY_EDITOR
        public void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
        {
            if (m_Root != null)
                m_Root.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, m_MaxDepth);
        }
#endif
        public void Remove(T item)
        {
            m_Root.Remove(item);
        }

        /// <summary>
        /// 可视化检测(在这里是这样,可视的话调用回调函数handle)
        /// </summary>
        /// <param name="detector"></param>
        /// <param name="handle"></param>
        public void Trigger(IDetector detector, TriggerHandle<T> handle)
        {
            if (handle == null)
                return;
            if (detector.UseCameraCulling)
            {
                m_Root.Trigger(detector, handle);
            }
            else
            {
                if (detector.IsDetected(Bounds) == false)
                    return;
                m_Root.Trigger(detector, handle);
            }
        }

        public static implicit operator bool(SceneTree<T> tree)
        {
            return tree != null;
        }
    }


    public class SceneTreeNode<T> where T: ISceneObject, ISOLinkedListNode
    {

        private int m_CurrentDepth;
        private Vector3 m_HalfSize;
        private LinkedList<T> m_ObjectList;
        private SceneTreeNode<T>[] m_ChildNodes;
        private int m_ChildCount;
        private Bounds m_Bounds;

        public Bounds Bounds
        {
            get { return m_Bounds; }
        }

        /// <summary>
        /// 节点当前深度
        /// </summary>
        public int CurrentDepth
        {
            get { return m_CurrentDepth; }
        }

        /// <summary>
        /// 节点数据列表
        /// </summary>
        public LinkedList<T> ObjectList
        {
            get
            {
                return m_ObjectList;
            }
        }

        public SceneTreeNode(Bounds bounds,int depth,int childCount)
        {
            m_Bounds = bounds;
            m_CurrentDepth = depth;
            m_ObjectList = new LinkedList<T>();
            m_ChildNodes = new SceneTreeNode<T>[childCount];

            if (childCount == 8)
                m_HalfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y / 2, m_Bounds.size.z / 2);
            else
                m_HalfSize = new Vector3(m_Bounds.size.x / 2, m_Bounds.size.y, m_Bounds.size.z / 2);

            m_ChildCount = childCount;
        }

        public void Clear()
        {
            for(int i = 0; i < m_ChildNodes.Length; ++i)
            {
                if (m_ChildNodes[i] != null)
                    m_ChildNodes[i].Clear();
            }
            if (m_ObjectList != null)
                m_ObjectList.Clear();
        }

        public bool Contains(T obj)
        {
            for(int i = 0; i < m_ChildNodes.Length; ++i)
            {
                if (m_ChildNodes[i] != null && m_ChildNodes[i].Contains(obj))
                    return true;
            }
            if(m_ObjectList != null && m_ObjectList.Contains(obj))
                return true;
            return false;
        }

        public void Remove(T obj)
        {
            var node = obj.GetLinkedListNode<T>(0);
            if(node != null)
            {
                if(node.List == m_ObjectList)
                {
                    m_ObjectList.Remove(node);
                    var nodes = obj.GetNodes();
                    if (nodes != null)
                        nodes.Clear();
                    return;
                }
            }
            if(m_ChildNodes != null && m_ChildNodes.Length > 0)
            {
                for(int i = 0; i < m_ChildNodes.Length; ++i)
                {
                    if (m_ChildNodes[i] != null)
                        m_ChildNodes[i].Remove(obj);
                }
            }
        }

        /// <summary>
        /// 按深度递归插入
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="depth"></param>
        /// <param name="maxDepth"></param>
        /// <returns></returns>
        public SceneTreeNode<T> Insert(T obj,int depth,int maxDepth)
        {
            //列表里已经有了,不继续插入
            if (m_ObjectList.Contains(obj))
                return this;
            //小于最大深度才执行插入
            if (depth < maxDepth)
            {
                SceneTreeNode<T> node = GetContainerNode(obj, depth);
                if (node != null)//如果节点下面还有节点,代表不是叶子节点,继续搜索下一个深度
                    return node.Insert(obj, depth + 1, maxDepth);
            }
            //添加到双向链表的头部
            var n = m_ObjectList.AddFirst(obj);
            //实体设置节点的引用
            obj.SetLinkedListNode(0, n);
            return this;
        }

        protected SceneTreeNode<T> GetContainerNode(T obj,int depth)
        {
            SceneTreeNode<T> result = null;
            int ix = -1;
            int iz = -1;
            //判断是否是四叉树么?
            int iy = m_ChildNodes.Length == 4 ? 0 : 1;

            int nodeIndex = 0;
            //简称就是,根据八个空间的顶点进行空间遍历
            for(int i = ix; i <= 1; i += 2)
            {
                for(int k = iy; k <= 1; k += 2)
                {
                    for(int j= iz; j <= 1; j += 2)
                    {
                        ///这个东西吧...虽然是createNode,但是第二次访问的时候一定是有值的
                        result = CreateNode(ref m_ChildNodes[nodeIndex], depth,
                            m_Bounds.center + new Vector3(i * m_HalfSize.x * 0.5f, k * m_HalfSize.y * 0.5f, j * m_HalfSize.z * 0.5f),
                            m_HalfSize, obj);
                        if(result != null)
                        {
                            return result;
                        }

                        nodeIndex += 1;
                    }
                }
            }
            return null;
        }

        protected SceneTreeNode<T> CreateNode(ref SceneTreeNode<T> node,int depth,Vector3 centerPos,Vector3 size,T obj)
        {
            SceneTreeNode<T> result = null;
            //node为空,尝试创建
            if(node == null)
            {
                Bounds bounds = new Bounds(centerPos, size);
                //如果当前包围盒包含obj,则创建新节点
                if (bounds.IsBoundsContainsAnotherBounds(obj.Bounds))
                {
                    SceneTreeNode<T> newNode = new SceneTreeNode<T>(bounds, depth + 1, m_ChildNodes.Length);
                    node = newNode;
                    result = node;
                }
            //node不为空,当做检查,如果当前节点的包围盒包含obj的包围盒,则直接返回当前节点
            }else if (node.Bounds.IsBoundsContainsAnotherBounds(obj.Bounds))
            {
                result = node;
            }
            return result;
        }

        public void Trigger(IDetector detector, TriggerHandle<T> handle)
        {
            if (handle == null)
                return;
            //如果使用相机裁剪
            if (detector.UseCameraCulling)
            {
                TreeCullingCode code = new TreeCullingCode()
                {
                    leftbottomback = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.min.y, m_Bounds.min.z, true),
                    leftbottomforward = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.min.y, m_Bounds.max.z, true),
                    lefttopback = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.max.y, m_Bounds.min.z, true),
                    lefttopforward = detector.GetDetectedCode(m_Bounds.min.x, m_Bounds.max.y, m_Bounds.max.z, true),
                    rightbottomback = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.min.y, m_Bounds.min.z, true),
                    rightbottomforward = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.min.y, m_Bounds.max.z, true),
                    righttopback = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.max.y, m_Bounds.min.z, true),
                    righttopforward = detector.GetDetectedCode(m_Bounds.max.x, m_Bounds.max.y, m_Bounds.max.z, true),
                };
                TriggerByCamera(detector, handle, code);
            }
            else
            {
                //不做裁剪
                int code = detector.GetDetectedCode(m_Bounds.center.x, m_Bounds.center.y, m_Bounds.center.z, m_ChildCount == 4);
                for(int i = 0; i < m_ChildNodes.Length; ++i)
                {
                    var node = m_ChildNodes[i];
                    if(node != null && (code & (1<<i)) != 0)
                    {
                        node.Trigger(detector, handle);
                    }
                }

                {
                    var node = m_ObjectList.First;
                    while(node != null)
                    {
                        if (detector.IsDetected(node.Value.Bounds))
                            handle(node.Value);
                        node = node.Next;
                    }
                }
            }
        }

        /// <summary>
        /// 相机驱动的检测是否可视
        /// </summary>
        /// <param name="detector"></param>
        /// <param name="handle"></param>
        /// <param name="code"></param>
        private void TriggerByCamera(IDetector detector,TriggerHandle<T> handle,TreeCullingCode code)
        {
            ///检测码来自于上一级,见下面的 m_ChildNodes[0].TriggerByCamera
            if (code.IsCulled())
                return;

            var node = m_ObjectList.First;
            while (node != null)
            {
                if (detector.IsDetected(node.Value.Bounds))
                    handle(node.Value);
                node = node.Next;
            }


            float centerx = m_Bounds.center.x, centery = m_Bounds.center.y, centerz = m_Bounds.center.z;
            float sx = m_Bounds.size.x * 0.5f, sy = m_Bounds.size.y * 0.5f, sz = m_Bounds.size.z * 0.5f;
            int leftbottommiddle = detector.GetDetectedCode(centerx - sx, centery - sy, centerz, true);
            int middlebottommiddle = detector.GetDetectedCode(centerx, centery - sy, centerz, true);
            int rightbottommiddle = detector.GetDetectedCode(centerx + sx, centery - sy, centerz, true);
            int middlebottomback = detector.GetDetectedCode(centerx, centery - sy, centerz - sz, true);
            int middlebottomforward = detector.GetDetectedCode(centerx, centery - sy, centerz + sz, true);

            int lefttopmiddle = detector.GetDetectedCode(centerx - sx, centery + sy, centerz, true);
            int middletopmiddle = detector.GetDetectedCode(centerx, centery + sy, centerz, true);
            int righttopmiddle = detector.GetDetectedCode(centerx + sx, centery + sy, centerz, true);
            int middletopback = detector.GetDetectedCode(centerx, centery + sy, centerz - sz, true);
            int middletopforward = detector.GetDetectedCode(centerx, centery + sy, centerz + sz, true);
            if (m_ChildCount == 8)
            {
                int leftmiddleback = detector.GetDetectedCode(centerx - sx, centery, centerz - sz, true);
                int leftmiddlemiddle = detector.GetDetectedCode(centerx - sx, centery, centerz, true);
                int leftmiddleforward = detector.GetDetectedCode(centerx - sx, centery, centerz + sz, true);
                int middlemiddleback = detector.GetDetectedCode(centerx, centery, centerz - sz, true);
                int middlemiddlemiddle = detector.GetDetectedCode(centerx, centery, centerz, true);
                int middlemiddleforward = detector.GetDetectedCode(centerx, centery, centerz + sz, true);
                int rightmiddleback = detector.GetDetectedCode(centerx + sx, centery, centerz - sz, true);
                int rightmiddlemiddle = detector.GetDetectedCode(centerx + sx, centery, centerz, true);
                int rightmiddleforward = detector.GetDetectedCode(centerx + sx, centery, centerz + sz, true);

                if (m_ChildNodes.Length > 0 && m_ChildNodes[0] != null) m_ChildNodes[0].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = code.leftbottomback,
                    leftbottomforward = leftbottommiddle,
                    lefttopback = leftmiddleback,
                    lefttopforward = leftmiddlemiddle,
                    rightbottomback = middlebottomback,
                    rightbottomforward = middlebottommiddle,
                    righttopback = middlemiddleback,
                    righttopforward = middlemiddlemiddle,
                });
                if (m_ChildNodes.Length > 1 && m_ChildNodes[1] != null) m_ChildNodes[1].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = leftbottommiddle,
                    leftbottomforward = code.leftbottomforward,
                    lefttopback = leftmiddlemiddle,
                    lefttopforward = leftmiddleforward,
                    rightbottomback = middlebottommiddle,
                    rightbottomforward = middlebottomforward,
                    righttopback = middlemiddlemiddle,
                    righttopforward = middlemiddleforward,
                });
                if (m_ChildNodes.Length > 2 && m_ChildNodes[2] != null) m_ChildNodes[2].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = leftmiddleback,
                    leftbottomforward = leftmiddlemiddle,
                    lefttopback = code.lefttopback,
                    lefttopforward = lefttopmiddle,
                    rightbottomback = middlemiddleback,
                    rightbottomforward = middlemiddlemiddle,
                    righttopback = middletopback,
                    righttopforward = middletopmiddle,
                });
                if (m_ChildNodes.Length > 3 && m_ChildNodes[3] != null) m_ChildNodes[3].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = leftmiddlemiddle,
                    leftbottomforward = leftmiddleforward,
                    lefttopback = lefttopmiddle,
                    lefttopforward = code.lefttopforward,
                    rightbottomback = middlemiddlemiddle,
                    rightbottomforward = middlemiddleforward,
                    righttopback = middletopmiddle,
                    righttopforward = middletopforward,
                });

                if (m_ChildNodes.Length > 4 && m_ChildNodes[4] != null) m_ChildNodes[4].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = middlebottomback,
                    leftbottomforward = middlebottommiddle,
                    lefttopback = middlemiddleback,
                    lefttopforward = middlemiddlemiddle,
                    rightbottomback = code.rightbottomback,
                    rightbottomforward = rightbottommiddle,
                    righttopback = rightmiddleback,
                    righttopforward = rightmiddlemiddle,
                });
                if (m_ChildNodes.Length > 5 && m_ChildNodes[5] != null) m_ChildNodes[5].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = middlebottommiddle,
                    leftbottomforward = middlebottomforward,
                    lefttopback = middlemiddlemiddle,
                    lefttopforward = middlemiddleforward,
                    rightbottomback = rightbottommiddle,
                    rightbottomforward = code.rightbottomforward,
                    righttopback = rightmiddlemiddle,
                    righttopforward = rightmiddleforward,
                });
                if (m_ChildNodes.Length > 6 && m_ChildNodes[6] != null) m_ChildNodes[6].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = middlemiddleback,
                    leftbottomforward = middlemiddlemiddle,
                    lefttopback = middletopback,
                    lefttopforward = middletopmiddle,
                    rightbottomback = rightmiddleback,
                    rightbottomforward = rightmiddlemiddle,
                    righttopback = code.righttopback,
                    righttopforward = righttopmiddle,
                });
                if (m_ChildNodes.Length > 7 && m_ChildNodes[7] != null) m_ChildNodes[7].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = middlemiddlemiddle,
                    leftbottomforward = middlemiddleforward,
                    lefttopback = middletopmiddle,
                    lefttopforward = middletopforward,
                    rightbottomback = rightmiddlemiddle,
                    rightbottomforward = rightmiddleforward,
                    righttopback = righttopmiddle,
                    righttopforward = code.righttopforward,
                });
            }
            else
            {
                if (m_ChildNodes.Length > 0 && m_ChildNodes[0] != null) m_ChildNodes[0].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = code.leftbottomback,
                    leftbottomforward = leftbottommiddle,
                    lefttopback = code.lefttopback,
                    lefttopforward = lefttopmiddle,
                    rightbottomback = middlebottomback,
                    rightbottomforward = middlebottommiddle,
                    righttopback = middletopback,
                    righttopforward = middletopmiddle,
                });
                if (m_ChildNodes.Length > 1 && m_ChildNodes[1] != null) m_ChildNodes[1].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = leftbottommiddle,
                    leftbottomforward = code.leftbottomforward,
                    lefttopback = lefttopmiddle,
                    lefttopforward = code.lefttopforward,
                    rightbottomback = middlebottommiddle,
                    rightbottomforward = middlebottomforward,
                    righttopback = middletopmiddle,
                    righttopforward = middletopforward,
                });
                if (m_ChildNodes.Length > 2 && m_ChildNodes[2] != null) m_ChildNodes[2].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = middlebottomback,
                    leftbottomforward = middlebottommiddle,
                    lefttopback = middletopback,
                    lefttopforward = middletopmiddle,
                    rightbottomback = code.rightbottomback,
                    rightbottomforward = rightbottommiddle,
                    righttopback = code.righttopback,
                    righttopforward = righttopmiddle,
                });
                if (m_ChildNodes.Length > 3 && m_ChildNodes[3] != null) m_ChildNodes[3].TriggerByCamera(detector, handle, new TreeCullingCode()
                {
                    leftbottomback = middlebottommiddle,
                    leftbottomforward = middlebottomforward,
                    lefttopback = middletopmiddle,
                    lefttopforward = middletopforward,
                    rightbottomback = rightbottommiddle,
                    rightbottomforward = code.rightbottomforward,
                    righttopback = righttopmiddle,
                    righttopforward = code.righttopforward,
                });
            }
        }

#if UNITY_EDITOR
        public void DrawNode(Color treeMinDepthColor,Color treeMaxDepthColor,Color objColor,Color hitObjColor,int drawMinDepth,int drawMaxDepth,bool drawObj,int maxDepth)
        {
            //绘制子节点
            if(m_ChildNodes != null)
            {
                for(int i = 0; i < m_ChildNodes.Length; ++i)
                {
                    var node = m_ChildNodes[i];
                    if (node != null)
                        node.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, maxDepth);
                }
            }
            //绘制当前节点
            if(m_CurrentDepth >= drawMinDepth && m_CurrentDepth <= drawMaxDepth)
            {
                float d = ((float)m_CurrentDepth) / maxDepth;
                Color color = Color.Lerp(treeMinDepthColor, treeMaxDepthColor, d);

                m_Bounds.DrawBounds(color);
            }

            //如果有需要绘制的Object,则绘制Object
            if (drawObj)
            {
                var node = m_ObjectList.First;
                while(node != null)
                {
                    var sceneobj = node.Value as SceneObject;
                    if (sceneobj != null)
                        sceneobj.DrawArea(objColor, hitObjColor);
                    node = node.Next;
                }
            }
        }
#endif
    }


}


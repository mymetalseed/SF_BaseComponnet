using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// ���������������װʵ�����ڶ�̬���ص����壩
    /// </summary>
    public class SceneObject : ISceneObject, ISOLinkedListNode
    {
        /// <summary>
        /// �������崴�����
        /// </summary>
        public enum CreateFlag
        {
            /// <summary>
            /// δ����
            /// </summary>
            None,
            /// <summary>
            /// ���Ϊ������
            /// </summary>
            New,
            /// <summary>
            /// ���Ϊ������
            /// </summary>
            Old,
            /// <summary>
            /// ���Ϊ�뿪��Ұ����
            /// </summary>
            OutofBounds,
        }

        /// <summary>
        /// ����������ر��
        /// </summary>
        public enum CreatingProcessFlag
        {
            None,
            /// <summary>
            /// ׼������
            /// </summary>
            IsPrepareCreate,
            /// <summary>
            /// ׼������
            /// </summary>
            IsPrepareDestroy,
        }

        private ISceneObject m_TargetObj;
        private float m_Weight;

        private Dictionary<uint, System.Object> m_Nodes;


        public SceneObject(ISceneObject obj)
        {
            m_Weight = 0;
            m_TargetObj = obj;
        }

        /// <summary>
        /// ��������İ�Χ��
        /// </summary>
        public Bounds Bounds
        {
            get { return m_TargetObj.Bounds; }
        }

        public float Weight
        {
            get { return m_Weight; }
            set { m_Weight = value; }
        }

        /// <summary>
        /// ����װ��ʵ�����ڶ�̬���غ����ٵĳ�������
        /// </summary>
        public ISceneObject TargetObj
        {
            get { return m_TargetObj; }
        }

        public CreateFlag Flag { get; set; }

        public CreatingProcessFlag ProcessFlag { get; set; }
        
        

        public LinkedListNode<T> GetLinkedListNode<T>(uint morton) where T : ISceneObject
        {
            if (m_Nodes != null && m_Nodes.ContainsKey(morton))
                return (LinkedListNode<T>)m_Nodes[morton];
            return null;
        }

        public void SetLinkedListNode<T>(uint morton, LinkedListNode<T> node)
        {
            if (m_Nodes == null)
                m_Nodes = new Dictionary<uint, object>();
            m_Nodes[morton] = node;
        }

        public Dictionary<uint, object> GetNodes()
        {
            return m_Nodes;
        }

        public void OnHide()
        {
            Weight = 0;
            m_TargetObj.OnHide();
        }

        public bool OnShow(Transform parent)
        {
            return m_TargetObj.OnShow(parent);
        }
#if UNITY_EDITOR
        public void DrawArea(Color color, Color hitColor)
        {
            if (Flag == CreateFlag.New || Flag == CreateFlag.Old)
            {
                m_TargetObj.Bounds.DrawBounds(hitColor);
            }
            else
                m_TargetObj.Bounds.DrawBounds(color);
        }
#endif
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace OctTree
{
    public enum TreeType
    {
        /// <summary>
        /// ���԰˲���
        /// </summary>
        LinearOcTree,
        /// <summary>
        /// �˲���
        /// </summary>
        OcTree,
    }

    /// <summary>
    /// �������������
    /// </summary>
    public class SceneObjectLoadController : MonoBehaviour
    {
        private WaitForEndOfFrame m_WaitForFrame;

        /// <summary>
        /// ��ǰ������Դ�Ĳ���/�˲���
        /// </summary>
        private ISeperateTree<SceneObject> m_Tree;

        /// <summary>
        /// ˢ��ʱ��
        /// </summary>
        private float m_RefreshTime;

        /// <summary>
        /// ����ʱ��
        /// </summary>
        private float m_DestoryRefreshTime;

        private Vector3 m_OldRefreshPosition;
        private Vector3 m_OldDestoryRefreshPosition;

        /// <summary>
        /// �첽�������
        /// </summary>
        private Queue<SceneObject> m_ProcessTaskQueue;

        /// <summary>
        /// �Ѽ��ص������б�(Ƶ���Ƴ������ʹ��˫������)
        /// </summary>
        private LinkedList<SceneObject> m_LoadedObjectLinkedList;

        /// <summary>
        /// �����ٵ������б�
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
        /// ��ʼ��
        /// </summary>
        /// <param name="center">������������</param>
        /// <param name="size">���������С</param>
        /// <param name="asyn">�Ƿ��첽</param>
        /// <param name="maxCreateCount">��󴴽�����</param>
        /// <param name="minCreateCount">��С��������</param>
        /// <param name="maxRefreshTime">��������ʱ����</param>
        /// <param name="maxDestroyTime">�������ʱ����</param>
        /// <param name="treeType">��������</param>
        /// <param name="quadTreeDepth">�Ĳ������</param>
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


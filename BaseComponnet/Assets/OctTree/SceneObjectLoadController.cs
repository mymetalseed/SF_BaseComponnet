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
        private float m_DestroyRefreshTime;

        private Vector3 m_OldRefreshPosition;
        private Vector3 m_OldDestroyRefreshPosition;

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
        private PriorityQueue<SceneObject> m_PreDestroyObjectQueue;

        private TriggerHandle<SceneObject> m_TriggerHandle;

        private bool m_IsTaskRunning;

        private bool m_IsInitialized;

        private int m_MaxCreateCount;
        private int m_MinCreateCount;
        private float m_MaxRefreshTime;
        private float m_MaxDestroyTime;
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
                    m_Tree = new SceneTree<SceneObject>(center, size, quadTreeDepth, true);
                    break;
                default:
                    m_Tree = new SceneTree<SceneObject>(center, size, quadTreeDepth,true);
                    break;
            }

            m_LoadedObjectLinkedList = new LinkedList<SceneObject>();
            m_PreDestroyObjectQueue = new PriorityQueue<SceneObject>();
            m_TriggerHandle = new TriggerHandle<SceneObject>(this.TriggerHandle);

            m_MaxCreateCount = Mathf.Max(0, maxCreateCount);
            m_MinCreateCount = Mathf.Clamp(minCreateCount, 0, m_MaxCreateCount);
            m_MaxRefreshTime = maxRefreshTime;
            m_MaxDestroyTime = maxDestroyTime;
            m_Asyn = asyn;

            m_IsInitialized = true;

            m_RefreshTime = maxRefreshTime;
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="data"></param>
        void TriggerHandle(SceneObject data)
        {
            if (data == null)
                return;
            //������������������Ѿ�����������Ϊ������,��ȷ�����ᱻɾ��
            if (data.Flag == SceneObject.CreateFlag.Old)
            {
                data.Weight++;
                data.Flag = SceneObject.CreateFlag.New;
            //������������������Ѿ������Ϊ��������,��Ӵ�ɾ���б��Ƴ�������,�����Ϊ������
            }else if(data.Flag == SceneObject.CreateFlag.OutofBounds)
            {
                data.Flag = SceneObject.CreateFlag.New;
                m_LoadedObjectLinkedList.AddFirst(data);
            //�����������������δ�����򴴽������岢�����Ѽ��ص������б�
            }else if(data.Flag == SceneObject.CreateFlag.None)
            {
                DoCreateInternal(data);
            }
        }

        /// <summary>
        /// ִ�д�������
        /// </summary>
        /// <param name="data"></param>
        private void DoCreateInternal(SceneObject data)
        {
            m_LoadedObjectLinkedList.AddFirst(data);
            CreateObject(data,m_Asyn);
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="asyn"></param>
        private void CreateObject(SceneObject obj,bool asyn)
        {
            if (obj == null)
                return;
            if (obj.TargetObj == null)
                return;
            if(obj.Flag == SceneObject.CreateFlag.None)
            {
                if (!asyn)
                {
                    CreateObjectSync(obj);
                }
                else
                    ProcessObjectAsyn(obj, true);
                obj.Flag = SceneObject.CreateFlag.New;//��������������ΪNew
            }
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="center">������������</param>
        /// <param name="size">���������С</param>
        /// <param name="asyn">�Ƿ��첽</param>
        public void Init(Vector3 center,Vector3 size,bool asyn,TreeType treeType)
        {
            Init(center, size, asyn, 25, 15, 1, 5, treeType);
        }

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="center">������������</param>
        /// <param name="size">���������С</param>
        /// <param name="asyn">�Ƿ��첽</param>
        /// <param name="maxCreateCount">��������ʱ����</param>
        /// <param name="minCreateCount">�������ʱ����</param>
        public void Init(Vector3 center, Vector3 size, bool asyn, int maxCreateCount, int minCreateCount, TreeType treeType)
        {
            Init(center, size, asyn, maxCreateCount, minCreateCount, 1, 5, treeType);
        }

        /// <summary>
        /// ��ӳ�������
        /// </summary>
        /// <param name="obj"></param>
        public void AddSceneBlockObject(ISceneObject obj)
        {
            if (!m_IsInitialized)
                return;
            if (m_Tree == null)
                return;
            if (obj == null)
                return;
            //ʹ��SceneObject��װ
            SceneObject sbobj = new SceneObject(obj);
            m_Tree.Add(sbobj);
            //���������������,ֱ�������Ƿ���Դ���,������Դ���,�򴴽�����(�����ʽ����ΪҪ���òü�)
            if(m_CurrentDetector != null && m_CurrentDetector.IsDetected(sbobj.Bounds))
            {
                DoCreateInternal(sbobj);
            }
        }

        /// <summary>
        /// ˢ�´�����
        /// </summary>
        /// <param name="detector"></param>
        public void RefreshDetector(IDetector detector)
        {
            if (!m_IsInitialized)
                return;
            //ֻ�����귢���ı�ʱ�ŵ���
            if(m_OldRefreshPosition != detector.Position)
            {
                m_RefreshTime += Time.deltaTime;
                //�ﵽˢ��ʱ���ˢ��,�����������Ƶ��
                if(m_RefreshTime > m_MaxRefreshTime)
                {
                    m_OldRefreshPosition = detector.Position;
                    m_RefreshTime = 0;
                    m_CurrentDetector = detector;
                    //���д������
                    m_Tree.Trigger(detector, m_TriggerHandle);
                    //��ǳ������������
                    MarkOutofBoundsObjs();
                }
            }
            if(m_OldDestroyRefreshPosition != detector.Position)
            {
                if (m_PreDestroyObjectQueue != null && m_PreDestroyObjectQueue.Count >= m_MaxCreateCount && m_PreDestroyObjectQueue.Count > m_MinCreateCount)
                //if (m_PreDestroyObjectList != null && m_PreDestroyObjectList.Count >= m_MaxCreateCount)
                {
                    m_DestroyRefreshTime += Time.deltaTime;
                    if (m_DestroyRefreshTime > m_MaxDestroyTime)
                    {
                        m_OldDestroyRefreshPosition = detector.Position;
                        m_DestroyRefreshTime = 0;
                        //ɾ���������������
                        DestroyOutOfBoundsObjs();
                    }
                }
            }
        }

        /// <summary>
        /// ����뿪��Ұ������
        /// </summary>
        void MarkOutofBoundsObjs()
        {
            if (m_LoadedObjectLinkedList == null)
                return;

            var node = m_LoadedObjectLinkedList.First;
            while (node != null)
            {
                var obj = node.Value;
                if (obj.Flag == SceneObject.CreateFlag.Old)//�Ѽ�����������ȻΪOld��˵��������û�н��봥�����򣬼���������������
                {
                    obj.Flag = SceneObject.CreateFlag.OutofBounds;
                    if (m_MinCreateCount == 0)//�����С������Ϊ0ֱ��ɾ��
                    {
                        DestroyObject(obj, m_Asyn);
                    }
                    else
                    {
                        m_PreDestroyObjectQueue.Push(obj);//�����ɾ������
                    }

                    var next = node.Next;
                    m_LoadedObjectLinkedList.Remove(node);
                    node = next;
                }
                else
                {
                    obj.Flag = SceneObject.CreateFlag.Old;
                    node = node.Next;
                }
            }
        }

        /// <summary>
        /// ɾ�����������������
        /// </summary>
        void DestroyOutOfBoundsObjs()
        {
            while (m_PreDestroyObjectQueue.Count > m_MinCreateCount)
            {

                var obj = m_PreDestroyObjectQueue.Pop();
                if (obj == null)
                    continue;
                if (obj.Flag == SceneObject.CreateFlag.OutofBounds)
                {
                    DestroyObject(obj, m_Asyn);
                }
            }
        }
        private void OnDestroy()
        {
            if (m_Tree != null)
                m_Tree.Clear();
            m_Tree = null;
            if (m_ProcessTaskQueue != null)
                m_ProcessTaskQueue.Clear();
            if (m_LoadedObjectLinkedList != null)
                m_LoadedObjectLinkedList.Clear();
            m_ProcessTaskQueue = null;
            m_LoadedObjectLinkedList = null;
            m_TriggerHandle = null;
        }
        #region ���������������

        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="asyn"></param>
        private void DestroyObject(SceneObject obj,bool asyn)
        {
            if (obj == null)
                return;
            if (obj.Flag == SceneObject.CreateFlag.None)
                return;
            if (obj.TargetObj == null)
                return;
            if (!asyn)
                DestroyObjectSync(obj);
            else
                ProcessObjectAsyn(obj, false);
            obj.Flag = SceneObject.CreateFlag.None;//��ɾ����������ΪNone
        }

        private void CreateObjectSync(SceneObject obj)
        {
            if(obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
            {
                obj.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                return;
            }
            obj.OnShow(transform);//ִ��OnShow
        }

        private void DestroyObjectSync(SceneObject obj)
        {
            if(obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
            {
                obj.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                return;
            }
            obj.OnHide();//ִ��OnHide()
        }

        /// <summary>
        /// �첽����
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="create"></param>
        private void ProcessObjectAsyn(SceneObject obj,bool create)
        {
            if (create)
            {
                //��ʾ�����Ѿ��������ȴ����٣�������ΪNone������
                if (obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
                {
                    obj.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                    return;
                }
                //�Ѿ���ʼ�ȴ�����,������
                if (obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
                    return;
                //����Ϊ�ȴ���ʼ����
                obj.ProcessFlag = SceneObject.CreatingProcessFlag.IsPrepareCreate;
            }
            else
            {
                if(obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
                {
                    obj.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                    return;
                }
                if (obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)
                    return;
                obj.ProcessFlag = SceneObject.CreatingProcessFlag.IsPrepareDestroy;
            }
            if (m_ProcessTaskQueue == null)
                m_ProcessTaskQueue = new Queue<SceneObject>();
            m_ProcessTaskQueue.Enqueue(obj);//����
            if (!m_IsTaskRunning)
            {
                StartCoroutine(AsynTaskProcess());//��ʼЭ��ִ���첽����
            }
        }

        /// <summary>
        /// �첽����
        /// </summary>
        /// <returns></returns>
        private IEnumerator AsynTaskProcess()
        {
            if (m_ProcessTaskQueue == null)
                yield return 0;
            m_IsTaskRunning = true;
            while(m_ProcessTaskQueue.Count > 0)
            {
                var obj = m_ProcessTaskQueue.Dequeue();
                if(obj != null)
                {
                    if(obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareCreate)
                    {
                        obj.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                        if (obj.OnShow(transform))
                        {
                            if (m_WaitForFrame == null)
                                m_WaitForFrame = new WaitForEndOfFrame();
                            yield return m_WaitForFrame;
                        }
                    }
                    else if (obj.ProcessFlag == SceneObject.CreatingProcessFlag.IsPrepareDestroy)//�ȴ�����
                    {
                        obj.ProcessFlag = SceneObject.CreatingProcessFlag.None;
                        obj.OnHide();
                        if (m_WaitForFrame == null)
                            m_WaitForFrame = new WaitForEndOfFrame();
                        yield return m_WaitForFrame;
                    }
                }
            }
            m_IsTaskRunning = false;
        }
        private class SceneObjectWeightComparer : IComparer<SceneObject>
        {

            public int Compare(SceneObject x, SceneObject y)
            {
                if (y.Weight < x.Weight)
                    return 1;
                else if (y.Weight == x.Weight)
                    return 0;
                return -1;
            }
        }
        #endregion
#if UNITY_EDITOR
        public int debug_DrawMinDepth = 0;
        public int debug_DrawMaxDepth = 5;
        public bool debug_DrawObj = true;
        void OnDrawGizmosSelected()
        {
            Color mindcolor = new Color32(0, 66, 255, 255);
            Color maxdcolor = new Color32(133, 165, 255, 255);
            Color objcolor = new Color32(0, 210, 255, 255);
            Color hitcolor = new Color32(255, 216, 0, 255);
            if (m_Tree != null)
                m_Tree.DrawTree(mindcolor, maxdcolor, objcolor, hitcolor, debug_DrawMinDepth, debug_DrawMaxDepth, debug_DrawObj);
        }
#endif
    }
}


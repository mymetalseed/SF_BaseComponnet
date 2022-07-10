using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// ����˲����Ķ�̬��ʾ�����ص�����ʵ�ֽӿ�
    /// </summary>
    public interface ISceneObject 
    {
        /// <summary>
        /// ������İ�Χ��
        /// </summary>
        Bounds Bounds { get; }

        /// <summary>
        /// �����������ʾ����ʱ����(�����ﴦ������ļ��ػ���ʾ)
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        bool OnShow(Transform parent);

        /// <summary>
        /// �������뿪��ʾ����ʱ����(�����ﴦ�������ж�ػ�����)
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


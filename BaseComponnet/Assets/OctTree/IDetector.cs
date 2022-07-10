using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// ������ӿ�,���ڼ��ͳ�������Ĵ���
    /// </summary>
    public interface IDetector 
    {
        /// <summary>
        /// �Ƿ�ʹ������ü�
        /// </summary>
        bool UseCameraCulling { get; }

        /// <summary>
        /// �Ƿ���ɹ�
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        bool IsDetected(Bounds bounds);

        /// <summary>
        /// ����ĳ��������������ײ����
        /// �˲�������
        /// �²㣺 |2|32|    �ϲ㣺|8|128|  
        ///        |1|16|          |4|64 |
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="ignoreY"></param>
        /// <returns></returns>
        int GetDetectedCode(float x, float y, float z, bool ignoreY);

        /// <summary>
        /// ������λ��
        /// </summary>
        Vector3 Position { get; }
    }

}


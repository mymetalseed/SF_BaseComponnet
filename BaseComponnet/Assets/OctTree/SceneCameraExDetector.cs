using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    public class SceneCameraExDetector : SceneCameraDetector
    {
        #region �ü�������չ���Ʋ���
        /// <summary>
        /// ˮƽ������չ���룬������������ƶ�ʱ�Ĳü�������չ
        /// </summary>
        public float horizontalExtDis;
        /// <summary>
        /// ����������չ���룬�������ǰ�ƶ�ʱ�Ĳü�������չ
        /// </summary>
        public float topExtDis;
        /// <summary>
        /// �ײ�������չ���룬����������ƶ�ʱ�Ĳü�������չ
        /// </summary>
        public float bottomExtDis;
        #endregion

        private Vector3 m_Position;

        private float m_LeftEx;
        private float m_RightEx;
        private float m_UpEx;
        private float m_DownEx;

        void Start()
        {
            m_Camera = gameObject.GetComponent<Camera>();
            m_Position = transform.position;
            //m_Codes = new int[27];
        }

        void Update()
        {
            Vector3 movedir = -transform.worldToLocalMatrix.MultiplyPoint(m_Position);
            m_Position = transform.position;

            m_LeftEx = movedir.x < -Mathf.Epsilon ? -horizontalExtDis : 0;
            m_RightEx = movedir.x > Mathf.Epsilon ? horizontalExtDis : 0;
            m_UpEx = movedir.y > Mathf.Epsilon ? topExtDis : 0;
            m_DownEx = movedir.y < -Mathf.Epsilon ? -bottomExtDis : 0;
        }

        public override bool IsDetected(Bounds bounds)
        {
            if (m_Camera == null)
                return false;

            return bounds.IsBoundsInCameraEx(m_Camera, m_LeftEx, m_RightEx, m_DownEx, m_UpEx);
        }

        protected override int CaculateCullCode(Vector4 position, Matrix4x4 matrix)
        {
            return MatrixEx.ComputeOutCodeEx(position, matrix, m_LeftEx, m_RightEx, m_DownEx, m_UpEx);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Camera camera = gameObject.GetComponent<Camera>();
            if (camera)
                GizmosEx.DrawViewFrustumEx(camera, m_LeftEx, m_RightEx, m_DownEx, m_UpEx, Color.yellow);
        }
#endif
    }
}


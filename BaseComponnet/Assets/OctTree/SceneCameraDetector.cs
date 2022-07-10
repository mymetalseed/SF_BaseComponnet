using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// 该触发器根据相机裁剪区域触发
    /// </summary>
    public class SceneCameraDetector : SceneDetectorBase
    {
        protected Camera m_Camera;

        public override bool UseCameraCulling
        {
            get { return true; }
        }

        private void Start()
        {
            m_Camera = gameObject.GetComponent<Camera>();
        }

        public override int GetDetectedCode(float x, float y, float z, bool ignoreY)
        {
            if (m_Camera == null)
                return 0;
            Matrix4x4 matrix = m_Camera.cullingMatrix;
            return CaculateCullCode(new Vector4(x, y, z, 1.0f), matrix);
        }

        public override bool IsDetected(Bounds bounds)
        {
            if (m_Camera == null)
                return false;
            return bounds.IsBoundsInCamera(m_Camera);
        }

        protected virtual int CaculateCullCode(Vector4 position,Matrix4x4 matrix)
        {
            return MatrixEx.ComputeOutCode(position, matrix);
        }
    }
}

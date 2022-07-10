using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    public static class BoundsEx
    {
        /// <summary>
        /// ���ư�Χ��
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="color"></param>
        public static void DrawBounds(this Bounds bounds, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// �жϰ�Χ���Ƿ�����ü�
        /// ӳ�䵽��βü��ռ��ڼ����򵥺ܶ�
        /// ������Ҫ����һ�ξ���˷�
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsBoundsInCamera(this Bounds bounds, Camera camera)
        {
            Matrix4x4 matrix = camera.projectionMatrix * camera.worldToCameraMatrix;

            int code = MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x / 2,
                bounds.center.y + bounds.size.y / 2, bounds.center.z + bounds.size.z / 2, 1), matrix);
            
            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix);

            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix);

            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix);

            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix);

            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix);

            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix);

            code &=
                MatrixEx.ComputeOutCode(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix);

            if (code != 0) return false;

            return true;
        }

        public static bool IsBoundsInCameraEx(this Bounds bounds,Camera camera,float leftex,float rightex,float downex,
            float upex)
        {

            Matrix4x4 matrix = camera.projectionMatrix * camera.worldToCameraMatrix;

            int code =
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);


            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);

            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);

            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z + bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);

            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);

            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y + bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);

            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x + bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);

            code &=
                MatrixEx.ComputeOutCodeEx(new Vector4(bounds.center.x - bounds.size.x / 2, bounds.center.y - bounds.size.y / 2,
                    bounds.center.z - bounds.size.z / 2, 1), matrix, leftex, rightex, downex, upex);


            if (code != 0) return false;

            return true;
        }


        /// <summary>
        /// �жϰ�Χ���Ƿ������һ����Χ��
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool IsBoundsContainsAnotherBounds(this Bounds bounds, Bounds compareTo)
        {
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x / 2, compareTo.size.y / 2, -compareTo.size.z / 2)))
                return false;
            if (
                !bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, compareTo.size.y / 2, -compareTo.size.z / 2)))
                return false;
            if (!bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, compareTo.size.y / 2, compareTo.size.z / 2)))
                return false;
            if (
                !bounds.Contains(compareTo.center + new Vector3(-compareTo.size.x / 2, compareTo.size.y / 2, compareTo.size.z / 2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x / 2, -compareTo.size.y / 2, -compareTo.size.z / 2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(compareTo.size.x / 2, -compareTo.size.y / 2, -compareTo.size.z / 2)))
                return false;
            if (
                !bounds.Contains(compareTo.center + new Vector3(compareTo.size.x / 2, -compareTo.size.y / 2, compareTo.size.z / 2)))
                return false;
            if (
                !bounds.Contains(compareTo.center +
                                 new Vector3(-compareTo.size.x / 2, -compareTo.size.y / 2, compareTo.size.z / 2)))
                return false;
            return true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    /// <summary>
    /// 检测器接口,用于检测和场景物件的触发
    /// </summary>
    public interface IDetector 
    {
        /// <summary>
        /// 是否使用相机裁剪
        /// </summary>
        bool UseCameraCulling { get; }

        /// <summary>
        /// 是否检测成功
        /// </summary>
        /// <param name="bounds"></param>
        /// <returns></returns>
        bool IsDetected(Bounds bounds);

        /// <summary>
        /// 计算某坐标与检测器的碰撞掩码
        /// 八叉树掩码
        /// 下层： |2|32|    上层：|8|128|  
        ///        |1|16|          |4|64 |
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="ignoreY"></param>
        /// <returns></returns>
        int GetDetectedCode(float x, float y, float z, bool ignoreY);

        /// <summary>
        /// 触发器位置
        /// </summary>
        Vector3 Position { get; }
    }

}


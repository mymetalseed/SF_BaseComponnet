using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DX12 14.6 P505 伯恩斯坦多项式实现贝塞尔曲面
/// </summary>
public class BernsteinBazier : MonoBehaviour
{
    /// <summary>
    /// 由伯恩斯坦多项式推导出的三次贝塞尔曲线多项式(参数)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector4 BernsteinBasis(float t)
    {
        float invT = 1.0f - t;
        return new Vector4(invT*invT*invT,//B0^3(t) = (1-t)^3
            3.0f * t * invT * invT,
            3.0f * t * t * invT,
            t*t*t
            );    
    }

    /// <summary>
    /// 三次伯恩斯坦导数多项式(参数)
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector4 dBernsteinBasis(float t)
    {
        float invT = 1.0f - t;
        return new Vector4(
            -3 * invT * invT,
            3 * invT * invT - 6 * t * invT,
            6 * t * invT - 3 * t * t,
            3*t*t
            );
    }

    /// <summary>
    /// 根据参数来计算三次贝塞尔多项式
    /// </summary>
    /// <param name="posPatch"></param>
    /// <param name="basisU"></param>
    /// <param name="basisV"></param>
    /// <returns></returns>
    public Vector3 CubicBezierSum(List<Vector3> posPatch,Vector4 basisU,Vector4 basisV) {

        Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
        sum = basisV.x * (basisU.x * posPatch[0] + basisU.y * posPatch[1] + basisU.z * posPatch[2] + basisU.w * posPatch[3]);
        sum += basisV.y * (basisU.x * posPatch[4] + basisU.y * posPatch[5] + basisU.z * posPatch[6] + basisU.w * posPatch[7]);
        sum += basisV.z * (basisU.x * posPatch[8] + basisU.y * posPatch[9] + basisU.z * posPatch[10] + basisU.w * posPatch[11]);
        sum += basisV.w * (basisU.x * posPatch[12] + basisU.y * posPatch[13] + basisU.z * posPatch[14] + basisU.w * posPatch[15]);
        return sum;
    }

}

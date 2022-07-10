using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OctTree
{
    public abstract class SceneDetectorBase : MonoBehaviour,IDetector
    {
        public abstract bool UseCameraCulling { get; }

        public Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        public abstract bool IsDetected(Bounds bounds);

        public abstract int GetDetectedCode(float x, float y, float z, bool ignoreY);
    }
}

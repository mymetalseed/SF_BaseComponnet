using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public class DemoPhysicsBody : MonoBehaviour, IQuadTreeBody
    {
        public Vector2 Position { get { return new Vector2(transform.position.x, transform.position.z); } }

        public bool QuadTreeIgnore { get { return false; } }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public class QuadTreeDemo : MonoBehaviour
    {
        public DemoPhysicsBody physicsBody;
        public int MaxBodies = 500;

        [Header("QuadTree Settings")]
        public Vector2 WorldSize = new Vector2(200, 200);
        public int BodiesPerNode = 6;
        public int MaxSplits = 6;

        public QuadTree _quadTree;
        private List<IQuadTreeBody> _quadTreeBodies = new List<IQuadTreeBody>();

        private void Start()
        {
            _quadTree = new QuadTree(new Rect(0, 0, WorldSize.x, WorldSize.y), BodiesPerNode, MaxSplits);
            
            for(int i = 0; i < MaxBodies; ++i)
            {
                var body = GameObject.Instantiate<DemoPhysicsBody>(physicsBody);
                body.transform.position = new Vector3(Random.Range(0, WorldSize.x), 0, Random.Range(0, WorldSize.y));
                _quadTree.AddBody(body); 
                _quadTreeBodies.Add(body);
            }
        }

        private void Update()
        {
            // refresh QuadTree each frame if bodies can move
            _quadTree.Clear();
            foreach (var b in _quadTreeBodies)
            {
                _quadTree.AddBody(b);
            }
        }

        private void OnDrawGizmos()
        {
            if (_quadTree == null) return;
            _quadTree.DrawGizmos();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QuadTree
{
    public class QuadTree
    {
        //参数
        public QuadTree _parent;
        public Rect _bounds;
        public List<IQuadTreeBody> _bodies;
        public int _maxBodiesPerNode;
        public int _maxLevel;
        public int _curLevel;
        public QuadTree _childA;
        public QuadTree _childB;
        public QuadTree _childC;
        public QuadTree _childD;
        public List<IQuadTreeBody> _entCache;

        //构造函数
        public QuadTree(Rect bounds,int maxBodiesPerNode = 6,int maxLevel = 6)
        {
            _bounds = bounds;
            _maxBodiesPerNode = maxBodiesPerNode;
            _maxLevel = maxLevel;
            _bodies = new List<IQuadTreeBody>(maxBodiesPerNode);
        }

        public QuadTree(Rect bounds,QuadTree parent)
            : this(bounds,parent._maxBodiesPerNode,parent._maxLevel)
        {
            _parent = parent;
            _curLevel = parent._curLevel + 1;
        }

        //方法
        public void AddBody(IQuadTreeBody body)
        {
            if(_childA != null)
            {
                var child = GetQuadrant(body.Position);
                child.AddBody(body);
            }
            else
            {
                _bodies.Add(body);
                if(_bodies.Count > _maxBodiesPerNode && _curLevel < _maxLevel)
                {
                    Split();
                }
            }
        }

        public List<IQuadTreeBody> GetBodies(Vector3 point,float radius)
        {
            var p = new Vector2(point.x, point.y);
            return GetBodies(p, radius);
        }

        public List<IQuadTreeBody> GetBodies(Vector2 point,float radius)
        {
            if (_entCache == null) _entCache = new List<IQuadTreeBody>(64);
            else _entCache.Clear();
            GetBodies(point, radius, _entCache);
            return _entCache;
        }

        public List<IQuadTreeBody> GetBodies(Rect rect)
        {
            if (_entCache == null) _entCache = new List<IQuadTreeBody>();
            else _entCache.Clear();
            GetBodies(rect, _entCache);
            return _entCache;
        }


        private void GetBodies(Vector2 point,float radius,List<IQuadTreeBody> bods)
        {
            if(_childA == null)
            {
                for (int i = 0; i < _bodies.Count; ++i)
                    bods.Add(_bodies[i]);
            }
            else
            {
                if (_childA.ContainsCircle(point, radius))
                    _childA.GetBodies(point, radius, bods);
                if (_childB.ContainsCircle(point, radius))
                    _childB.GetBodies(point, radius, bods);
                if (_childC.ContainsCircle(point, radius))
                    _childC.GetBodies(point,radius,bods);
                if (_childD.ContainsCircle(point, radius))
                    _childD.GetBodies(point, radius, bods);
            }
        }

        private void GetBodies(Rect rect,List<IQuadTreeBody> bods)
        {
            if(_childA == null)
            {
                for(int i = 0; i < bods.Count; ++i)
                {
                    bods.Add(_bodies[i]);
                }
            }
            else
            {
                if (_childA.ContainsRect(rect))
                    _childA.GetBodies(rect, bods);
                if (_childB.ContainsRect(rect))
                    _childB.GetBodies(rect, bods);
                if (_childC.ContainsRect(rect))
                    _childC.GetBodies(rect);
                if (_childD.ContainsRect(rect))
                    _childD.GetBodies(rect, bods);
            }
        }

        /// <summary>
        /// 是否包含在一个圆内
        /// </summary>
        /// <param name="circleCenter"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public bool ContainsCircle(Vector2 circleCenter,float radius)
        {
            var center = _bounds.center;
            var dx = Math.Abs(circleCenter.x - center.x);
            var dy = Math.Abs(circleCenter.y - center.y);
            if(dx > (_bounds.width / 2 + radius)) { return false; }
            if(dy > (_bounds.height / 2 + radius)) { return false; }
            if(dx < (_bounds.width / 2)) { return true; }
            if(dy < (_bounds.height / 2)) { return true; }
            var cornerDist = Math.Pow((dx - _bounds.width / 2), 2) + Math.Pow((dy - _bounds.height / 2), 2);
            return cornerDist <= (radius * radius);
        }

        /// <summary>
        /// 返回两个Rect是否重叠
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool ContainsRect(Rect rect)
        {
            return _bounds.Overlaps(rect);
        }

        private QuadTree GetLowestChild(Vector2 point)
        {
            var ret = this;
            while(ret != null)
            {
                var newChild = ret.GetQuadrant(point);
                if (newChild != null) ret = newChild;
                else break;
            }
            return ret;
        }

        private void Split()
        {
            var hx = _bounds.width / 2;
            var hz = _bounds.height / 2;
            var sz = new Vector2(hx, hz);

            //split a
            var aLoc = _bounds.position;
            var aRect = new Rect(aLoc, sz);
            //split b
            var bLoc = new Vector2(_bounds.position.x + hx, _bounds.position.y);
            var bRect = new Rect(bLoc, sz);
            //split c
            var cLoc = new Vector2(_bounds.position.x + hx, _bounds.position.y + hz);
            var cRect = new Rect(cLoc, sz);
            //split d
            var dLoc = new Vector2(_bounds.position.x, _bounds.position.y + hz);
            var dRect = new Rect(dLoc, sz);

            //assign QuadTrees
            _childA = QuadTreePool.GetQuadTree(aRect, this);
            _childB = QuadTreePool.GetQuadTree(bRect, this);
            _childC = QuadTreePool.GetQuadTree(cRect, this);
            _childD = QuadTreePool.GetQuadTree(dRect, this);

            for(int i = _bodies.Count - 1; i >= 0; --i)
            {
                var child = GetQuadrant(_bodies[i].Position);
                child.AddBody(_bodies[i]);
                _bodies.RemoveAt(i);
            }
        }

        /// <summary>
        /// 获取点在那个子节点上
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private QuadTree GetQuadrant(Vector2 point)
        {
            if (_childA == null) return null;
            if(point.x > _bounds.x + _bounds.width / 2)
            {
                if (point.y > _bounds.y + _bounds.height / 2) return _childC;
                else return _childB;
            }
            else
            {
                if (point.y > _bounds.y + _bounds.height / 2) return _childD;
                return _childD;
            }
        }

        /// <summary>
        /// 清空为初始状态,比如回池时会进行这个操作
        /// </summary>
        public void Clear()
        {
            QuadTreePool.PoolQuadTree(_childA);
            QuadTreePool.PoolQuadTree(_childB);
            QuadTreePool.PoolQuadTree(_childC);
            QuadTreePool.PoolQuadTree(_childD);
            _childA = null;
            _childB = null;
            _childC = null;
            _childD = null;
            _bodies.Clear();
        }

        public void DrawGizmos()
        {
            if (_childA != null) _childA.DrawGizmos();
            if (_childB != null) _childB.DrawGizmos();
            if (_childC != null) _childB.DrawGizmos();
            if (_childD != null) _childB.DrawGizmos();

            //draw rect
            Gizmos.color = Color.cyan;
            var p1 = new Vector3(_bounds.position.x, 0.1f, _bounds.position.y);
            var p2 = new Vector3(p1.x + _bounds.width,0.1f,p1.z);
            var p3 = new Vector3(p1.x + _bounds.width,0.1f,p1.z + _bounds.height);
            var p4 = new Vector3(p1.x,0.1f,p1.z + _bounds.height);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p4);
            Gizmos.DrawLine(p4, p1);
        }
    }

}


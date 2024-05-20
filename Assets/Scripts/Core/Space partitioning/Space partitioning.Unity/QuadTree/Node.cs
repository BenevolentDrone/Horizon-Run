using System;

using System.Collections.Generic;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.SpacePartitioning
{
     public class Node<TValue>
    {
        private const int MAX_DEPTH = 8;

        private readonly ILogger logger;

        private IQuadTree<TValue> tree;

        private Node<TValue> parent;

        private Node<TValue>[] children;

        private Bounds2D bounds;

        private List<ValueSpaceData<TValue>> values;
        
        //public bool IsLeaf { get { return children[0] == null; } }

        private int depth;
        
        public void UpdateBounds(Bounds2D bounds2D)
        {
            bounds = bounds2D;
        }

        public Node(
            Node<TValue>[] children,
            List<ValueSpaceData<TValue>> values,
            int depth,
            ILogger logger = null)
        {
            parent = null;

            this.children = children;

            this.values = values;

            this.depth = depth;

            this.logger = logger;
        }

        public Node<TValue> Parent { get => parent; }
        
        public Node<TValue>[] Children { get => children; }
        
        public Bounds2D Bounds { get => bounds; }
        
        public int Depth { get => depth; }
        
        public List<ValueSpaceData<TValue>> Values { get => values; }
        
        public void Initialize(
            Bounds2D bounds,
            Node<TValue> parent,
            IQuadTree<TValue> tree,
            int depth)
        {
            this.bounds = bounds;

            this.parent = parent;

            this.tree = tree;

            this.depth = depth;
        }

        public Node<TValue> GetSuitableNode(
            Bounds2D targetBounds,
            bool checkBounds = true)
        {
            if (checkBounds)
            {
                if (!bounds.Contains(targetBounds))
                {
                    if (parent == null)
                        throw new Exception(
                            logger.TryFormat<Node<TValue>>(
                                $"OUT OF ROOT NODE BOUNDS. BOUNDS: {targetBounds.ToString()} ROOT: {bounds.ToString()}"));

                    return parent.GetSuitableNode(
                        targetBounds,
                        true);
                }
            }

            var quadrant = bounds.GetQuadrant(targetBounds);

            if (quadrant == Quadrants.UNIDENTIFIED || depth == MAX_DEPTH)
                return this;

            var quadrantIndex = (int)quadrant;

            var childNode = children[quadrantIndex];

            Bounds2D childBounds = (childNode != null)
                ? childNode.bounds
                : bounds.Divide(quadrant);

            if (childBounds.Contains(targetBounds))
            {
                if (childNode == null)
                    childNode = children[quadrantIndex] = tree.AllocateNode(
                        childBounds,
                        this,
                        depth + 1);

                return childNode.GetSuitableNode(targetBounds, false);
            }

            return this;
        }

        public void TryShrink(
            Node<TValue> child = null)
        {
            if (child != null)
                for (int i = 0; i < 4; i++)
                    if (children[i] == child)
                    {
                        tree.DisposeNode(children[i]);

                        children[i] = null;

                        break;
                    }

            if (values.Count != 0)
                return;

            for (int i = 0; i < 4; i++)
            {
                if (children[i] != null)
                {
                    return;
                }
            }

            if (parent != null)
                parent.TryShrink(this);
        }

        public void CheckForIntersections(
            Bounds2D targetBounds,
            IList<TValue> result)
        {
            int valuesAmount = values.Count;

            for (int i = 0; i < valuesAmount; i++)
            {
                var valuesBounds = values[i].Bounds;

                if (valuesBounds.Intersects(targetBounds))
                    result.Add(values[i].Value);
            }

            if (depth == MAX_DEPTH)
                return;

            for (int i = 0; i < 4; i++)
            {
                if (children[i] != null && children[i].Bounds.Intersects(targetBounds))
                    children[i].CheckForIntersections(targetBounds, result);
            }
        }

        public void Dump(List<Bounds2D> boundsList, List<Bounds2D> entityBounds)
        {
            boundsList.Add(bounds);

            for (int i = 0; i < values.Count; i++)
                entityBounds.Add(values[i].Bounds);

            if (depth == MAX_DEPTH)
                return;

            for (int i = 0; i < 4; i++)
                if (children[i] != null)
                    children[i].Dump(boundsList, entityBounds);
        }
    }
}
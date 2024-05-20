using System;
using System.Collections.Generic;
using HereticalSolutions.Pools;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
    //TODO: finish
   public class Quadtree<TValue>
       : I2DPartitionedSpace<TValue>,
         IQuadTree<TValue>,
         IQuadTreeDebuggable
   {
        private readonly IRepository<TValue, ValueSpaceData<TValue>> registeredValues;

        private readonly IPool<Node<TValue>> nodePool;
       
        private readonly IPool<ValueSpaceData<TValue>> valueDataPool;

        private readonly IComparer<TValue> comparer;

        private readonly IContainsPointOfReference<Vector2> comparerWithPointOfReference;

        private readonly ILogger logger;

        private Node<TValue> root;

        private Bounds2D lookupBounds;

        public Node<TValue> Root
        {
            get { return root; }
            set { root = value; }
        }

        public Quadtree(
            IRepository<TValue, ValueSpaceData<TValue>> registeredValues,
            IPool<Node<TValue>> nodePool,
            IPool<ValueSpaceData<TValue>> valueDataPool,
            IComparer<TValue> comparer,
            Bounds2D lookupBounds,
            ILogger logger = null)
        {
            this.registeredValues = registeredValues;

            this.nodePool = nodePool;

            this.valueDataPool = valueDataPool;

            this.comparer = comparer;

            this.comparerWithPointOfReference = comparer as IContainsPointOfReference<Vector2>;

            this.lookupBounds = lookupBounds;
            
            this.logger = logger;

            if (comparer is IContainsGetPositionDelegate<TValue> comparerWithGetPositionDelegate) //Wow, didn't know 'is' supports allocating a variable, thanks Copilot
            {
                comparerWithGetPositionDelegate.GetPositionDelegate = (value) => registeredValues.Get(value).Bounds.Center;
            }
        }

        #region I2DPartitionedSpace

        #region Register

        public void Register(
            TValue value,
            float x,
            float y,
            float radius)
        {
            if (registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY ALREADY PRESENT: {value.ToString()}"));

            Bounds2D bounds = new Bounds2D(x, y, radius);
            
            Node<TValue> suitableNode = root.GetSuitableNode(bounds);

            ValueSpaceData<TValue> data = AllocateValueSpaceData(
                value,
                bounds,
                suitableNode);

            suitableNode.Values.Add(data);
            
            registeredValues.Add(
                value,
                data);
        }

        public void Register(
            TValue value,
            Vector2 position,
            float radius)
        {
            if (registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY ALREADY PRESENT: {value.ToString()}"));

            Bounds2D bounds = new Bounds2D(position, radius);

            Node<TValue> suitableNode = root.GetSuitableNode(bounds);

            ValueSpaceData<TValue> data = AllocateValueSpaceData(
                value, 
                bounds, 
                suitableNode);

            suitableNode.Values.Add(data);
            
            registeredValues.Add(value, data);
        }
        

        public void Register(
            TValue value,
            Vector2 position,
            Vector2 size)
        {
            if (registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY ALREADY PRESENT: {value.ToString()}"));

            Bounds2D bounds = new Bounds2D(position, size);

            Node<TValue> suitableNode = root.GetSuitableNode(bounds);

            ValueSpaceData<TValue> data = AllocateValueSpaceData(
                value,
                bounds,
                suitableNode);

            suitableNode.Values.Add(data);
            
            registeredValues.Add(value, data);
        }

        #endregion
        
        #region Get bounds
        
        public Bounds2D GetBounds(TValue value)
        {
            if (!registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY NOT FOUND: {value.ToString()}"));

            return registeredValues.Get(value).Bounds;
        }
        
        #endregion
        
        #region Has
        
        public bool Has(TValue value)
        {
            return registeredValues.Has(value);
        }

        #endregion       

        #region Update
        
        public void Update(
            TValue value,
            float x,
            float y)
        {
            if (!registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY NOT FOUND: {value.ToString()}"));

            ValueSpaceData<TValue> data = registeredValues.Get(value);

            data.Bounds.UpdateCenter(x, y);

            var previousNode = data.CurrentNode;

            data.CurrentNode = previousNode.GetSuitableNode(data.Bounds);

            if (data.CurrentNode != previousNode)
            {
                previousNode.Values.Remove(data);

                data.CurrentNode.Values.Add(data);

                previousNode.TryShrink();
            }
        }

        #endregion
        
        #region Remove
        
        public void Remove(TValue value)
        {
            if (EqualityComparer<TValue>.Default.Equals(value, default(TValue)))
                return;

            if (!registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY NOT FOUND: {value.ToString()}"));

            ValueSpaceData<TValue> data = registeredValues.Get(value);

            Node<TValue> currentNode = data.CurrentNode;

            currentNode.Values.Remove(data);

            currentNode.TryShrink();

            DisposeValueSpaceData(data);

            registeredValues.Remove(value);
        }

        #endregion
        
        #region All in range broad
        
        public void AllInRangeBroad(
            float x,
            float y,
            float range,
            IList<TValue> result,
            bool sorted = false)
        {
            lookupBounds.Update(x, y, range);

            root.CheckForIntersections(lookupBounds, result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = new Vector2(x, y);

                Sort(result);
            }
        }

        public void AllInRangeBroad(
            Vector2 position,
            float range,
            IList<TValue> result,
            bool sorted = false)
        {
            lookupBounds.Update(position, range);

            root.CheckForIntersections(lookupBounds, result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = position;

                Sort(result);
            }
        }
        
        public void AllInRangeBroad(
            Vector2 position,
            Vector2 size,
            IList<TValue> result,
            bool sorted = false)
        {
            lookupBounds.Update(position, size);

            root.CheckForIntersections(lookupBounds, result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = position;

                Sort(result);
            }
        }

        public void AllInRangeBroad(
            TValue value,
            float range,
            IList<TValue> result,
            bool sorted = false)
        {
            if (!registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY NOT FOUND: {value.ToString()}"));

            var valuesBounds = registeredValues.Get(value).Bounds;

            lookupBounds.Update(valuesBounds.Center, range);

            root.CheckForIntersections(lookupBounds, result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = valuesBounds.Center;

                Sort(result);
            }
        }

        #endregion

        #region All in range narrow

        public void AllInRangeNarrow(
            float x,
            float y,
            float range,
            IList<TValue> result,

            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false)
        {
            AllInRangeBroad(
                x,
                y,
                range + additionalRange,
                result,
                false);

            ExcludeEntitiesOutOfRange(
                new Vector2(x, y),
                range,
                additionalRange,
                checkAgainstCentersOnly,
                result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = new Vector2(x, y);

                Sort(result);
            }
        }

        public void AllInRangeNarrow(
            Vector2 position,
            float range,
            IList<TValue> result,

            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false)
        {
            AllInRangeBroad(
                position,
                range + additionalRange,
                result,
                false);

            ExcludeEntitiesOutOfRange(
                position,
                range,
                additionalRange,
                checkAgainstCentersOnly,
                result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = position;

                Sort(result);
            }
        }

        public void AllInRangeNarrow(
            Vector2 position,
            Vector2 size,
            IList<TValue> result,

            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false)
        {
            AllInRangeBroad(
                position,
                size,
                result,
                false);

            ExcludeEntitiesOutOfRange(
                position,
                size.magnitude / 2f,
                additionalRange,
                checkAgainstCentersOnly,
                result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = position;

                Sort(result);
            }
        }

        public void AllInRangeNarrow(
            TValue value,
            float range,
            IList<TValue> result,

            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false)
        {
            if (!registeredValues.Has(value))
                throw new Exception(
                    logger.TryFormat<Quadtree<TValue>>(
                        $"KEY NOT FOUND: {value.ToString()}"));

            var valuesBounds = registeredValues.Get(value).Bounds;

            AllInRangeBroad(
                value,
                range + additionalRange,
                result,
                false);

            ExcludeEntitiesOutOfRange(
                valuesBounds.Center,
                range,
                additionalRange,
                checkAgainstCentersOnly,
                result);

            if (sorted)
            {
                comparerWithPointOfReference.PointOfReference = valuesBounds.Center;

                Sort(result);
            }
        }

        #endregion

        #endregion

        #region IQuadTree

        public Node<TValue> AllocateNode(
            Bounds2D bounds,
            Node<TValue> parent,
            int depth)
        {
            Node<TValue> result = nodePool.Pop();

            result.Initialize(
                bounds,
                parent,
                this,
                depth);

            return result;
        }

        public void DisposeNode(Node<TValue> node)
        {
            node.Initialize(
                default(Bounds2D),
                null,
                null,
                -1);

            node.Values.Clear();

            nodePool.Push(node);
        }

        public ValueSpaceData<TValue> AllocateValueSpaceData(
            TValue value,
            Bounds2D bounds,
            Node<TValue> node)
        {
            ValueSpaceData<TValue> result = valueDataPool.Pop();

            result.Value = value;

            result.Bounds = bounds;

            result.CurrentNode = node;

            return result;
        }

        public void DisposeValueSpaceData(ValueSpaceData<TValue> valueData)
        {
            valueData.Value = default;

            valueData.Bounds = default(Bounds2D);

            valueData.CurrentNode = null;

            valueDataPool.Push(valueData);
        }

        #endregion
        
        #region IQuadTreeDebuggable

        public void Dump(List<Bounds2D> boundsList, List<Bounds2D> entityBounds)
        {
            root.Dump(boundsList, entityBounds);
        }
        
        #endregion

        private void Sort(IList<TValue> result)
        {
            switch (result)
            {
                case List<TValue> list:
                {
                    list.Sort(comparer);

                    break;
                }

                case TValue[] array:
                {
                    Array.Sort(array, comparer);

                    break;
                }

                default:
                {
                    throw new Exception(
                        logger.TryFormat<Quadtree<TValue>>(
                            $"UNSUPPORTED COLLECTION TYPE: {result.GetType().ToString()}"));
                }
            }
        }

        private void ExcludeEntitiesOutOfRange(
            Vector2 position,
            float range,
            float additionalRange,
            bool checkAgainstCentersOnly,
            IList<TValue> result)
        {
            for (int i = result.Count - 1; i >= 0; i--)
            {
                TValue value = result[i];

                //prevent values from other spaces being processed
                if (!registeredValues.Has(value))
                {
                    result.RemoveAt(i);
                    continue;
                }
                
                Bounds2D bounds = registeredValues.Get(value).Bounds;

                float distance = (position - bounds.Center).magnitude;

                float effectiveRange = range + additionalRange;

                if (!checkAgainstCentersOnly)
                {
                    effectiveRange += bounds.Size.magnitude / 2f;
                }

                if (distance > effectiveRange)
                {
                    result.RemoveAt(i);
                }
            }
        }
    }
}
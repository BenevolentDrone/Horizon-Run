using System.Collections.Generic;

using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
    public class SpatialHashedSpace<TValue>
        : I2DUniformlyPartitionedSpace<TValue>
    {
        private const int MASK_SIZE = 1;

        private const int MAX_VALUES_PER_PARTITION = 3;

        private const int MAX_DIRTY_HASHES = 500;
        
        private readonly float partitionSize;
        
        private readonly int dimensionSizeX;
        
        private readonly int dimensionSizeY;

        private readonly TValue[] partitions;

        private readonly Bounds2D bounds;

        private readonly int[] dirtyHashes;

        private int dirtyHashesCount = 0;
        
        public SpatialHashedSpace(
            float partitionSize,
            Bounds2D bounds)
        {
            this.partitionSize = partitionSize;
            
            this.bounds = bounds;

            dimensionSizeX = Mathf.CeilToInt(bounds.Size.x / partitionSize);

            dimensionSizeY = Mathf.CeilToInt(bounds.Size.y / partitionSize); 
            
            partitions = new TValue[dimensionSizeX * dimensionSizeY * MAX_VALUES_PER_PARTITION];
            
            dirtyHashes = new int[MAX_DIRTY_HASHES];
            
            dirtyHashesCount = 0;
        }
        
        #region I2DUniformlyPartitionedSpace

        #region Register

        public void Register(
            TValue value,
            Vector2 position)
        {
            if (!WithinBounds(position))
                return;
            
            int hash = GetHash(position);
            
            for (int i = 0; i < MAX_VALUES_PER_PARTITION; i++)
            {
                int hashWithOffset = hash + i;
                
                if (EqualityComparer<TValue>.Default.Equals(
                        partitions[hashWithOffset],
                        default(TValue)))
                {
                    partitions[hashWithOffset] = value;
                    
                    dirtyHashes[dirtyHashesCount++] = hashWithOffset;
                    
                    return;
                }
            }
        }

        #endregion

        #region Remove

        public void EraseAll()
        {
            for (int i = 0; i < dirtyHashesCount; i++)
            {
                partitions[dirtyHashes[i]] = default(TValue);
            }
            
            dirtyHashesCount = 0;
        }

        #endregion

        #region Intersect

        public int EntitiesIntersectingAt(
            Vector2 position,
            TValue[] result)
        {
            int resultsFound = 0;
            
            if (!WithinBounds(position))
                return resultsFound;
            
            Vector2 relativePosition = position - bounds.Min;

            int positionX = (int)(relativePosition.x / partitionSize);
            
            int positionY = (int)(relativePosition.y / partitionSize);
            
            for (int i = -MASK_SIZE; i <= MASK_SIZE; i++)
            {
                for (int j = -MASK_SIZE; j <= MASK_SIZE; j++)
                {
                    int x = positionX + i;
                    
                    int y = positionY + j;
                    
                    if (x < 0
                        || x >= dimensionSizeX
                        || y < 0
                        || y >= dimensionSizeY)
                        continue;
                    
                    int hash = (y * dimensionSizeX + x) * MAX_VALUES_PER_PARTITION;
                    
                    for (int k = 0; k < MAX_VALUES_PER_PARTITION; k++)
                    {
                        int hashWithOffset = hash + k;
                        
                        if (EqualityComparer<TValue>.Default.Equals(
                            partitions[hashWithOffset],
                            default(TValue)))
                            continue;
                        
                        result[resultsFound++] = partitions[hashWithOffset];
                        
                        if (resultsFound == result.Length)
                            return resultsFound;
                    }
                }
            }
            
            return resultsFound;
        }

        #endregion

        #endregion

        private bool WithinBounds(Vector2 position)
        {
            return position.x > bounds.Min.x
                && position.y > bounds.Min.y
                && position.x < bounds.Max.x
                && position.y < bounds.Max.y;
        }

        private int GetHash(Vector2 position)
        {
            Vector2 relativePosition = position - bounds.Min;
            
            int stride = (int)(relativePosition.y / partitionSize) * dimensionSizeX + (int)(relativePosition.x / partitionSize);
            
            return stride * MAX_VALUES_PER_PARTITION;
        }

        public List<Vector2> Dump()
        {
            List<Vector2> result = new List<Vector2>();
            
            for (int i = 0; i < dirtyHashesCount; i++)
            {
                int hash = dirtyHashes[i];
                
                int x = hash / MAX_VALUES_PER_PARTITION % dimensionSizeX;
                
                int y = hash / MAX_VALUES_PER_PARTITION / dimensionSizeX;
                
                result.Add(new Vector2(
                    bounds.Min.x + x * partitionSize,
                    bounds.Min.y + y * partitionSize)
                    + new Vector2(
                        partitionSize / 2f,
                        partitionSize / 2f));
            }
            
            return result;
        }
        
        public float DebugPartitionSize => partitionSize;
    }
}
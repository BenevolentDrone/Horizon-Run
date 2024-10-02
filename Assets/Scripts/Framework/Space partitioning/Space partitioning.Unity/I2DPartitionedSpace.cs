using System.Collections.Generic;
using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
    public interface I2DPartitionedSpace<TValue>
    {
        #region Register

        void Register(
            TValue value,
            float x,
            float y,
            float radius);
        
        void Register(
            TValue value,
            Vector2 position,
            float radius);
        
        void Register(
            TValue value,
            Vector2 position,
            Vector2 size);
        
        #endregion

        #region Get bounds
        
        Bounds2D GetBounds(TValue value);
        
        #endregion

        #region Has
        
        bool Has(TValue value);
        
        #endregion

        #region Update

        void Update(
            TValue value,
            float x,
            float y);
        
        #endregion

        #region Remove

        void Remove(TValue value);
        
        #endregion

        #region All in range broad
        
        void AllInRangeBroad(
            float x,
            float y,
            float range,
            IList<TValue> result,
            bool sorted = false);
        
        void AllInRangeBroad(
            Vector2 position,
            float range,
            IList<TValue> result,
            bool sorted = false);
        
        void AllInRangeBroad(
            Vector2 position,
            Vector2 size,
            IList<TValue> result,
            bool sorted = false);
        
        void AllInRangeBroad(
            TValue value,
            float range,
            IList<TValue> result,
            bool sorted = false);

        #endregion

        #region All in range broad

        void AllInRangeNarrow(
            float x,
            float y,
            float range,
            IList<TValue> result,
            
            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false);

        void AllInRangeNarrow(
            Vector2 position,
            float range,
            IList<TValue> result,
            
            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false);

        void AllInRangeNarrow(
            Vector2 position,
            Vector2 size,
            IList<TValue> result,
            
            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false);

        void AllInRangeNarrow(
            TValue value,
            float range,
            IList<TValue> result,
            
            float additionalRange = 0f,
            bool checkAgainstCentersOnly = false,
            bool sorted = false);

        #endregion
    }
}
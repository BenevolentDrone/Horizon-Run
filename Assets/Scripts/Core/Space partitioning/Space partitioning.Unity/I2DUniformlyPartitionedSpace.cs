using UnityEngine;

namespace HereticalSolutions.SpacePartitioning
{
    public interface I2DUniformlyPartitionedSpace<TValue>
    {
        #region Register

        void Register(
            TValue value,
            Vector2 position);
        
        #endregion

        #region Remove

        void EraseAll();
        
        #endregion

        #region Intersect
        
        int EntitiesIntersectingAt(
            Vector2 position,
            TValue[] result);

        #endregion
    }
}
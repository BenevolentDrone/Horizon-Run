using System.Collections.Generic;

namespace HereticalSolutions.Entities
{
    public interface IReadOnlyEntityManager<TEntityID, TEntity>
    {
        bool HasEntity(
            TEntityID entityID);
        
        bool TryGetEntity(
            TEntityID entityID,
            out TEntity entity);


        IEnumerable<TEntityID> AllAllocatedIDs { get; }
    }
}
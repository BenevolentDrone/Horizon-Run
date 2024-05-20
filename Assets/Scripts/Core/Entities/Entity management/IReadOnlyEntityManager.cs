using System;
using System.Collections.Generic;

namespace HereticalSolutions.Entities
{
    public interface IReadOnlyEntityManager<TEntityID, TEntity>
    {
        bool HasEntity(TEntityID entityID);
        
        TEntity GetRegistryEntity(TEntityID entityID);

        TEntity GetEntity(
            TEntityID entityID,
            string worldID);

        //EntityDescriptor<TEntityID>[] AllRegistryEntities { get; }

        IEnumerable<TEntity> AllRegistryEntities { get; }


        IEnumerable<TEntityID> AllAllocatedIDs { get; }
    }
}
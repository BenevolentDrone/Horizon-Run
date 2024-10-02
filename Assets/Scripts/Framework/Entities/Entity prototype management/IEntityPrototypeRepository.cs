using System.Collections.Generic;

namespace HereticalSolutions.Entities
{
    public interface IEntityPrototypeRepository<TPrototypeID, TEntity>
    {
        bool HasPrototype(
            TPrototypeID prototypeID);

        bool TryGetPrototype(
            TPrototypeID prototypeID,
            out TEntity prototypeEntity);

        bool TryAllocatePrototype(
            TPrototypeID prototypeID,
            out TEntity prototypeEntity);

        bool RemovePrototype(
            TPrototypeID prototypeID);

        IEnumerable<TPrototypeID> AllPrototypeIDs { get; }
    }
}
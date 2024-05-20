using System;

using System.Numerics;

namespace HereticalSolutions.Entities
{
    public interface IEventEntityBuilder<TEntity, TEntityID>
    {
        IEventEntityBuilder<TEntity, TEntityID> NewEvent(out TEntity eventEntity);

        IEventEntityBuilder<TEntity, TEntityID> HappenedAtPosition(
            TEntity eventEntity,
            Vector3 position);

        IEventEntityBuilder<TEntity, TEntityID> AddressedToEntity(
            TEntity eventEntity,
            TEntityID receiverEntity);

        IEventEntityBuilder<TEntity, TEntityID> AddressedToWorldLocalEntity(
            TEntity eventEntity,
            TEntity receiverEntity);

        IEventEntityBuilder<TEntity, TEntityID> CausedByEntity(
            TEntity eventEntity,
            TEntityID sourceEntity);

        IEventEntityBuilder<TEntity, TEntityID> CausedByWorldLocalEntity(
            TEntity eventEntity,
            TEntity sourceEntity);

        IEventEntityBuilder<TEntity, TEntityID> TargetedAtEntity(
            TEntity eventEntity,
            TEntityID targetEntity);

        IEventEntityBuilder<TEntity, TEntityID> TargetedAtWorldLocalEntity(
            TEntity eventEntity,
            TEntity targetEntity);

        IEventEntityBuilder<TEntity, TEntityID> TargetedAtPosition(
            TEntity eventEntity,
            Vector3 position);

        IEventEntityBuilder<TEntity, TEntityID> HappenedAtTime(
            TEntity eventEntity,
            long ticks);

        IEventEntityBuilder<TEntity, TEntityID> WithData<TData>(
            TEntity eventEntity,
            TData data);
    }
}
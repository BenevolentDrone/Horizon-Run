using System.Numerics;

using HereticalSolutions.Entities;

using DefaultEcs;


using TEntityID = System.Guid;

using TEntity = DefaultEcs.Entity;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    public class EventEntityBuilder
        : IEventEntityBuilder<TEntity, TEntityID>
    {
        protected readonly World eventWorld;

        public EventEntityBuilder(
            World eventWorld)
        {
            this.eventWorld = eventWorld;
        }

        public IEventEntityBuilder<TEntity, TEntityID> NewEvent(out Entity eventEntity)
        {
            eventEntity = eventWorld.CreateEntity();

            return this;
        }

        public IEventEntityBuilder<TEntity, TEntityID> HappenedAtPosition(
            Entity eventEntity,
            Vector3 position)
        {
            eventEntity
                .Set<EventPositionComponent>(
                    new EventPositionComponent
                    {
                        Position = position
                    });

            return this;
        }

        public IEventEntityBuilder<TEntity, TEntityID> AddressedToEntity(
            Entity eventEntity,
            TEntityID receiverEntity)
        {
            eventEntity
                .Set<EventReceiverEntityComponent<TEntityID>>(
                    new EventReceiverEntityComponent<TEntityID>
                    {
                        ReceiverID = receiverEntity
                    });

            return this;
        }
        
        public IEventEntityBuilder<TEntity, TEntityID> AddressedToEntity(
            Entity eventEntity,
            TEntityID receiverEntity,
            ushort[] subaddress)
        {
            eventEntity
                .Set<EventReceiverEntitySubaddressComponent>(
                    new EventReceiverEntitySubaddressComponent
                    {
                        Subaddress = subaddress
                    });

            return AddressedToEntity(
                eventEntity,
                receiverEntity);
        }

        public IEventEntityBuilder<TEntity, TEntityID> AddressedToWorldLocalEntity(
            Entity eventEntity,
            Entity receiverEntity)
        {
            eventEntity
                .Set<EventReceiverWorldLocalEntityComponent<TEntity>>(
                    new EventReceiverWorldLocalEntityComponent<TEntity>
                    {
                        Receiver = receiverEntity
                    });

            return this;
        }
        
        public IEventEntityBuilder<TEntity, TEntityID> AddressedToWorldLocalEntity(
            Entity eventEntity,
            Entity receiverEntity,
            ushort[] subaddress)
        {
            eventEntity
                .Set<EventReceiverWorldLocalEntitySubaddressComponent>(
                    new EventReceiverWorldLocalEntitySubaddressComponent
                    {
                        Subaddress = subaddress
                    });

            return AddressedToWorldLocalEntity(
                eventEntity,
                receiverEntity);
        }

        public IEventEntityBuilder<TEntity, TEntityID> CausedByEntity(
            Entity eventEntity,
            TEntityID sourceEntity)
        {
            eventEntity
                .Set<EventSourceEntityComponent<TEntityID>>(
                    new EventSourceEntityComponent<TEntityID>
                    {
                        SourceID = sourceEntity
                    });

            return this;
        }
        
        public IEventEntityBuilder<TEntity, TEntityID> CausedByEntity(
            Entity eventEntity,
            TEntityID sourceEntity,
            ushort[] subaddress)
        {
            eventEntity
                .Set<EventSourceEntitySubaddressComponent>(
                    new EventSourceEntitySubaddressComponent
                    {
                        Subaddress = subaddress
                    });

            return CausedByEntity(
                eventEntity,
                sourceEntity);
        }

        public IEventEntityBuilder<TEntity, TEntityID> CausedByWorldLocalEntity(
            Entity eventEntity,
            Entity sourceEntity)
        {
            eventEntity
                .Set<EventSourceWorldLocalEntityComponent<TEntity>>(
                    new EventSourceWorldLocalEntityComponent<TEntity>
                    {
                        Source = sourceEntity
                    });

            return this;
        }
        
        public IEventEntityBuilder<TEntity, TEntityID> CausedByWorldLocalEntity(
            Entity eventEntity,
            Entity sourceEntity,
            ushort[] subaddress)
        {
            eventEntity
                .Set<EventSourceWorldLocalEntitySubaddressComponent>(
                    new EventSourceWorldLocalEntitySubaddressComponent
                    {
                        Subaddress = subaddress
                    });

            return CausedByWorldLocalEntity(
                eventEntity,
                sourceEntity);
        }

        public IEventEntityBuilder<TEntity, TEntityID> TargetedAtEntity(
            Entity eventEntity,
            TEntityID targetEntity)
        {
            eventEntity
                .Set<EventTargetEntityComponent<TEntityID>>(
                    new EventTargetEntityComponent<TEntityID>
                    {
                        TargetID = targetEntity
                    });

            return this;
        }
        
        public IEventEntityBuilder<TEntity, TEntityID> TargetedAtEntity(
            Entity eventEntity,
            TEntityID targetEntity,
            ushort[] subaddress)
        {
            eventEntity
                .Set<EventTargetEntitySubaddressComponent>(
                    new EventTargetEntitySubaddressComponent
                    {
                        Subaddress = subaddress
                    });

            return TargetedAtEntity(
                eventEntity,
                targetEntity);
        }

        public IEventEntityBuilder<TEntity, TEntityID> TargetedAtWorldLocalEntity(
            Entity eventEntity,
            Entity targetEntity)
        {
            eventEntity
                .Set<EventTargetWorldLocalEntityComponent<TEntity>>(
                    new EventTargetWorldLocalEntityComponent<TEntity>
                    {
                        Target = targetEntity
                    });

            return this;
        }
        
        public IEventEntityBuilder<TEntity, TEntityID> TargetedAtWorldLocalEntity(
            Entity eventEntity,
            Entity targetEntity,
            ushort[] subaddress)
        {
            eventEntity
                .Set<EventTargetWorldLocalEntitySubaddressComponent>(
                    new EventTargetWorldLocalEntitySubaddressComponent
                    {
                        Subaddress = subaddress
                    });

            return TargetedAtWorldLocalEntity(
                eventEntity,
                targetEntity);
        }

        public IEventEntityBuilder<TEntity, TEntityID> TargetedAtPosition(
            Entity eventEntity,
            Vector3 position)
        {
            eventEntity
                .Set<EventTargetPositionComponent>(
                    new EventTargetPositionComponent
                    {
                        Position = position
                    });

            return this;
        }

        public IEventEntityBuilder<TEntity, TEntityID> HappenedAtTime(
            Entity eventEntity,
            long ticks)
        {
            eventEntity
                .Set<EventTimeComponent>(
                    new EventTimeComponent
                    {
                        Ticks = ticks
                    });

            return this;
        }

        public IEventEntityBuilder<TEntity, TEntityID> WithData<TData>(
            Entity eventEntity,
            TData data)
        {
            eventEntity
                .Set<TData>(data);
            
            return this;
        }
    }
}
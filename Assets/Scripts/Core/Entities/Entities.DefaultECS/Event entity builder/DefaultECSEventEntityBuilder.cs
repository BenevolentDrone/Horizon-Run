using System;

using System.Numerics;

using DefaultEcs;

namespace HereticalSolutions.Entities
{
    public class DefaultECSEventEntityBuilder<TEntityID>
        : IEventEntityBuilder<Entity, TEntityID>
    {
        protected readonly World eventWorld;

        public DefaultECSEventEntityBuilder(
            World eventWorld)
        {
            this.eventWorld = eventWorld;
        }

        public IEventEntityBuilder<Entity, TEntityID> NewEvent(out Entity eventEntity)
        {
            eventEntity = eventWorld.CreateEntity();

            return this;
        }

        public IEventEntityBuilder<Entity, TEntityID> HappenedAtPosition(
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

        public IEventEntityBuilder<Entity, TEntityID> AddressedToEntity(
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

        public IEventEntityBuilder<Entity, TEntityID> AddressedToWorldLocalEntity(
            Entity eventEntity,
            Entity receiverEntity)
        {
            eventEntity
                .Set<EventReceiverWorldLocalEntityComponent<Entity>>(
                    new EventReceiverWorldLocalEntityComponent<Entity>
                    {
                        Receiver = receiverEntity
                    });

            return this;
        }

        public IEventEntityBuilder<Entity, TEntityID> CausedByEntity(
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

        public IEventEntityBuilder<Entity, TEntityID> CausedByWorldLocalEntity(
            Entity eventEntity,
            Entity sourceEntity)
        {
            eventEntity
                .Set<EventSourceWorldLocalEntityComponent<Entity>>(
                    new EventSourceWorldLocalEntityComponent<Entity>
                    {
                        Source = sourceEntity
                    });

            return this;
        }

        public IEventEntityBuilder<Entity, TEntityID> TargetedAtEntity(
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

        public IEventEntityBuilder<Entity, TEntityID> TargetedAtWorldLocalEntity(
            Entity eventEntity,
            Entity targetEntity)
        {
            eventEntity
                .Set<EventTargetWorldLocalEntityComponent<Entity>>(
                    new EventTargetWorldLocalEntityComponent<Entity>
                    {
                        Target = targetEntity
                    });

            return this;
        }

        public IEventEntityBuilder<Entity, TEntityID> TargetedAtPosition(
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

        public IEventEntityBuilder<Entity, TEntityID> HappenedAtTime(
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

        public IEventEntityBuilder<Entity, TEntityID> WithData<TData>(
            Entity eventEntity,
            TData data)
        {
            eventEntity
                .Set<TData>(data);
            
            return this;
        }
    }
}
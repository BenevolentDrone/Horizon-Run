using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ReplicateToClientsEventSystem<TEventComponent, TDelta>
        : AEntitySetSystem<TDelta>
    {
        private readonly INetworkEventEntityManager<Entity> networkEventEntityManager;

        private readonly bool replicateIfOriginatedFromClient;
        
        private readonly bool keepProcessingAfterReplication;
        
        private readonly ILogger logger;
        
        public ReplicateToClientsEventSystem(
            World eventWorld,
            INetworkEventEntityManager<Entity> networkEventEntityManager,
            bool replicateIfOriginatedFromClient,
            bool keepProcessingAfterReplication,
            ILogger logger)
            : base(
                eventWorld
                    .GetEntities()
                    .With<TEventComponent>()
                    .Without<EventProcessedComponent>()
                    .AsSet())
        {
            this.networkEventEntityManager = networkEventEntityManager;

            this.replicateIfOriginatedFromClient = replicateIfOriginatedFromClient;
            
            this.keepProcessingAfterReplication = keepProcessingAfterReplication;
            
            this.logger = logger;
        }

        protected override void Update(TDelta delta, in Entity entity)
        {
            if (entity.Has<NetworkEventSourceComponent>())
            {
                var sourceComponent = entity.Get<NetworkEventSourceComponent>();
                
                if (sourceComponent.Source == ENetworkEventSource.SERVER)
                {
                    logger?.LogError<ReplicateToClientsEventSystem<TEventComponent, TDelta>>(
                        $"EVENT ENTITY {nameof(TEventComponent)} HAS SERVER AS NETWORK EVENT SOURCE, ABORTING PROCESSING");

                    entity.Set<EventProcessedComponent>();
                    
                    return;
                }

                if (sourceComponent.Source == ENetworkEventSource.CLIENT
                    && !replicateIfOriginatedFromClient)
                {
                    logger?.Log<ReplicateToClientsEventSystem<TEventComponent, TDelta>>(
                        $"PROCESSING EVENT ENTITY SOURCED FROM CLIENT: {nameof(TEventComponent)}");

                    return;
                }
            }

            networkEventEntityManager.ReplicateEventEntity(entity);

            if (!keepProcessingAfterReplication)
            {
                entity.Set<EventProcessedComponent>();
            }
        }
    }
}
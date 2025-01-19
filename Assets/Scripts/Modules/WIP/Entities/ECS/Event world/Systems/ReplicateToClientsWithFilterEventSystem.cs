using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ReplicateToClientsWithFilterEventSystem<TEventComponent, TDelta>
        : AEntitySetSystem<TDelta>
    {
        private readonly INetworkEventEntityManager<Entity> networkEventEntityManager;
        
        private readonly Func<Entity, bool> filterDelegate;

        private readonly bool replicateIfOriginatedFromClient;
        
        private readonly bool keepProcessingAfterReplication;
        
        private readonly ILogger logger;
        
        public ReplicateToClientsWithFilterEventSystem(
            World eventWorld,
            INetworkEventEntityManager<Entity> networkEventEntityManager,
            Func<Entity, bool> filterDelegate,
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
            
            this.filterDelegate = filterDelegate;

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
                    logger?.LogError<ReplicateToClientsWithFilterEventSystem<TEventComponent, TDelta>>(
                        $"EVENT ENTITY {nameof(TEventComponent)} HAS SERVER AS NETWORK EVENT SOURCE, ABORTING PROCESSING");
                    
                    entity.Set<EventProcessedComponent>();
                    
                    return;
                }

                if (sourceComponent.Source == ENetworkEventSource.CLIENT
                    && !replicateIfOriginatedFromClient)
                {
                    logger?.Log<ReplicateToClientsWithFilterEventSystem<TEventComponent, TDelta>>(
                        $"PROCESSING EVENT ENTITY SOURCED FROM CLIENT: {nameof(TEventComponent)}");
                    
                    return;
                }
            }

            if (!filterDelegate.Invoke(entity))
            {
                logger?.Log<ReplicateToClientsWithFilterEventSystem<TEventComponent, TDelta>>(
                    $"FILTER CONDITIONS UNMET FOR EVENT ENTITY: {nameof(TEventComponent)}. ABORTING REPLICATION");
                
                return;
            }

            logger?.Log<ReplicateToClientsWithFilterEventSystem<TEventComponent, TDelta>>(
                $"REPLICATING EVENT ENTITY: {nameof(TEventComponent)}");
            
            networkEventEntityManager.ReplicateEventEntity(entity);

            if (!keepProcessingAfterReplication)
            {
                entity.Set<EventProcessedComponent>();
            }
        }
    }
}
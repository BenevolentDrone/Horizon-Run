using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ReplicateToServerWithFilterEventSystem<TEventComponent, TDelta>
        : AEntitySetSystem<TDelta>
    {
        private readonly INetworkEventEntityManager<Entity> networkEventEntityManager;
	
        private readonly Func<Entity, bool> filterDelegate;
        
        private readonly bool keepProcessingAfterReplication;
        
        private readonly ILogger logger;
		
        public ReplicateToServerWithFilterEventSystem(
            World eventWorld,
            INetworkEventEntityManager<Entity> networkEventEntityManager,
            Func<Entity, bool> filterDelegate,
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
            
            this.keepProcessingAfterReplication = keepProcessingAfterReplication;
            
            this.logger = logger;
        }

        protected override void Update(TDelta delta, in Entity entity)
        {
            if (entity.Has<NetworkEventSourceComponent>())
            {
                var sourceComponent = entity.Get<NetworkEventSourceComponent>();
                
                if (sourceComponent.Source == ENetworkEventSource.CLIENT)
                {
                    logger?.LogError<ReplicateToServerWithFilterEventSystem<TEventComponent, TDelta>>(
                        $"EVENT ENTITY {nameof(TEventComponent)} HAS CLIENT AS NETWORK EVENT SOURCE, ABORTING PROCESSING");

                    entity.Set<EventProcessedComponent>();

                    return;
                }
				
                if (sourceComponent.Source == ENetworkEventSource.SERVER)
                {
                    logger?.Log<ReplicateToServerWithFilterEventSystem<TEventComponent, TDelta>>(
                        $"PROCESSING EVENT ENTITY SOURCED FROM SERVER: {nameof(TEventComponent)}");

                    return;
                }
            }

            if (!filterDelegate.Invoke(entity))
            {
                logger?.Log<ReplicateToServerWithFilterEventSystem<TEventComponent, TDelta>>(
                    $"FILTER CONDITIONS UNMET FOR EVENT ENTITY: {nameof(TEventComponent)}. ABORTING REPLICATION");

                return;
            }

            logger?.Log<ReplicateToServerWithFilterEventSystem<TEventComponent, TDelta>>(
                $"REPLICATING EVENT ENTITY: {nameof(TEventComponent)}");

            networkEventEntityManager.ReplicateEventEntity(entity);
			
            if (!keepProcessingAfterReplication)
            {
                entity.Set<EventProcessedComponent>();
            }
        }
    }
}
using HereticalSolutions.Entities;

using HereticalSolutions.Networking.ECS;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
	public class ReplicateToServerEventSystem<TEventComponent, TDelta>
		: AEntitySetSystem<TDelta>
	{
		private readonly INetworkEventEntityManager<Entity> networkEventEntityManager;
	
		private readonly bool keepProcessingAfterReplication;
        
		private readonly ILogger logger;
		
		public ReplicateToServerEventSystem(
			World eventWorld,
			INetworkEventEntityManager<Entity> networkEventEntityManager,
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
					logger?.LogError<ReplicateToServerEventSystem<TEventComponent, TDelta>>(
						$"EVENT ENTITY {nameof(TEventComponent)} HAS CLIENT AS NETWORK EVENT SOURCE, ABORTING PROCESSING");
                    
					entity.Set<EventProcessedComponent>();

					return;
				}
				
				if (sourceComponent.Source == ENetworkEventSource.SERVER)
				{
					logger?.Log<ReplicateToServerEventSystem<TEventComponent, TDelta>>(
						$"PROCESSING EVENT ENTITY SOURCED FROM SERVER: {nameof(TEventComponent)}");
					
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
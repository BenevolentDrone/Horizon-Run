using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class PreventFromProcessingUnlessOriginatedFromServerEventComponent<TEventComponent, TDelta>
        : AEntitySetSystem<TDelta>
    {
        private readonly ILogger logger;
        
        public PreventFromProcessingUnlessOriginatedFromServerEventComponent(
            World eventWorld,
            ILogger logger = null)
            : base(
                eventWorld
                    .GetEntities()
                    .With<TEventComponent>()
                    .Without<EventProcessedComponent>()
                    .AsSet())
        {
            this.logger = logger;
        }

        protected override void Update(TDelta delta, in Entity entity)
        {
            if (entity.Has<NetworkEventSourceComponent>())
            {
                var sourceComponent = entity.Get<NetworkEventSourceComponent>();
                
                if (sourceComponent.Source == ENetworkEventSource.CLIENT)
                {
                    logger?.LogError<PreventFromProcessingUnlessOriginatedFromServerEventComponent<TEventComponent, TDelta>>(
                        $"EVENT ENTITY {nameof(TEventComponent)} HAS CLIENT AS NETWORK EVENT SOURCE, ABORTING PROCESSING");
                    
                    entity.Set<EventProcessedComponent>();

                    return;
                }
				
                if (sourceComponent.Source == ENetworkEventSource.SERVER)
                {
                    logger?.Log<PreventFromProcessingUnlessOriginatedFromServerEventComponent<TEventComponent, TDelta>>(
                        $"PROCESSING EVENT ENTITY SOURCED FROM SERVER: {nameof(TEventComponent)}");
                    
                    return;
                }
            }
            
            entity.Set<EventProcessedComponent>();

            //logger?.LogError<PreventFromProcessingUnlessOriginatedFromServerEventComponent<TEventComponent, TDelta>>(
            //    $"EVENT ENTITY {nameof(TEventComponent)} WAS PREVENTED FROM PROCESSING," +
            //    $" PLEASE ENSURE THE EVENT IS NOT FIRED WHERE UNNECESSARY");
        }
    }
}
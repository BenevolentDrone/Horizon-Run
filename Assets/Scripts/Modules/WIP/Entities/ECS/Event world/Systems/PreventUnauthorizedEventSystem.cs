using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class PreventUnauthorizedEventSystem<TEventComponent, TDelta>
        : AEntitySetSystem<TDelta>
    {
        private readonly Func<Entity, byte> getAuthoringPermissionDelegate;
        
        private readonly ILogger logger;
        
        public PreventUnauthorizedEventSystem(
            World eventWorld,
            Func<Entity, byte> getAuthoringPermissionDelegate,
            ILogger logger)
            : base(
                eventWorld
                    .GetEntities()
                    .With<TEventComponent>()
                    .Without<EventProcessedComponent>()
                    .AsSet())
        {
            this.getAuthoringPermissionDelegate = getAuthoringPermissionDelegate;
            
            this.logger = logger;
        }

        protected override void Update(TDelta delta, in Entity entity)
        {
            if (entity.Has<NetworkEventSourceComponent>())
            {
                var sourceComponent = entity.Get<NetworkEventSourceComponent>();
                
                if (sourceComponent.Source == ENetworkEventSource.SERVER)
                {
                    logger?.LogError<PreventUnauthorizedEventSystem<TEventComponent, TDelta>>(
                        $"EVENT ENTITY {nameof(TEventComponent)} HAS SERVER AS NETWORK EVENT SOURCE, ABORTING PROCESSING");
                    
                    entity.Set<EventProcessedComponent>();

                    return;
                }
				
                if (sourceComponent.Source == ENetworkEventSource.CLIENT)
                {
                    byte authoringPermission = getAuthoringPermissionDelegate.Invoke(entity);

                    if (authoringPermission == byte.MaxValue)
                    {
                        logger?.Log<PreventUnauthorizedEventSystem<TEventComponent, TDelta>>(
                            $"EVENT ENTITY AUTHOR IS SERVER, PROCEEDING");
                        
                        return;
                    }

                    if (authoringPermission == sourceComponent.PlayerSlot)
                    {
                        logger?.Log<PreventUnauthorizedEventSystem<TEventComponent, TDelta>>(
                            $"EVENT ENTITY AUTHOR {sourceComponent.PlayerSlot} IS EQUAL TO AUTHORING PERMISSION {authoringPermission}, PROCEEDING");
                        
                        return;
                    }

                    logger?.Log<PreventUnauthorizedEventSystem<TEventComponent, TDelta>>(
                        $"EVENT ENTITY AUTHOR {sourceComponent.PlayerSlot} IS NOT EQUAL TO AUTHORING PERMISSION {authoringPermission}, ABORTING PROCESSING");

                    entity.Set<EventProcessedComponent>();
                    
                    return;
                }
            }
        }
    }
}
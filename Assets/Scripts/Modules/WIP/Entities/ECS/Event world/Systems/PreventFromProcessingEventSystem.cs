using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class PreventFromProcessingEventSystem<TEventComponent, TDelta>
        : AEntitySetSystem<TDelta>
    {
        private readonly ILogger logger;
        
        public PreventFromProcessingEventSystem(
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
            entity.Set<EventProcessedComponent>();

            //logger?.LogError<PreventFromProcessingEventSystem<TEventComponent, TDelta>>(
            //    $"EVENT ENTITY {nameof(TEventComponent)} WAS PREVENTED FROM PROCESSING," +
            //    $" PLEASE ENSURE THE EVENT IS NOT FIRED WHERE UNNECESSARY");
        }
    }
}
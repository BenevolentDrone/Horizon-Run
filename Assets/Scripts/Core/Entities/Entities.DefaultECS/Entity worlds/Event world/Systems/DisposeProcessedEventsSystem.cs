using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Entities
{
    public class DisposeProcessedEventsSystem<TDelta>
        : AEntitySetSystem<TDelta>
    {
        public DisposeProcessedEventsSystem(World eventWorld)
            : base(
                eventWorld
                    .GetEntities()
                    .With<EventProcessedComponent>()
                    .AsSet())
        {
        }

        protected override void Update(TDelta delta, in Entity entity)
        {
            entity.Set<DespawnComponent>();
        }
    }
}
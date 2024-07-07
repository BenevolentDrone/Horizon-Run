using HereticalSolutions.Entities;

using DefaultEcs;
using DefaultEcs.System;

namespace HereticalSolutions.Templates.Universal.Unity
{
    public class CheckSensorPresenceOnSensorScanPerformedEventSystem : AEntitySetSystem<float>
    {
        public CheckSensorPresenceOnSensorScanPerformedEventSystem(
            World world)
            : base(
                world
                    .GetEntities()
                    .With<SensorScanPerformedEventComponent>()
                    .With<EventSourceWorldLocalEntityComponent<Entity>>()
                    .Without<EventProcessedComponent>()
                    .AsSet())
        {
        }

        protected override void Update(
            float deltaTime,
            in Entity entity)
        {
            var eventSourceEntityComponent = entity.Get<EventSourceWorldLocalEntityComponent<Entity>>();

            var sensorEntity = eventSourceEntityComponent.Source;
            
            if (!sensorEntity.IsAlive)
            {
                entity.Set<EventProcessedComponent>();
            }
            
            if (!sensorEntity.Has<SensorComponent>())
            {
                entity.Set<EventProcessedComponent>();
            }
        }
    }
}
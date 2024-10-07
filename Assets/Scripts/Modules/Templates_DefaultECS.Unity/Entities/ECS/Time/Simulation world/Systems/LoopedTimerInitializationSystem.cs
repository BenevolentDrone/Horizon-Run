using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class LoopedTimerInitializationSystem
        : IEntityInitializationSystem
    {
        public LoopedTimerInitializationSystem()
        {
        }

        //Required by ISystem
        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if (!IsEnabled)
                return;

            if (!entity.Has<LoopedTimerComponent>())
                return;

            ref var loopedTimerComponent = ref entity.Get<LoopedTimerComponent>();

            loopedTimerComponent.TimerHandle = 0;
        }

        public void Dispose()
        {
        }
    }
}
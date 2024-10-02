using HereticalSolutions.Time;

using HereticalSolutions.Entities;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public class LoopedTimerDeinitializationSystem
        : IEntityInitializationSystem
    {
        private readonly ITimerManager timerManager;

        public LoopedTimerDeinitializationSystem(
            ITimerManager timerManager)
        {
            this.timerManager = timerManager;
        }

        public bool IsEnabled { get; set; } = true;

        public void Update(Entity entity)
        {
            if(!IsEnabled)
                return;
            
            if (!entity.Has<LoopedTimerComponent>())
                return;

            var loopedTimerComponent = entity.Get<LoopedTimerComponent>();
            
            timerManager.TryDestroyTimer(
                loopedTimerComponent.TimerHandle);

            loopedTimerComponent.TimerHandle = 0;
        }

        public void Dispose()
        {
            
        }
    }
}
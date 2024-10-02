using HereticalSolutions.Synchronization;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.DI
{
    public class TimerManagersInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;
        
        
        [Inject(Id = "Update time manager")]
        private ITimeManager updateTimeManager;

        [Inject(Id = "Fixed update time manager")]
        private ITimeManager fixedUpdateTimeManager;

        [Inject(Id = "Late update time manager")]
        private ITimeManager lateUpdateTimeManager;

        
        public override void InstallBindings()
        {
            #region Simulation update
            
            var updateSynchronizationProvidersRepository = updateTimeManager as ISynchronizationProvidersRepository;

            updateSynchronizationProvidersRepository.TryGetProvider(
                "Update",
                out var updateSynchronizationProvider);
            
            var simulationUpdateTimerManager = TimeFactory.BuildTimerManager(
                "Simulation update timer manager",
                updateSynchronizationProvider,
                false,
                loggerResolver);

            Container
                .Bind<ITimerManager>()
                .WithId(simulationUpdateTimerManager.ID)
                .FromInstance(simulationUpdateTimerManager)
                .AsCached();

            #endregion

            #region Simulation fixed update

            var fixedUpdateSynchronizationProvidersRepository = fixedUpdateTimeManager as ISynchronizationProvidersRepository;

            fixedUpdateSynchronizationProvidersRepository.TryGetProvider(
                "Fixed update",
                out var fixedUpdateSynchronizationProvider);
            
            var simulationFixedUpdateTimerManager = TimeFactory.BuildTimerManager(
                "Simulation fixed update timer manager",
                fixedUpdateSynchronizationProvider,
                false,
                loggerResolver);

            Container
                .Bind<ITimerManager>()
                .WithId(simulationFixedUpdateTimerManager.ID)
                .FromInstance(simulationFixedUpdateTimerManager)
                .AsCached();

            #endregion

            #region View late update

            var lateUpdateSynchronizationProvidersRepository = lateUpdateTimeManager as ISynchronizationProvidersRepository;

            lateUpdateSynchronizationProvidersRepository.TryGetProvider(
                "Late update",
                out var lateUpdateSynchronizationProvider);
            
            var viewLateUpdateTimerManager = TimeFactory.BuildTimerManager(
                "View late update timer manager",
                lateUpdateSynchronizationProvider,
                false,
                loggerResolver);

            Container
                .Bind<ITimerManager>()
                .WithId(viewLateUpdateTimerManager.ID)
                .FromInstance(viewLateUpdateTimerManager)
                .AsCached();
            
            #endregion
        }
    }
}
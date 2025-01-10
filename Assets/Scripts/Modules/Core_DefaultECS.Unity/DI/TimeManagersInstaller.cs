using HereticalSolutions.Synchronization;
using HereticalSolutions.Synchronization.Factories;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;
using ITickable = HereticalSolutions.Time.ITickable;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
    public class TimeManagersInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [SerializeField]
        private TimeSynchronizationBehaviour timeSynchronizationBehaviour;

        public override void InstallBindings()
        {
            #region Update

            var updateTimeManager = TimerFactory.BuildTimeManager(loggerResolver);

            var updateSynchronizableRepository = updateTimeManager as ISynchronizableGenericArgRepository<float>;

            updateSynchronizableRepository.AddSynchronizable(
                SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
                    "Update",
                    canBeToggled: true,
                    active: true,
                    canScale: true,
                    scale: 1f,
                    scaleDeltaDelegate: (value, scale) => value * scale,
                    loggerResolver: loggerResolver));

            Container
                .Bind<ITimeManager>()
                .WithId("Update time manager")
                .FromInstance(updateTimeManager)
                .AsCached();

            #endregion

            #region Fixed update

            var fixedUpdateTimeManager = TimerFactory.BuildTimeManager(loggerResolver);

            var fixedUpdateSynchronizableRepository = fixedUpdateTimeManager as ISynchronizableGenericArgRepository<float>;

            fixedUpdateSynchronizableRepository.AddSynchronizable(
                SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
                    "Fixed update",
                    canBeToggled: true,
                    active: true,
                    canScale: true,
                    scale: 1f,
                    scaleDeltaDelegate: (value, scale) => value * scale,
                    loggerResolver: loggerResolver));

            Container
                .Bind<ITimeManager>()
                .WithId("Fixed update time manager")
                .FromInstance(fixedUpdateTimeManager)
                .AsCached();

            #endregion

            #region Late update

            var lateUpdateTimeManager = TimerFactory.BuildTimeManager(loggerResolver);

            var lateUpdateSynchronizableRepository = lateUpdateTimeManager as ISynchronizableGenericArgRepository<float>;

            lateUpdateSynchronizableRepository.AddSynchronizable(
                SynchronizationFactory.BuildSynchronizationContextGeneric<float>(
                    "Late update",
                    canBeToggled: true,
                    active: true,
                    canScale: true,
                    scale: 1f,
                    scaleDeltaDelegate: (value, scale) => value * scale,
                    loggerResolver: loggerResolver));

            Container
                .Bind<ITimeManager>()
                .WithId("Late update time manager")
                .FromInstance(lateUpdateTimeManager)
                .AsCached();

            #endregion


            var updateTimeManagerAsTickable = updateTimeManager as ITickable;

            var fixedUpdateTimeManagerAsTickable = fixedUpdateTimeManager as ITickable;

            var lateUpdateTimeManagerAsTickable = lateUpdateTimeManager as ITickable;

            timeSynchronizationBehaviour.Initialize(
                updateTimeManagerAsTickable,
                fixedUpdateTimeManagerAsTickable,
                lateUpdateTimeManagerAsTickable);
        }
    }
}
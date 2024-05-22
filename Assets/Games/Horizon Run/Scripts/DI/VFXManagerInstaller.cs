using HereticalSolutions.Time;

using HereticalSolutions.HorizonRun.Factories;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
    public class VFXManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [Inject(Id = "View late update timer manager")]
        private ITimerManager viewLateUpdateTimerManager;
        
        [SerializeField]
        private VFXPoolSettings settings;

        [SerializeField]
        private Transform parentTransform;
        
        public override void InstallBindings()
        {
            var VFXManager = VFXPoolFactory.BuildVFXManager(
                Container,
                settings,
                viewLateUpdateTimerManager,
                parentTransform,
                loggerResolver);

            Container
                .Bind<IVFXManager>()
                .FromInstance(VFXManager)
                .AsCached();
        }
    }
}
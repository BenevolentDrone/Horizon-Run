using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using Zenject;

using HereticalSolutions.HorizonRun.Factories;

namespace HereticalSolutions.HorizonRun.DI
{
    public class EntityManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [Inject]
        private EEntityAuthoringPresets authoringPreset;

        public override void InstallBindings()
        {
            var entityManager = HorizonRunEntityFactory.BuildHorizonRunEntityManager(
                authoringPreset,
                loggerResolver);

            Container
                .Bind<HorizonRunEntityManager>()
                .FromInstance(entityManager)
                .AsCached();

            //For editor purposes
            Container
                .Bind<DefaultECSEntityManager<Guid>>()
                .FromInstance(entityManager)
                .AsCached();
        }
    }
}
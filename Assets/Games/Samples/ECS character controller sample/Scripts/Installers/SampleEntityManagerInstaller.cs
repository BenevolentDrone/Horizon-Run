using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using Zenject;

using HereticalSolutions.Samples.ECSCharacterControllerSample.Factories;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
    public class SampleEntityManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        public override void InstallBindings()
        {
            var entityManager = SampleEntityFactory.BuildSampleEntityManager(
                loggerResolver);

            Container
                .Bind<SampleEntityManager>()
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
using System;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using Zenject;

using HereticalSolutions.Templates.Universal.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.DI
{
    public class EntityManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [Inject]
        private EntityAuthoringSettings entityAuthoringSettings;

        public override void InstallBindings()
        {
            var entityManager = UniversalTemplateEntityFactory.BuildUniversalTemplateEntityManager(
                entityAuthoringSettings.AuthoringPreset,
                loggerResolver);

            Container
                .Bind<UniversalTemplateEntityManager>()
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
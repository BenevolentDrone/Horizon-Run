using HereticalSolutions.Entities;

using HereticalSolutions.Modules.Core_DefaultECS.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
    public class EntityManagerInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        [Inject]
        private EntityAuthoringSettings entityAuthoringSettings;

        public override void InstallBindings()
        {
            var entityManager = EntityFactory.BuildEntityManager(
                entityAuthoringSettings.AuthoringPreset,
                loggerResolver);

            Container
                .Bind<EntityManager>()
                .FromInstance(entityManager)
                .AsCached();
        }
    }
}
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement.Factories;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.LifetimeManagement
{
    public class SceneLifetimeInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;
        
        [Inject(Id = LifetimeConsts.PROJECT_LIFETIME)]
        private ILifetimeable projectLifetime;

        [Inject]
        private IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool;
        
        [SerializeField]
        private string sceneName;

        public override void InstallBindings()
        {
            var sceneLifetime = LifetimeFactory.BuildHierarchicalLifetime(
                null,
                bufferPool,
                loggerResolver,
                projectLifetime);
            
            sceneLifetime.SetUp();

            sceneLifetime.Initialize();

            Container
                .Bind<ILifetimeable>()
                .WithId(LifetimeConsts.SCENE_LIFETIME)
                .FromInstance(sceneLifetime)
                .AsCached();
        }
        
        private void OnDestroy()
        {
            var sceneLifetime = Container
                .ResolveId<ILifetimeable>(
                    LifetimeConsts.SCENE_LIFETIME);

            ITearDownable sceneLifetimeAsTearDownable = sceneLifetime as ITearDownable;
            
            if (sceneLifetimeAsTearDownable != null)
                sceneLifetimeAsTearDownable.TearDown();
        }
    }
}
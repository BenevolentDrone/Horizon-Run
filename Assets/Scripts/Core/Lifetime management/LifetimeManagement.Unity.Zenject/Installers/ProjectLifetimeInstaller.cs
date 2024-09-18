using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.LifetimeManagement.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

using Zenject;

namespace HereticalSolutions.LifetimeManagement
{
    public class ProjectLifetimeInstaller : MonoInstaller
    {
        [Inject]
        private ILoggerResolver loggerResolver;

        public override void InstallBindings()
        {
            IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool =
                new PoolWithListCleanup<List<IReadOnlyHierarchyNode<ILifetimeable>>>(
                    StackPoolFactory.BuildStackPool<List<IReadOnlyHierarchyNode<ILifetimeable>>>(
                        new AllocationCommand<List<IReadOnlyHierarchyNode<ILifetimeable>>>
                        {
                            Descriptor = new AllocationCommandDescriptor
                            {
                                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                                Amount = 5 //TODO: REMOVE MAGIC
                            },
                            AllocationDelegate =
                                AllocationsFactory
                                    .ActivatorAllocationDelegate<List<IReadOnlyHierarchyNode<ILifetimeable>>>
                        },
                        new AllocationCommand<List<IReadOnlyHierarchyNode<ILifetimeable>>>
                        {
                            Descriptor = new AllocationCommandDescriptor
                            {
                                Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

                                Amount = 5 //TODO: REMOVE MAGIC
                            },
                            AllocationDelegate =
                                AllocationsFactory
                                    .ActivatorAllocationDelegate<List<IReadOnlyHierarchyNode<ILifetimeable>>>
                        },
                        loggerResolver));
            
            Container
                .Bind<IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>>>()
                .FromInstance(bufferPool)
                .AsCached();   
                
            var projectLifetime = LifetimeFactory.BuildHierarchicalLifetime(
                null,
                bufferPool,
                loggerResolver: loggerResolver);
            
            projectLifetime.SetUp();

            projectLifetime.Initialize();

            Container
                .Bind<ILifetimeable>()
                .WithId(LifetimeConsts.PROJECT_LIFETIME)
                .FromInstance(projectLifetime)
                .AsCached();
        }

        private void OnDestroy()
        {
            var projectLifetime = Container
                .ResolveId<ILifetimeable>(
                    LifetimeConsts.PROJECT_LIFETIME);

            ITearDownable projectLifetimeAsTearDownable = projectLifetime as ITearDownable;
            
            if (projectLifetimeAsTearDownable != null)
                projectLifetimeAsTearDownable.TearDown();
        }
    }
}
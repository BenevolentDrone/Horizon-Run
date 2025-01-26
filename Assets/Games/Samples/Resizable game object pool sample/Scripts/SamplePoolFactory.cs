using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.AllocationCallbacks;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ResizableGameObjectPoolSample
{
    public static class SamplePoolFactory
    {
        public static IManagedPool<GameObject> BuildPool(
            DiContainer container,
            SamplePoolSettings settings,
            ILoggerResolver loggerResolver)
        {
            #region Builders

            // Create a builder for resizable pools
            var managedPoolBuilder = new ManagedPoolBuilder<GameObject>(
                loggerResolver,
                loggerResolver?.GetLogger<ManagedPoolBuilder<GameObject>>());

            #endregion

            #region Callbacks

            // Create a push to decorated pool callback
            PushToManagedPoolWhenAvailableCallback<GameObject> pushCallback =
                ObjectPoolAllocationCallbackFactory.BuildPushToManagedPoolWhenAvailableCallback<GameObject>();

            #endregion

            #region Metadata descriptor builders

            // Create an array of metadata descriptor builder functions
            var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
            {
                //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
            };

            #endregion

            // Get the prefab of the current element
            var prefab = settings.Prefab;

            // Create a value allocation delegate
            Func<GameObject> valueAllocationDelegate =
                () => UnityZenjectAllocationFactory.DIResolveOrInstantiateAllocationDelegate(
                    container,
                    prefab);

            #region Allocation callbacks initialization

            var facadeAllocationCallbacks = new IAllocationCallback<IPoolElementFacade<GameObject>>[]
            {
                pushCallback
            };

            #endregion

            // Initialize the resizable pool builder
            managedPoolBuilder.Initialize(
                valueAllocationDelegate,

                metadataDescriptorBuilders,

                settings.Initial,
                settings.Additional,
                
                facadeAllocationCallbacks,
                null);

            // Build the resizable pool
            var resizablePool = managedPoolBuilder.BuildLinkedListManagedPool();

            // Build the game object pool
            var gameObjectPool = UnityDecoratorPoolFactory.BuildGameObjectManagedPool(
                resizablePool,
                null);

            // Set the root of the push callback
            pushCallback.TargetPool = gameObjectPool;

            return gameObjectPool;
        }
    }
}
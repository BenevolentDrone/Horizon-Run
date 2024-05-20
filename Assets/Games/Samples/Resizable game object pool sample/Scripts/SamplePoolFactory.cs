using System;

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
        public static INonAllocDecoratedPool<GameObject> BuildPool(
            DiContainer container,
            SamplePoolSettings settings,
            ILoggerResolver loggerResolver = null)
        {
            #region Builders

            // Create a builder for resizable pools.
            var resizablePoolBuilder = new ResizablePoolBuilder<GameObject>(
                loggerResolver,
                loggerResolver?.GetLogger<ResizablePoolBuilder<GameObject>>());

            #endregion

            #region Callbacks

            // Create a push to decorated pool callback.
            PushToDecoratedPoolCallback<GameObject> pushCallback =
                PoolsFactory.BuildPushToDecoratedPoolCallback<GameObject>(
                    PoolsFactory.BuildDeferredCallbackQueue<GameObject>());

            #endregion

            #region Metadata descriptor builders

            // Create an array of metadata descriptor builder functions.
            var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
            {
                PoolsFactory.BuildIndexedMetadataDescriptor
            };

            #endregion

            // Get the prefab of the current element
            var prefab = settings.Prefab;

            // Create a value allocation delegate.
            Func<GameObject> valueAllocationDelegate =
                () => UnityZenjectAllocationsFactory.DIResolveOrInstantiateAllocationDelegate(
                    container,
                    prefab);

            #region Allocation callbacks initialization

            // Create an array of allocation callbacks.
            var callbacks = new IAllocationCallback<GameObject>[]
            {
                pushCallback
            };

            #endregion

            // Initialize the resizable pool builder.
            resizablePoolBuilder.Initialize(
                valueAllocationDelegate,
                true,

                metadataDescriptorBuilders,

                settings.Initial,
                settings.Additional,
                
                callbacks);

            // Build the resizable pool.
            var resizablePool = resizablePoolBuilder.BuildResizablePool();

            // Build the game object pool.
            var gameObjectPool = UnityDecoratorsPoolsFactory.BuildNonAllocGameObjectPool(
                resizablePool,
                null);

            // Set the root of the push callback.
            pushCallback.Root = gameObjectPool;

            return gameObjectPool;
        }
    }
}
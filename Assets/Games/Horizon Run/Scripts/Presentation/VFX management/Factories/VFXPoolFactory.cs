using System;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;
using HereticalSolutions.Pools.AllocationCallbacks;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.HorizonRun.Factories
{
    public static class VFXPoolFactory
    {
        public static VFXManager BuildVFXManager(
            DiContainer container,
            VFXPoolSettings settings,
            ITimerManager timerManager,
            Transform parentTransform = null,
            ILoggerResolver loggerResolver = null)
        {
            var argumentsCache = new ArgumentBuilder()
                .Add<WorldPositionArgument>(out var worldPositionArgument)
                .Add<WorldRotationArgument>(out var worldRotationArgument)
                .Add<AddressArgument>(out var addressArgument)
                .Build();

            return new VFXManager(
                BuildPool(
                    container,
                    settings,
                    timerManager,
                    parentTransform,
                    loggerResolver),
                addressArgument,
                worldPositionArgument,
                worldRotationArgument,
                argumentsCache);
        }

        public static INonAllocDecoratedPool<GameObject> BuildPool(
            DiContainer container,
            VFXPoolSettings settings,
            ITimerManager timerManager,
            Transform parentTransform = null,
            ILoggerResolver loggerResolver = null)
        {
            #region Builders

            // Create a builder for pools with address.
            var poolWithAddressBuilder = new PoolWithAddressBuilder<GameObject>(
                loggerResolver,
                loggerResolver?.GetLogger<PoolWithAddressBuilder<GameObject>>());

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
                PoolsFactory.BuildIndexedMetadataDescriptor,
                AddressDecoratorsPoolsFactory.BuildAddressMetadataDescriptor,
                TimersDecoratorsPoolsFactory.BuildRuntimeTimerWithPushSubscriptionMetadataDescriptor
            };

            #endregion

            // Initialize the pool with address builder.
            poolWithAddressBuilder.Initialize();

            foreach (var element in settings.Elements)
            {
                #region Address

                // Get the full address of the element
                string fullAddress = element.Name;

                // Convert the address to hashes.
                int[] addressHashes = fullAddress.AddressToHashes();

                // Build a set address callback.
                SetAddressCallback<GameObject> setAddressCallback =
                    AddressDecoratorsPoolsFactory.BuildSetAddressCallback<GameObject>(
                        fullAddress,
                        addressHashes);

                #endregion

                // Build a set runtime timer callback.
                SetDurationAndPushSubscriptionCallback<GameObject> setRuntimeTimerWithPushSubscriptionCallback =
                    TimersDecoratorsPoolsFactory.BuildSetDurationAndPushSubscriptionCallback<GameObject>(
                        element.Duration,
                        loggerResolver);

                // Build a rename callback.
                RenameByStringAndIndexCallback renameCallback =
                    UnityDecoratorsPoolsFactory.BuildRenameByStringAndIndexCallback(element.Name);

                #region Allocation callbacks initialization

                // Create an array of allocation callbacks.
                var callbacks = new IAllocationCallback<GameObject>[]
                {
                    renameCallback,
                    setAddressCallback,
                    setRuntimeTimerWithPushSubscriptionCallback,
                    pushCallback
                };

                #endregion

                #region Value allocation delegate initialization

                // Get the prefab of the current variant.
                var prefab = element.Prefab;

                // Create a value allocation delegate.
                Func<GameObject> valueAllocationDelegate =
                    () => UnityZenjectAllocationsFactory.DIResolveOrInstantiateAllocationDelegate(
                        container,
                        prefab);

                #endregion

                // Initialize the resizable pool builder.
                resizablePoolBuilder.Initialize(
                    valueAllocationDelegate,
                    true,

                    metadataDescriptorBuilders,

                    element.Initial,
                    element.Additional,

                    callbacks);

                // Build the resizable pool.
                var resizablePool = resizablePoolBuilder.BuildResizablePool();

                // Build the game object pool.
                var gameObjectPool = UnityDecoratorsPoolsFactory.BuildNonAllocGameObjectPool(
                    resizablePool,
                    parentTransform);

                // Build the pool with runtime timers.
                var poolWithRuntimeTimers = TimersDecoratorsPoolsFactory.BuildNonAllocPoolWithRuntimeTimer(
                    gameObjectPool,
                    timerManager,
                    loggerResolver);

                // Parse the address and variant pool to the pool with address builder.
                poolWithAddressBuilder.Parse(
                    fullAddress,
                    poolWithRuntimeTimers);
            }

            // Build the pool with address.
            var poolWithAddress = poolWithAddressBuilder.Build();

            // Set the root of the push callback.
            pushCallback.Root = poolWithAddress;

            return poolWithAddress;
        }
    }
}
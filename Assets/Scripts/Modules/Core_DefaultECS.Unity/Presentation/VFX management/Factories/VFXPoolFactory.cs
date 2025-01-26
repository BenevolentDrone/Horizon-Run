using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.AllocationCallbacks;
using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.Factories
{
    public static class VFXPoolFactory
    {
        public static VFXManager BuildVFXManager(
            DiContainer container,
            VFXPoolSettings settings,
            ITimerManager timerManager,
            Transform parentTransform = null,
            ILoggerResolver loggerResolver)
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

        public static IManagedPool<GameObject> BuildPool(
            DiContainer container,
            VFXPoolSettings settings,
            ITimerManager timerManager,
            Transform parentTransform = null,
            ILoggerResolver loggerResolver)
        {
            #region Builders

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
            //ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor,
            AddressDecoratorMetadataFactory.BuildAddressMetadataDescriptor,
            TimerDecoratorMetadataFactory.BuildRuntimeTimerWithPushSubscriptionMetadataDescriptor
            };

            #endregion

            ManagedPoolWithAddress<GameObject> poolWithAddress =
                AddressDecoratorPoolFactory.BuildPoolWithAddress<GameObject>(
                    loggerResolver);

            foreach (var element in settings.Elements)
            {
                #region Address

                // Get the full address of the element
                string fullAddress = element.Name;

                // Convert the address to hashes
                int[] addressHashes = fullAddress.AddressToHashes();

                // Build a set address callback
                SetAddressCallback<GameObject> setAddressCallback =
                    AddressDecoratorAllocationCallbackFactory.BuildSetAddressCallback<GameObject>(
                        fullAddress,
                        addressHashes);

                #endregion

                // Build a set runtime timer callback
                SetDurationAndPushSubscriptionCallback<GameObject> setRuntimeTimerWithPushSubscriptionCallback =
                    TimerDecoratorAllocationCallbacksFactory.BuildSetDurationAndPushSubscriptionCallback<GameObject>(
                        element.Duration,
                        loggerResolver);

                // Build a rename callback
                RenameByStringAndIndexCallback renameCallback =
                    UnityDecoratorAllocationCallbackFactory.BuildRenameByStringAndIndexCallback(element.Name);

                #region Allocation callbacks initialization

                // Create allocation callbacks
                var valueAllocationCallbacks = new IAllocationCallback<GameObject>[]
                {
                renameCallback
                };

                var facadeAllocationCallbacks = new IAllocationCallback<IPoolElementFacade<GameObject>>[]
                {
                setAddressCallback,
                setRuntimeTimerWithPushSubscriptionCallback,
                pushCallback
                };

                #endregion

                #region Value allocation delegate initialization

                // Get the prefab of the current variant
                var prefab = element.Prefab;

                // Create a value allocation delegate
                Func<GameObject> valueAllocationDelegate =
                    () => UnityZenjectAllocationFactory.DIResolveOrInstantiateAllocationDelegate(
                        container,
                        prefab);

                #endregion

                // Initialize the resizable pool builder
                managedPoolBuilder.Initialize(
                    valueAllocationDelegate,

                    metadataDescriptorBuilders,

                    element.Initial,
                    element.Additional,

                    facadeAllocationCallbacks,
                    valueAllocationCallbacks);

                // Build the resizable pool
                var resizablePool = managedPoolBuilder.BuildLinkedListManagedPool();

                // Build the game object pool
                var gameObjectPool = UnityDecoratorPoolFactory.BuildGameObjectManagedPool(
                    resizablePool,
                    parentTransform);

                // Build the pool with runtime timers
                var poolWithRuntimeTimer = TimerDecoratorPoolFactory.BuildManagedPoolWithRuntimeTimer(
                    gameObjectPool,
                    timerManager,
                    loggerResolver);

                // Parse the address and variant pool to the pool with address builder
                poolWithAddress.AddPool(
                    fullAddress,
                    poolWithRuntimeTimer);
            }

            // Set the root of the push callback
            pushCallback.TargetPool = poolWithAddress;

            return poolWithAddress;
        }
    }
}
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

namespace HereticalSolutions.Samples.PoolWithAddressVariantAndTimerSample
{
    public static class SamplePoolFactory
    {
        public static IManagedPool<GameObject> BuildPool(
            DiContainer container,
            SamplePoolSettings settings,
            ITimerManager timerManager,
            Transform parentTransform = null,
            ILoggerResolver loggerResolver = null)
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
                ObjectPoolsAllocationCallbacksFactory.BuildPushToManagedPoolWhenAvailableCallback<GameObject>();

            #endregion

            #region Metadata descriptor builders

            // Create an array of metadata descriptor builder functions
            var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
            {
                //ObjectPoolsMetadataFactory.BuildIndexedMetadataDescriptor,
                AddressDecoratorMetadataFactory.BuildAddressMetadataDescriptor,
                VariantDecoratorMetadataFactory.BuildVariantMetadataDescriptor,
                TimerDecoratorMetadataFactory.BuildRuntimeTimerWithPushSubscriptionMetadataDescriptor
            };

            #endregion

            ManagedPoolWithAddress<GameObject> poolWithAddress =
                AddressDecoratorPoolsFactory.BuildPoolWithAddress<GameObject>(
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
                    AddressDecoratorAllocationCallbacksFactory.BuildSetAddressCallback<GameObject>(
                        fullAddress,
                        addressHashes);

                #endregion

                // Initialize the pool with variants builder
                ManagedPoolWithVariants<GameObject> poolWithVariants =
                    VariantDecoratorPoolsFactory.BuildPoolWithVariants<GameObject>(
                        loggerResolver);

                for (int i = 0; i < element.Variants.Length; i++)
                {
                    #region Variant

                    var currentVariant = element.Variants[i];

                    // Build the name of the current variant
                    string currentVariantName = $"{fullAddress} (Variant {i.ToString()})";

                    // Build a set variant callback
                    SetVariantCallback<GameObject> setVariantCallback =
                        VariantDecoratorAllocationCallbacksFactory.BuildSetVariantCallback<GameObject>(i);

                    // Build a set runtime timer callback
                    SetDurationAndPushSubscriptionCallback<GameObject> setRuntimeTimerWithPushSubscriptionCallback =
                        TimerDecoratorAllocationCallbacksFactory.BuildSetDurationAndPushSubscriptionCallback<GameObject>(
                            currentVariant.Duration,
                            loggerResolver);

                    // Build a rename callback
                    RenameByStringAndIndexCallback renameCallback =
                        UnityDecoratorAllocationCallbacksFactory.BuildRenameByStringAndIndexCallback(currentVariantName);

                    #endregion

                    #region Allocation callbacks initialization

                    var valueAllocationCallbacks = new IAllocationCallback<GameObject>[]
                    {
                        renameCallback
                    };
					
                    var facadeAllocationCallbacks = new IAllocationCallback<IPoolElementFacade<GameObject>>[]
                    {
                        setAddressCallback,
                        setVariantCallback,
                        setRuntimeTimerWithPushSubscriptionCallback,
                        pushCallback
                    };

                    #endregion

                    #region Value allocation delegate initialization

                    // Get the prefab of the current variant
                    var prefab = currentVariant.Prefab;

                    // Create a value allocation delegate
                    Func<GameObject> valueAllocationDelegate =
                        () => UnityZenjectAllocationsFactory.DIResolveOrInstantiateAllocationDelegate(
                            container,
                            prefab);

                    #endregion

                    // Initialize the resizable pool builder
                    managedPoolBuilder.Initialize(
                        valueAllocationDelegate,

                        metadataDescriptorBuilders,

                        currentVariant.Initial,
                        currentVariant.Additional,
                        
                        facadeAllocationCallbacks,
                        valueAllocationCallbacks);

                    // Build the resizable pool
                    var resizablePool = managedPoolBuilder.BuildLinkedListManagedPool();

                    // Build the game object pool
                    var gameObjectPool = UnityDecoratorPoolsFactory.BuildGameObjectManagedPool(
                        resizablePool,
                        parentTransform);

                    // Build the prefab instance pool
                    var prefabInstancePool = UnityDecoratorPoolsFactory.BuildPrefabInstanceManagedPool(
                        gameObjectPool,
                        prefab);

                    // Build the pool with runtime timers
                    var poolWithRuntimeTimers = TimerDecoratorPoolsFactory.BuildManagedPoolWithRuntimeTimer(
                        prefabInstancePool,
                        timerManager,
                        loggerResolver);

                    // Add the variant to the pool with variants builder
                    poolWithVariants.AddVariant(
                        i,
                        currentVariant.Chance,
                        poolWithRuntimeTimers);
                }

                poolWithAddress.AddPool(
					fullAddress,
					poolWithVariants);
            }

            // Build the pool with ID
            var poolWithID = PoolWithIDFactory.BuildManagedPoolWithID(
                poolWithAddress,
                settings.ID,
                loggerResolver);

            // Set the root of the push callback
            pushCallback.TargetPool = poolWithID;

            return poolWithID;
        }
    }
}
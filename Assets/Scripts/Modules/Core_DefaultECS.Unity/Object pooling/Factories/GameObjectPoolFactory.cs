using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.AllocationCallbacks;
using HereticalSolutions.Pools.Decorators;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.Factories
{
	public static class GameObjectPoolFactory
	{
		public static IManagedPool<GameObject> BuildPool(
			DiContainer container,
			GameObjectPoolSettings settings,
			Transform parentTransform = null,
			ILoggerResolver loggerResolver)
		{
			#region Builders

			var managedPoolBuilder = new ManagedPoolBuilder<GameObject>(
				loggerResolver,
				loggerResolver?.GetLogger<ManagedPoolBuilder<GameObject>>());

			#endregion

			#region Callbacks

			PushToManagedPoolWhenAvailableCallback<GameObject> pushCallback =
				ObjectPoolAllocationCallbacksFactory.BuildPushToManagedPoolWhenAvailableCallback<GameObject>();

			#endregion

			#region Metadata descriptor builders

			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				//ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor,
				AddressDecoratorMetadataFactory.BuildAddressMetadataDescriptor,
				VariantDecoratorMetadataFactory.BuildVariantMetadataDescriptor
			};

			#endregion

			ManagedPoolWithAddress<GameObject> poolWithAddress =
				AddressDecoratorPoolFactory.BuildPoolWithAddress<GameObject>(
					loggerResolver);

			foreach (var element in settings.Elements)
			{
				#region Address

				// Get the full address of the element
				string fullAddress = element.GameObjectAddress;

				// Convert the address to hashes
				int[] addressHashes = fullAddress.AddressToHashes();

				// Build a set address callback
				SetAddressCallback<GameObject> setAddressCallback =
					AddressDecoratorAllocationCallbackFactory.BuildSetAddressCallback<GameObject>(
						fullAddress,
						addressHashes);

				#endregion

				ManagedPoolWithVariants<GameObject> poolWithVariants =
					VariantDecoratorPoolFactory.BuildPoolWithVariants<GameObject>(
						loggerResolver);

				for (int i = 0; i < element.Variants.Length; i++)
				{
					#region Variant

					var currentVariant = element.Variants[i];

					// Build the name of the current variant
					string currentVariantName = $"{fullAddress} (Variant {i.ToString()})";

					// Build a set variant callback
					SetVariantCallback<GameObject> setVariantCallback =
						VariantDecoratorAllocationCallbackFactory.BuildSetVariantCallback<GameObject>(i);

					// Build a rename callback
					RenameByStringAndIndexCallback renameCallback =
						UnityDecoratorAllocationCallbackFactory.BuildRenameByStringAndIndexCallback(currentVariantName);

					#endregion

					#region Allocation callbacks initialization

					// Create allocation callbacks
					var valueAllocationCallbacks = new IAllocationCallback<GameObject>[]
					{
						renameCallback
					};

					var facadeAllocationCallbacks = new IAllocationCallback<IPoolElementFacade<GameObject>>[]
					{
						setAddressCallback,
						setVariantCallback,
						pushCallback
					};

					#endregion

					#region Value allocation delegate initialization

					// Get the prefab of the current variant
					var prefab = currentVariant.Prefab;

					// Create a value allocation delegate
					Func<GameObject> valueAllocationDelegate =
						() => UnityZenjectAllocationFactory.DIResolveOrInstantiateAllocationDelegate(
							container,
							prefab);

					#endregion

					managedPoolBuilder.Initialize(
						valueAllocationDelegate,

						metadataDescriptorBuilders,

						currentVariant.Initial,
						currentVariant.Additional,

						facadeAllocationCallbacks,
						valueAllocationCallbacks);

					// Build the resizable pool
					var resizablePool = managedPoolBuilder.BuildAppendableLinkedListManagedPool();

					// Build the game object pool
					var gameObjectPool = UnityDecoratorPoolFactory.BuildGameObjectManagedPool(
						resizablePool,
						parentTransform);

					// Build the prefab instance pool
					var prefabInstancePool = UnityDecoratorPoolFactory.BuildPrefabInstanceManagedPool(
						gameObjectPool,
						prefab);

					// Add the variant to the pool with variants builder
					poolWithVariants.AddVariant(
						i,
						currentVariant.Chance,
						prefabInstancePool);
				}

				poolWithAddress.AddPool(
					fullAddress,
					poolWithVariants);
			}

			// Build the pool with ID
			var poolWithID = PoolWithIDFactory.BuildManagedPoolWithID(
				poolWithAddress,
				settings.PoolID,
				loggerResolver);

			// Set the root of the push callback
			pushCallback.TargetPool = poolWithID;

			return poolWithID;
		}
	}
}
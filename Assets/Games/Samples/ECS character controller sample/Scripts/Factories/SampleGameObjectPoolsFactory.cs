using System;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.AllocationCallbacks;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Factories
{
	public static class SampleGameObjectPoolsFactory
	{
		public static INonAllocDecoratedPool<GameObject> BuildPool(
			DiContainer container,
			SampleGameObjectPoolSettings settings,
			Transform parentTransform = null,
			ILoggerResolver loggerResolver = null)
		{
			#region Builders

			// Create a builder for pools with address.
			var poolWithAddressBuilder = new PoolWithAddressBuilder<GameObject>(
				loggerResolver,
				loggerResolver?.GetLogger<PoolWithAddressBuilder<GameObject>>());

			// Create a builder for pools with variants.
			var poolWithVariantsBuilder = new PoolWithVariantsBuilder<GameObject>(
				loggerResolver,
				loggerResolver?.GetLogger<PoolWithVariantsBuilder<GameObject>>());

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
				VariantsDecoratorsPoolsFactory.BuildVariantMetadataDescriptor
			};

			#endregion

			// Initialize the pool with address builder.
			poolWithAddressBuilder.Initialize();

			foreach (var element in settings.Elements)
			{
				#region Address

				// Get the full address of the element
				string fullAddress = element.GameObjectAddress;

				// Convert the address to hashes.
				int[] addressHashes = fullAddress.AddressToHashes();

				// Build a set address callback.
				SetAddressCallback<GameObject> setAddressCallback = AddressDecoratorsPoolsFactory.BuildSetAddressCallback<GameObject>(
					fullAddress,
					addressHashes);

				#endregion

				// Initialize the pool with variants builder.
				poolWithVariantsBuilder.Initialize();

				for (int i = 0; i < element.Variants.Length; i++)
				{
					#region Variant

					var currentVariant = element.Variants[i];

					// Build the name of the current variant.
					string currentVariantName = $"{fullAddress} (Variant {i.ToString()})";

					// Build a set variant callback.
					SetVariantCallback<GameObject> setVariantCallback =
						VariantsDecoratorsPoolsFactory.BuildSetVariantCallback<GameObject>(i);

					// Build a rename callback.
					RenameByStringAndIndexCallback renameCallback =
						UnityDecoratorsPoolsFactory.BuildRenameByStringAndIndexCallback(currentVariantName);

					#endregion

					#region Allocation callbacks initialization

					// Create an array of allocation callbacks.
					var callbacks = new IAllocationCallback<GameObject>[]
					{
						renameCallback,
						setAddressCallback,
						setVariantCallback,
						pushCallback
					};

					#endregion

					#region Value allocation delegate initialization

					// Get the prefab of the current variant.
					var prefab = currentVariant.Prefab;

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

						currentVariant.Initial,
						currentVariant.Additional,
						
						callbacks);

					// Build the resizable pool.
					var resizablePool = resizablePoolBuilder.BuildSupplyAndMergePool(); //.BuildResizablePool();

					// Build the game object pool.
					var gameObjectPool = UnityDecoratorsPoolsFactory.BuildNonAllocGameObjectPool(
						resizablePool,
						parentTransform);

					// Build the prefab instance pool.
					var prefabInstancePool = UnityDecoratorsPoolsFactory.BuildNonAllocPrefabInstancePool(
						gameObjectPool,
						prefab);

					// Add the variant to the pool with variants builder.
					poolWithVariantsBuilder.AddVariant(
						i,
						currentVariant.Chance,
						prefabInstancePool);
				}

				// Build the variant pool.
				var variantPool = poolWithVariantsBuilder.Build();

				// Parse the address and variant pool to the pool with address builder.
				poolWithAddressBuilder.Parse(
					fullAddress,
					variantPool);
			}

			// Build the pool with address.
			var poolWithAddress = poolWithAddressBuilder.Build();

			// Build the pool with ID.
			var poolWithID = IDDecoratorsPoolsFactory.BuildNonAllocPoolWithID(
				poolWithAddress,
				settings.PoolID,
				loggerResolver);

			// Set the root of the push callback.
			pushCallback.Root = poolWithID;

			return poolWithID;
		}
	}
}
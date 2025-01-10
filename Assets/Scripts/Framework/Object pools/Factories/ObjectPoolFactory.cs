using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
	public static class ObjectPoolFactory
	{
		public static IManagedPool<TValue> BuildObjectPool<TValue>(
			AllocationCommandDescriptor initialAllocation,
			AllocationCommandDescriptor additionalAllocation,
			object[] valueAllocationArguments = null,
			ILoggerResolver loggerResolver = null)
		{
			#region Builders

			var managedPoolBuilder = new ManagedPoolBuilder<TValue>(
				loggerResolver,
				loggerResolver?.GetLogger<ManagedPoolBuilder<TValue>>());

			#endregion

			#region Metadata descriptor builders

			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				//ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
			};

			#endregion

			#region Value allocation delegate initialization

			Func<TValue> valueAllocationDelegate;

			if (valueAllocationArguments == null)
			{
				valueAllocationDelegate =
					() => AllocationFactory.ActivatorAllocationDelegate<TValue>();
			}
			else
			{
				valueAllocationDelegate =
					() => AllocationFactory.ActivatorAllocationDelegate<TValue>(
						valueAllocationArguments);
			}

			#endregion

			managedPoolBuilder.Initialize(
				valueAllocationDelegate,

				metadataDescriptorBuilders,

				initialAllocation,
				additionalAllocation);

			var resizablePool = managedPoolBuilder.BuildLinkedListManagedPool();

			return resizablePool;
		}

		public static IManagedPool<TAbstractValue> BuildManagedObjectPool<TAbstractValue, TConcreteValue>(
			AllocationCommandDescriptor initialAllocation,
			AllocationCommandDescriptor additionalAllocation,
			ILoggerResolver loggerResolver = null,
			object[] valueAllocationArguments = null)
		{
			#region Builders

			var managedPoolBuilder = new ManagedPoolBuilder<TAbstractValue>(
				loggerResolver,
				loggerResolver?.GetLogger<ManagedPoolBuilder<TAbstractValue>>());

			#endregion

			#region Metadata descriptor builders

			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				//ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
			};

			#endregion

			#region Value allocation delegate initialization

			Func<TAbstractValue> valueAllocationDelegate;

			if (valueAllocationArguments == null)
			{
				valueAllocationDelegate =
					() => AllocationFactory.ActivatorAllocationDelegate<TAbstractValue, TConcreteValue>();
			}
			else
			{
				valueAllocationDelegate =
					() => AllocationFactory.ActivatorAllocationDelegate<TAbstractValue, TConcreteValue>(
						valueAllocationArguments);
			}

			#endregion

			managedPoolBuilder.Initialize(
				valueAllocationDelegate,

				metadataDescriptorBuilders,

				initialAllocation,
				additionalAllocation);

			var resizablePool = managedPoolBuilder.BuildLinkedListManagedPool();

			return resizablePool;
		}
	}
}
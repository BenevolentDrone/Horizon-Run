using System.Collections.Generic;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;
using System;

namespace HereticalSolutions.Hierarchy.Factories
{
	public static class HierarchyFactory
	{
		#region Factory settings

		#region Hierarchy node list pool

		public const int DEFAULT_HIERARCHY_NODE_LIST_POOL_SIZE = 32;

		public static AllocationCommandDescriptor HierarchyNodeListPoolInitialAllocationDescriptor =
			new AllocationCommandDescriptor
			{
				Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

				Amount = DEFAULT_HIERARCHY_NODE_LIST_POOL_SIZE
			};

		public static AllocationCommandDescriptor HierarchyNodeListPoolAdditionalAllocationDescriptor =
			new AllocationCommandDescriptor
			{
				Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

				Amount = DEFAULT_HIERARCHY_NODE_LIST_POOL_SIZE
			};

		#endregion

		#endregion

		public static IPool<List<IReadOnlyHierarchyNode<T>>> BuildHierarchyNodeListPool<T>(
			ILoggerResolver loggerResolver)
		{
			return BuildHierarchyNodeListPool<T>(
				HierarchyNodeListPoolInitialAllocationDescriptor,
				HierarchyNodeListPoolAdditionalAllocationDescriptor,
				loggerResolver);
		}

		public static IPool<List<IReadOnlyHierarchyNode<T>>> BuildHierarchyNodeListPool<T>(
			AllocationCommandDescriptor initialAllocationDescriptor,
			AllocationCommandDescriptor additionalAllocationDescriptor,
			ILoggerResolver loggerResolver)
		{
			Func<List<IReadOnlyHierarchyNode<T>>> allocationDelegate = AllocationFactory
				.ActivatorAllocationDelegate<List<IReadOnlyHierarchyNode<T>>>;

			return new PoolWithListCleanup<List<IReadOnlyHierarchyNode<T>>>(
				StackPoolFactory.BuildStackPool<List<IReadOnlyHierarchyNode<T>>>(
					new AllocationCommand<List<IReadOnlyHierarchyNode<T>>>
					{
						Descriptor = initialAllocationDescriptor,

						AllocationDelegate = allocationDelegate
					},
					new AllocationCommand<List<IReadOnlyHierarchyNode<T>>>
					{
						Descriptor = additionalAllocationDescriptor,

						AllocationDelegate = allocationDelegate
					},
					loggerResolver));
		}
	}
}
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;
using System;

namespace HereticalSolutions.SpacePartitioning.Factories
{
	public static class NodePoolFactory
	{
		private const int INITIAL_NODE_POOL_SIZE = 128;

		public static IPool<Node<TValue>> BuildNodePool<TValue>(
			ILoggerResolver loggerResolver)
		{
			ILogger logger =
				loggerResolver?.GetLogger<Node<TValue>>();

			Func<Node<TValue>> allocationDelegate = () => new Node<TValue>(
				new Node<TValue>[4],
				new List<ValueSpaceData<TValue>>(),
				-1,
				logger);

			return StackPoolFactory.BuildStackPool<Node<TValue>>(
				new AllocationCommand<Node<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = 5
					},

					AllocationDelegate = allocationDelegate
				},
				new AllocationCommand<Node<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = INITIAL_NODE_POOL_SIZE
					},

					AllocationDelegate = allocationDelegate
				},
				loggerResolver);
		}

		public static IPool<ValueSpaceData<TValue>> BuildValueDataPool<TValue>(
			ILoggerResolver loggerResolver)
		{
			Func<ValueSpaceData<TValue>> allocationDelegate = () => new ValueSpaceData<TValue>();

			return StackPoolFactory.BuildStackPool<ValueSpaceData<TValue>>(
				new AllocationCommand<ValueSpaceData<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = 5
					},
					AllocationDelegate = allocationDelegate,
				},
				new AllocationCommand<ValueSpaceData<TValue>>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

						Amount = INITIAL_NODE_POOL_SIZE
					},
					AllocationDelegate = allocationDelegate,
				},
				loggerResolver);
		}
	}
}
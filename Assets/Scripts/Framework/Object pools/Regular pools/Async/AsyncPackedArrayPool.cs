using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class AsyncPackedArrayPool<T>
		: IAsyncPool<T>,
		  IAsyncAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly AsyncAllocationCommand<T> allocationCommand;

		private readonly object lockObject;

		private readonly ILogger logger;

		private T[] packedArray;

		private int allocatedCount;

		private bool isResizing;

		public AsyncPackedArrayPool(
			T[] packedArray,
			AsyncAllocationCommand<T> allocationCommand,
			ILogger logger)
		{
			this.packedArray = packedArray;

			this.allocationCommand = allocationCommand;

			this.logger = logger;


			lockObject = new object();


			allocatedCount = 0;

			isResizing = false;
		}

		#region IAsyncPool

		public async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			T result = default(T);

			#region Wait for resize

			bool isResizingClosure = false;

			lock (lockObject)
			{
				isResizingClosure = isResizing;
			}

			while (isResizingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isResizingClosure = isResizing;
				}
			}

			#endregion

			lock (lockObject)
			{

				if (allocatedCount < packedArray.Length)
				{
					result = packedArray[allocatedCount];

					allocatedCount++;

					return result;
				}

				isResizing = true;
			}

			packedArray = await PackedArrayPoolFactory.ResizeAsyncPackedArrayPool(
				packedArray,
				allocationCommand,
				logger,
				
				asyncContext);

			lock (lockObject)
			{
				result = packedArray[allocatedCount];

				allocatedCount++;

				isResizing = false;

				return result;
			}
		}

		public async Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await Pop(
				args,

				asyncContext);
		}

		public async Task Push(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			#region Wait for resize

			bool isResizingClosure = false;

			lock (lockObject)
			{
				isResizingClosure = isResizing;
			}

			while (isResizingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isResizingClosure = isResizing;
				}
			}

			#endregion

			lock (lockObject)
			{
				int lastAllocatedItemIndex = allocatedCount - 1;

				if (lastAllocatedItemIndex < 0)
				{
					logger?.LogError(
						GetType(),
						"ATTEMPT TO PUSH AN ITEM WHEN NO ITEMS ARE ALLOCATED");

					return;
				}

				int instanceIndex = Array.IndexOf(packedArray, instance);

				if (instanceIndex == -1)
				{
					logger?.LogError(
						GetType(),
						"ATTEMPT TO PUSH AN ITEM TO PACKED ARRAY IT DOES NOT BELONG TO");

					return;
				}

				if (instanceIndex > lastAllocatedItemIndex)
				{
					logger?.LogError(
						GetType(),
						$"ATTEMPT TO PUSH AN ALREADY PUSHED ITEM: {instanceIndex}");

					return;
				}

				if (instanceIndex != lastAllocatedItemIndex)
				{
					// Swap pushed element and last allocated element

					var swap = packedArray[instanceIndex];

					packedArray[instanceIndex] = packedArray[lastAllocatedItemIndex];

					packedArray[lastAllocatedItemIndex] = swap;
				}

				allocatedCount--;
			}
		}

		#endregion

		#region IAsyncAllocationResizeable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			#region Wait for resize

			bool isResizingClosure = false;

			lock (lockObject)
			{
				isResizingClosure = isResizing;
			}

			while (isResizingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isResizingClosure = isResizing;
				}
			}

			#endregion

			lock (lockObject)
			{
				isResizing = true;
			}

			packedArray = await PackedArrayPoolFactory.ResizeAsyncPackedArrayPool(
				packedArray,
				allocationCommand,
				logger,
				
				asyncContext);

			lock (lockObject)
			{
				isResizing = false;
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				foreach (var item in packedArray)
				{
					if (item is ICleanuppable cleanItem)
					{
						cleanItem.Cleanup();
					}
				}

				allocatedCount = 0;
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				foreach (var item in packedArray)
				{
					if (item is IDisposable disposableItem)
					{
						disposableItem.Dispose();
					}
				}

				for (int i = 0; i < packedArray.Length; i++)
				{
					packedArray[i] = default;
				}

				allocatedCount = 0;
			}
		}

		#endregion
	}
}
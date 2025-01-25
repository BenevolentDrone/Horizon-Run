/*
using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;


using HereticalSolutions.Collections.Managed;

using HereticalSolutions.Messaging.Concurrent;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AResourceStorageHandle<TResource>
		: ICleanuppable,
		  IDisposable
	{
		protected readonly IRuntimeResourceManager runtimeResourceManager;

		protected readonly ILogger logger;


		protected bool allocated = false;

		protected TResource resource = default;

		public AResourceStorageHandle(
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger)
		{
			this.runtimeResourceManager = runtimeResourceManager;

			this.logger = logger;


			resource = default;

			allocated = false;
		}

		#region ICleanUppable

		public void Cleanup()
		{
			if (resource is ICleanuppable)
				(resource as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (resource is IDisposable)
				(resource as IDisposable).Dispose();
		}

		#endregion

		protected abstract Task<TResource> AllocateResource(

			//Async tail
			AsyncExecutionContext asyncContext);

		protected abstract Task FreeResource(
			TResource resource,

			//Async tail
			AsyncExecutionContext asyncContext);

		protected async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var task = ((IContainsDependencyResources)runtimeResourceManager)
				.LoadDependency(
					path,
					variantID,
					
					asyncContext);

			var result = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny<IReadOnlyResourceStorageHandle>(
					GetType(),
					logger);

			return result;
		}

		protected async Task ExecuteOnMainThread(
			Action delegateToExecute,
			IConcurrentCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var command = new MainThreadCommand(
				delegateToExecute);

			while (!mainThreadCommandBuffer.TryProduce(
				command,
				out _))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}
		}

		protected async Task ExecuteOnMainThread(
			Func<Task> asyncDelegateToExecute,
			IConcurrentCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var command = new MainThreadCommand(
				asyncDelegateToExecute);

			while (!mainThreadCommandBuffer.TryProduce(
				command,
				out _))
			{
				await Task.Yield();
			}

			while (command.Status != ECommandStatus.DONE)
			{
				await Task.Yield();
			}
		}
	}
}
*/
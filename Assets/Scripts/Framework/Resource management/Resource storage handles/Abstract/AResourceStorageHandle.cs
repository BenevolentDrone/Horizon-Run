using System;
using System.Threading;
using System.Threading.Tasks;

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
			ILogger logger = null)
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
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		protected abstract Task FreeResource(
			TResource resource,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		protected async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null)
		{
			var task = ((IContainsDependencyResources)runtimeResourceManager)
				.LoadDependency(
					path,
					variantID,
					progress: progress);

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
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			var command = new MainThreadCommand(
				delegateToExecute);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
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
			IGenericCircularBuffer<MainThreadCommand> mainThreadCommandBuffer,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			var command = new MainThreadCommand(
				asyncDelegateToExecute);

			while (!mainThreadCommandBuffer.TryProduce(
				command))
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
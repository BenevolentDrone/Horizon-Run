using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.ResourceManagement
{
	public abstract class AReadOnlyResourceStorageHandle<TResource>
		: AResourceStorageHandle<TResource>,
		  IReadOnlyResourceStorageHandle,
		  ICleanuppable,
		  IDisposable
	{
		public AReadOnlyResourceStorageHandle(
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger = null)
			: base (
				runtimeResourceManager,
				logger)
		{
		}

		#region IReadOnlyResourceStorageHandle

		#region IAllocatable

		public bool Allocated
		{
			get => allocated;
		}

		public virtual async Task Allocate(

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			if (allocated)
			{
				progress?.Report(1f);

				return;
			}

			logger?.Log(
				GetType(),
				$"ALLOCATING");

			var task = AllocateResource(
				progress);

			resource = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			allocated = true;

			logger?.Log(
				GetType(),
				$"ALLOCATED");

			progress?.Report(1f);
		}

		public virtual async Task Free(
			
			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			if (!allocated)
			{
				progress?.Report(1f);

				return;
			}

			logger?.Log(
				GetType(),
				$"FREEING");

			var task = FreeResource(
				resource,
				progress);

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			resource = default;

			allocated = false;

			logger?.Log(
				GetType(),
				$"FREE");

			progress?.Report(1f);
		}

		#endregion

		public object RawResource
		{
			get
			{
				if (!allocated)
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"RESOURCE IS NOT ALLOCATED"));

				return resource;
			}
		}

		public TValue GetResource<TValue>()
		{
			if (!allocated)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"RESOURCE IS NOT ALLOCATED"));

			switch (resource)
			{
				case TValue targetTypeResource:

					return targetTypeResource;

				default:

					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"CANNOT GET RESOURCE OF TYPE {typeof(TValue).Name} FROM RESOURCE OF TYPE {typeof(TResource).Name}"));
			}
		}

		#endregion

		#region ICleanUppable

		public new void Cleanup()
		{
			Free();
		}

		#endregion

		#region IDisposable

		public new void Dispose()
		{
			Free();
		}

		#endregion
	}
}
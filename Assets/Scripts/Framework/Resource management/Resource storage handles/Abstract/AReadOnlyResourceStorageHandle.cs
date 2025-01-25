/*
using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

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
			ILogger logger)
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
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			if (allocated)
			{
				asyncContext?.Progress?.Report(1f);

				return;
			}

			logger?.Log(
				GetType(),
				$"ALLOCATING");

			var task = AllocateResource(
				asyncContext);

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

			asyncContext?.Progress?.Report(1f);
		}

		public virtual async Task Free(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			if (!allocated)
			{
				asyncContext?.Progress?.Report(1f);

				return;
			}

			logger?.Log(
				GetType(),
				$"FREEING");

			var task = FreeResource(
				resource,
				asyncContext);

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

			asyncContext?.Progress?.Report(1f);
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
							$"CANNOT GET RESOURCE OF TYPE {nameof(TValue)} FROM RESOURCE OF TYPE {nameof(TResource)}"));
			}
		}

		#endregion

		#region ICleanUppable

		public new void Cleanup()
		{
			Free(null);
		}

		#endregion

		#region IDisposable

		public new void Dispose()
		{
			Free(null);
		}

		#endregion
	}
}
*/
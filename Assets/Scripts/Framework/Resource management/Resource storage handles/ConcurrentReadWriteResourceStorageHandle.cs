/*
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentReadWriteResourceStorageHandle<TResource>
		: AConcurrentReadWriteResourceStorageHandle<TResource>
	{
		private TResource defaultValue;

		public ConcurrentReadWriteResourceStorageHandle(
			TResource defaultValue,
			SemaphoreSlim semaphore,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger)
			: base(
				semaphore,
				runtimeResourceManager,
				logger)
		{
			this.defaultValue = defaultValue;
		}

		protected override async Task<TResource> AllocateResource(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return defaultValue;
		}

		protected override async Task FreeResource(
			TResource resource,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
		}
	}
}
*/
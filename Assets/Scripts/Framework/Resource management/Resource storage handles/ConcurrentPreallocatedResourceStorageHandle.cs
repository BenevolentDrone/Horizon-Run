/*
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentPreallocatedResourceStorageHandle<TResource>
		: AConcurrentReadOnlyResourceStorageHandle<TResource>
	{
		private TResource value;

		public ConcurrentPreallocatedResourceStorageHandle(
			TResource value,
			SemaphoreSlim semaphore,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger)
			: base(
				semaphore,
				runtimeResourceManager,
				logger)
		{
			this.value = value;
		}

        protected override async Task<TResource> AllocateResource(

			//Async tail
			AsyncExecutionContext asyncContext)
        {
			return value;
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
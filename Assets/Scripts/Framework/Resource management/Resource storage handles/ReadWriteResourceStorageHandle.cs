/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ReadWriteResourceStorageHandle<TResource>
		: AReadWriteResourceStorageHandle<TResource>
	{
		private TResource defaultValue;

		public ReadWriteResourceStorageHandle(
			TResource defaultValue,
			IRuntimeResourceManager runtimeResourceManager,
			ILogger logger)
			: base(
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
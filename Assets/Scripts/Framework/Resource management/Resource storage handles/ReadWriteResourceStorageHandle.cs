using System;
using System.Threading;
using System.Threading.Tasks;

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
			ILogger logger = null)
			: base(
				runtimeResourceManager,
				logger)
		{
			this.defaultValue = defaultValue;
		}

		protected override async Task<TResource> AllocateResource(

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			return defaultValue;
		}

		protected override async Task FreeResource(
			TResource resource,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
		}
	}
}
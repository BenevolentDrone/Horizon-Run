using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public interface IContainsDependencyResources
	{
		Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IReadOnlyResourceData> GetDependencyResource(
			string path,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}
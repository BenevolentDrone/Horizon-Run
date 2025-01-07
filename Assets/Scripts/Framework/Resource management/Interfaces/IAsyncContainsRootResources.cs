using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public interface IAsyncContainsRootResources
	{
		#region Get

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(
			int rootResourceIDHash,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(
			string rootResourceID,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(
			int[] resourcePathPartHashes,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IReadOnlyResourceData> GetResourceWhenAvailable(
			string[] resourcePathParts,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion

		#region Get default

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(
			int rootResourceIDHash,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(
			string rootResourceID,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(
			int[] resourcePathPartHashes,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		Task<IResourceVariantData> GetDefaultResourceWhenAvailable(
			string[] resourcePathParts,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);

		#endregion
	}
}
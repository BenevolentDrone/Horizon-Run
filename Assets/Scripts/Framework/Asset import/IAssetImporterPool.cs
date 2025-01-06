using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Pools;

using HereticalSolutions. Logging;

namespace HereticalSolutions.AssetImport
{
	public interface IAssetImporterPool
	{
		Task<IPoolElementFacade<AAssetImporter>> PopImporter<TImporter>(

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
			where TImporter : AAssetImporter;

		Task PushImporter(
			IPoolElementFacade<AAssetImporter> pooledImporter,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}
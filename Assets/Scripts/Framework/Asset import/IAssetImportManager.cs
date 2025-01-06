using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public interface IAssetImportManager
		: IAssetImporterPool
	{
		Task<IResourceVariantData> Import<TImporter>(
			Action<TImporter> initializationDelegate = null,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
			where TImporter : AAssetImporter;

		Task RegisterPostProcessor<TImporter, TPostProcessor>(
			TPostProcessor instance,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
			where TImporter : AAssetImporter
			where TPostProcessor : AAssetImportPostProcessor;
	}
}
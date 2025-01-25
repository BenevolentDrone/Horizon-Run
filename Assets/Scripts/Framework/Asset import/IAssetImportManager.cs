/*
using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ResourceManagement;

namespace HereticalSolutions.AssetImport
{
	public interface IAssetImportManager
		: IAssetImporterPool
	{
		Task<IResourceVariantData> Import<TImporter>(
			Action<TImporter> initializationDelegate = null,

			//Async tail
			AsyncExecutionContext asyncContext)
			where TImporter : AAssetImporter;

		Task RegisterPostProcessor<TImporter, TPostProcessor>(
			TPostProcessor instance,

			//Async tail
			AsyncExecutionContext asyncContext)
			where TImporter : AAssetImporter
			where TPostProcessor : AAssetImportPostProcessor;
	}
}
*/
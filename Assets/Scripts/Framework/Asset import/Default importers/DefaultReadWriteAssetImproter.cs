/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public class DefaultReadWriteAssetImporter<TAsset>
		: AAssetImporter
	{
		private string resourcePath;

		private TAsset readWriteAsset;

		public DefaultReadWriteAssetImporter(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		public void Initialize(
			IRuntimeResourceManager runtimeResourceManager,
			string resourcePath,
			TAsset readWriteAsset)
		{
			InitializeInternal(runtimeResourceManager);

			this.resourcePath = resourcePath;

			this.readWriteAsset = readWriteAsset;
		}

		public override async Task<IResourceVariantData> Import(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			logger?.Log(
				GetType(),
				$"IMPORTING {resourcePath} INITIATED");

			asyncContext?.Progress?.Report(0f);

			var getOrCreateResourceTask = GetOrCreateResourceData(
				resourcePath,
				
				asyncContext);

			var resource = await getOrCreateResourceTask;
				//.ConfigureAwait(false);

			await getOrCreateResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);


			var addAsVariantTask = AddAssetAsResourceVariant(
				resource,
				new ResourceVariantDescriptor()
				{
					VariantID = string.Empty,
					VariantIDHash = string.Empty.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.RUNTIME_GENERATED,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TAsset),
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentReadWriteResourceStorageHandle<TAsset>(
					readWriteAsset,
					runtimeResourceManager,
					loggerResolver),
#else
				ResourceManagementFactory.BuildReadWriteResourceStorageHandle<TAsset>(
					readWriteAsset,
					runtimeResourceManager,
					loggerResolver),
#endif
				true,
				asyncContext);

			var result = await addAsVariantTask;
				//.ConfigureAwait(false);

			await addAsVariantTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			asyncContext?.Progress?.Report(1f);

			logger?.Log(
				GetType(),
				$"IMPORTING {resourcePath} FINISHED");

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;

			readWriteAsset = default;
		}
	}
}
*/
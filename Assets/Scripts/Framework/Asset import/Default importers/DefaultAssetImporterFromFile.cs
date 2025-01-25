/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Persistence;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public abstract class DefaultAssetImporterFromFile<TAsset>
		: AAssetImporter
	{
		protected string resourcePath;

		protected ISerializer serializer;

		public DefaultAssetImporterFromFile(
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
			ISerializer serializer)
		{
			InitializeInternal(runtimeResourceManager);

			this.resourcePath = resourcePath;

			this.serializer = serializer;
		}

		public override async Task<IResourceVariantData> Import(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			serializer.Deserialize<TAsset>(
				out var asset);

			var task = AddAssetAsResourceToManager(
				asset,
				true,
				asyncContext);

			var result = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny<IResourceVariantData>(
					GetType(),
					logger);

			asyncContext?.Progress?.Report(1f);

			return result;
		}

		public override void Cleanup()
		{
			resourcePath = null;

			serializer = null;
		}

		protected virtual async Task<IResourceVariantData> AddAssetAsResourceToManager(
			TAsset asset,
			bool allocate = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			var getOrCreateResourceTask = GetOrCreateResourceData(
				resourcePath,
				
				asyncContext);

			var resource = await getOrCreateResourceTask;
				//.ConfigureAwait(false);

			await getOrCreateResourceTask
				.ThrowExceptionsIfAny<IResourceData>(
					GetType(),
					logger);


			var addAsVariantTask = AddAssetAsResourceVariant(
				resource,
				new ResourceVariantDescriptor()
				{
					VariantID = string.Empty,
					VariantIDHash = string.Empty.AddressToHash(),
					Priority = 0,
					Source = EResourceSources.LOCAL_STORAGE,
					Storage = EResourceStorages.RAM,
					ResourceType = typeof(TAsset)
				},
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory
					.BuildConcurrentPreallocatedResourceStorageHandle<TAsset>(
						asset,
						runtimeResourceManager,
						loggerResolver),
#else
				ResourceManagementFactory
					.BuildPreallocatedResourceStorageHandle<TAsset>(
						asset,
						runtimeResourceManager,
						loggerResolver),
#endif
				allocate,
				progress: progress);

			var result = await addAsVariantTask;
				//.ConfigureAwait(false);

			await addAsVariantTask
				.ThrowExceptionsIfAny<IResourceVariantData>(
					GetType(),
					logger);

			asyncContext?.Progress?.Report(1f);

			return result;
		}
	}
}
*/
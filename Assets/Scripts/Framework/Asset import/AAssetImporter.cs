/*
using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public abstract class AAssetImporter
	{
		protected readonly ILoggerResolver loggerResolver;

		protected readonly ILogger logger;

		protected IRuntimeResourceManager runtimeResourceManager;

		public AAssetImporter(
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		public virtual async Task<IResourceVariantData> Import(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			throw new NotImplementedException();
		}

		protected void InitializeInternal(IRuntimeResourceManager runtimeResourceManager)
		{
			this.runtimeResourceManager = runtimeResourceManager;
		}

		public virtual void Cleanup()
		{
			runtimeResourceManager = null;
		}

		protected virtual async Task<IResourceData> GetOrCreateResourceData(
			string fullResourcePath,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (runtimeResourceManager == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"RUNTIME RESOURCE MANAGER IS NOT INITIALIZED"));
			}

			string[] resourcePathParts = fullResourcePath.SplitAddressBySeparator();

			IReadOnlyResourceData currentData = null;

			if (!runtimeResourceManager.TryGetRootResource(
				resourcePathParts[0],
				out currentData))
			{
				var descriptor = new ResourceDescriptor()
				{
					ID = resourcePathParts[0],

					IDHash = resourcePathParts[0].AddressToHash(),

					FullPath = resourcePathParts[0]
				};

				currentData =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
					ResourceManagementFactory.BuildConcurrentResourceData(
						descriptor,
						loggerResolver);
#else
					ResourceManagementFactory.BuildResourceData(
						descriptor,
						loggerResolver);
#endif

				var addRootResourceTask = runtimeResourceManager.AddRootResource(
					currentData,
					
					asyncContext);

				await addRootResourceTask;
					//.ConfigureAwait(false);

				await addRootResourceTask
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			for (int i = 1; i < resourcePathParts.Length; i++)
			{
				IReadOnlyResourceData newCurrentData;

				if (currentData.TryGetNestedResource(
					resourcePathParts[i],
					out newCurrentData))
				{
					currentData = newCurrentData;
				}
				else
				{
					var descriptor = new ResourceDescriptor()
					{
						ID = resourcePathParts[i],

						IDHash = resourcePathParts[i].AddressToHash(),

						//TODO: check if works correctly
						FullPath = resourcePathParts.PartialAddress(i)
					};

					newCurrentData =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
						ResourceManagementFactory.BuildConcurrentResourceData(
							descriptor,
							loggerResolver);
#else						
						ResourceManagementFactory.BuildResourceData(
							descriptor,
							loggerResolver);
#endif

					var addNestedResourceTask = ((IResourceData)currentData)
						.AddNestedResource(
							newCurrentData,
							
							asyncContext);

					await addNestedResourceTask;
						//.ConfigureAwait(false);

					await addNestedResourceTask
						.ThrowExceptionsIfAny(
							GetType(),
							logger);

					currentData = newCurrentData;
				}
			}

			return (IResourceData)currentData;
		}

		protected virtual async Task<IResourceData> GetOrCreateNestedResourceData(
			string parentResourcePath,
			string nestedResourceID,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var getOrCreateResourceTask = GetOrCreateResourceData(
				parentResourcePath,
				
				asyncContext);

			var parent = await getOrCreateResourceTask;
				//.ConfigureAwait(false);

			await getOrCreateResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			var descriptor = new ResourceDescriptor()
			{
				ID = nestedResourceID,

				IDHash = nestedResourceID.AddressToHash(),

				FullPath = $"{parentResourcePath}/{nestedResourceID}"
			};

			IReadOnlyResourceData child =
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
				ResourceManagementFactory.BuildConcurrentResourceData(
					descriptor,
					loggerResolver);
#else				
				ResourceManagementFactory.BuildResourceData(
					descriptor,
					loggerResolver);
#endif

			var addNestedResourceTask = parent
				.AddNestedResource(
					child,
					
					asyncContext);

			await addNestedResourceTask;
				//.ConfigureAwait(false);

			await addNestedResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return (IResourceData)child;
		}

		protected virtual async Task<IResourceVariantData> AddAssetAsResourceVariant(
			IResourceData resourceData,
			ResourceVariantDescriptor variantDescriptor,
			IReadOnlyResourceStorageHandle resourceStorageHandle,
			bool allocate = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			var variantData = ResourceManagementFactory.BuildResourceVariantData(
				variantDescriptor,
				resourceStorageHandle,
				resourceData);

			var task = resourceData
				.AddVariant(
					variantData,
					allocate,
					
					asyncContext);

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			asyncContext?.Progress?.Report(1f);

			return variantData;
		}

		protected async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (runtimeResourceManager == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"RUNTIME RESOURCE MANAGER IS NOT INITIALIZED"));
			}

			var task = ((IContainsDependencyResources)runtimeResourceManager)
				.LoadDependency(
					path,
					variantID,
					
					asyncContext);

			var result = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}
	}
}
*/
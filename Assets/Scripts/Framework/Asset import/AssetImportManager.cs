using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public class AssetImportManager
		: IAssetImportManager
	{
		private const int DEFAULT_IMPORTER_POOL_CAPACITY = 4;

		private static readonly AllocationCommandDescriptor initialAllocation = new AllocationCommandDescriptor
		{
			Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

			Amount = DEFAULT_IMPORTER_POOL_CAPACITY
		};

		private static readonly AllocationCommandDescriptor additionalAllocation = new AllocationCommandDescriptor
		{
			Rule = EAllocationAmountRule.DOUBLE_AMOUNT,

			Amount = 0
		};

		private readonly IRepository<Type, List<AAssetImportPostProcessor>> postProcessorRepository;

		private readonly IRepository<Type, IManagedPool<AAssetImporter>> importerPoolRepository;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public AssetImportManager(
			IRepository<Type, List<AAssetImportPostProcessor>> postProcessorRepository,
			IRepository<Type, IManagedPool<AAssetImporter>> importerPoolRepository,
			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.postProcessorRepository = postProcessorRepository;

			this.importerPoolRepository = importerPoolRepository;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region IAssetImportManager

		public async Task<IResourceVariantData> Import<TImporter>(
			Action<TImporter> initializationDelegate = null,

			//Async tail
			AsyncExecutionContext asyncContext)
			where TImporter : AAssetImporter
		{
			logger?.Log(
				GetType(),
				$"IMPORTING {nameof(TImporter)} INITIATED");

			var pooledImporter = PopImporterSync<TImporter>();

			initializationDelegate?.Invoke(
				pooledImporter.Value as TImporter);

			var importTask = pooledImporter.Value.Import(
				asyncContext);

			var result = await importTask;
				//.ConfigureAwait(false);

			await importTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			if (postProcessorRepository.Has(typeof(TImporter)))
			{
				var postProcessors = postProcessorRepository.Get(
					typeof(TImporter));

				for (int i = 0; i < postProcessors.Count; i++)
				{
					//This one should NOT be paralleled
					//Imagine one post processor adding 10 exrta vertices to the mesh while the other one substracts 20
					//Why? For any fucking reason. PostProcessors are exposed as API and I have zero idea what kind of shit
					//would a user come up with its usage
					var postProcessorTask = postProcessors[i].OnImport(
						result,

						asyncContext);

					await postProcessorTask;
						//.ConfigureAwait(false);

					await postProcessorTask
						.ThrowExceptionsIfAny(
							GetType(),
							logger);
				}
			}

			pooledImporter.Value.Cleanup();

			pooledImporter.Push();

			logger?.Log(
				GetType(),
				$"IMPORTING {nameof(TImporter)} FINISHED");

			return result;
		}

		public async Task RegisterPostProcessor<TImporter, TPostProcessor>(
			TPostProcessor instance,

			//Async tail
			AsyncExecutionContext asyncContext)
			where TImporter : AAssetImporter
			where TPostProcessor : AAssetImportPostProcessor
		{
			if (!postProcessorRepository.Has(typeof(TImporter)))
			{
				postProcessorRepository.Add(
					typeof(TImporter),
					new List<AAssetImportPostProcessor>());
			}

			var postProcessors = postProcessorRepository.Get(
				typeof(TImporter));

			postProcessors.Add(
				instance);
		}

		#region IAssetImporterPool

		public async Task<IPoolElementFacade<AAssetImporter>> PopImporter<TImporter>(

			//Async tail
			AsyncExecutionContext asyncContext)
			where TImporter : AAssetImporter
		{
			return PopImporterSync<TImporter>();
		}

		public async Task PushImporter(
			IPoolElementFacade<AAssetImporter> pooledImporter,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			pooledImporter.Value.Cleanup();

			pooledImporter.Push();
		}

		#endregion

		#endregion

		private IPoolElementFacade<AAssetImporter> PopImporterSync<TImporter>()
			where TImporter : AAssetImporter
		{
			IManagedPool<AAssetImporter> importerPool;

			if (!importerPoolRepository.Has(typeof(TImporter)))
			{
				importerPool = ObjectPoolFactory.BuildManagedObjectPool<AAssetImporter, TImporter>(
					initialAllocation,
					additionalAllocation,
					loggerResolver,
					new object[]
					{
						loggerResolver,
						(loggerResolver?.GetLogger<TImporter>())
					});

				importerPoolRepository.Add(
					typeof(TImporter),
					importerPool);
			}
			else
			{
				importerPool = importerPoolRepository.Get(
					typeof(TImporter));
			}

			return importerPool.Pop(null);
		}
	}
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
	public class ConcurrentAssetImportManager
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

		private readonly IRepository<Type, INonAllocDecoratedPool<AAssetImporter>> importerPoolRepository;

		private readonly SemaphoreSlim importerPoolSemaphore;

		private readonly SemaphoreSlim postProcessorsSemaphore;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public ConcurrentAssetImportManager(
			IRepository<Type, List<AAssetImportPostProcessor>> postProcessorRepository,
			IRepository<Type, INonAllocDecoratedPool<AAssetImporter>> importerPoolRepository,
			SemaphoreSlim importerPoolSemaphore,
			SemaphoreSlim postProcessorsSemaphore,
			ILoggerResolver loggerResolver = null,
			ILogger logger = null)
		{
			this.postProcessorRepository = postProcessorRepository;

			this.importerPoolRepository = importerPoolRepository;

			this.importerPoolSemaphore = importerPoolSemaphore;

			this.postProcessorsSemaphore = postProcessorsSemaphore;

			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region IAssetImportManager

		public async Task<IResourceVariantData> Import<TImporter>(
			Action<TImporter> initializationDelegate = null,
			IProgress<float> progress = null)
			where TImporter : AAssetImporter
		{
			logger?.Log<ConcurrentAssetImportManager>($"IMPORTING {typeof(TImporter).Name} INITIATED");

			var importer = await PopImporter<TImporter>()
				.ThrowExceptions< IPoolElement<AAssetImporter>, AssetImportManager>(
					logger);

			initializationDelegate?.Invoke(
				importer.Value as TImporter);

			var result = await importer.Value.Import(
				progress)
				.ThrowExceptions<IResourceVariantData, AssetImportManager>(
					logger);

			AAssetImportPostProcessor[] postProcessors = null;

			await postProcessorsSemaphore.WaitAsync();

			try
			{
				if (postProcessorRepository.Has(typeof(TImporter)))
				{
					postProcessors = postProcessorRepository
						.Get(
							typeof(TImporter))
						.ToArray();
				}
			}
			finally
			{
				postProcessorsSemaphore.Release();
			}

			if (postProcessors != null)
			{
				for (int i = 0; i < postProcessors.Length; i++)
				{
					//This one should NOT be paralleled
					//Imagine one post processor adding 10 exrta vertices to the mesh while the other one substracts 20
					//Why? For any fucking reason. PostProcessors are exposed as API and I have zero idea what kind of shit
					//would a user come up with its usage
					await postProcessors[i].OnImport(
						result,
						progress)
						.ThrowExceptions<AssetImportManager>(
							logger);
				}
			}

			await PushImporter(importer);

			logger?.Log<ConcurrentAssetImportManager>($"IMPORTING {typeof(TImporter).Name} FINISHED");

			return result;
		}

		public async Task RegisterPostProcessor<TImporter, TPostProcessor>(
			TPostProcessor instance)
			where TImporter : AAssetImporter
			where TPostProcessor : AAssetImportPostProcessor
		{
			await postProcessorsSemaphore.WaitAsync();

			try
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
			finally
			{
				postProcessorsSemaphore.Release();
			}
		}

		#region IAssetImporterPool

		public async Task<IPoolElement<AAssetImporter>> PopImporter<TImporter>()
			where TImporter : AAssetImporter
		{
			INonAllocDecoratedPool<AAssetImporter> importerPool;

			await importerPoolSemaphore.WaitAsync();

			try
			{
				if (!importerPoolRepository.Has(typeof(TImporter)))
				{
					importerPool = PoolsFactory.BuildSimpleResizableObjectPool<AAssetImporter, TImporter>(
						initialAllocation,
						additionalAllocation,
						loggerResolver,
						new object[]
						{
							loggerResolver,
							(loggerResolver?.GetLogger<TImporter>() ?? null)
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
			}
			finally
			{
				importerPoolSemaphore.Release();
			}

			return importerPool.Pop(null);
		}

		public async Task PushImporter(
			IPoolElement<AAssetImporter> pooledImporter)
		{
			pooledImporter.Value.Cleanup();

			await importerPoolSemaphore.WaitAsync();

			try
			{
				pooledImporter.Push();
			}
			finally
			{
				importerPoolSemaphore.Release();
			}
		}

		#endregion

		#endregion
	}
}
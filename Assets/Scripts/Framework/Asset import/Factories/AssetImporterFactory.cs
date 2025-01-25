/*
using System;
using System.Collections.Generic;
using System.Threading;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport.Factories
{
	public static class AssetImporterFactory
	{
		public static AssetImportManager BuildAssetImportManager(
			ILoggerResolver loggerResolver)
		{
			var logger =
				loggerResolver?.GetLogger<AssetImportManager>();

			return new AssetImportManager(
				(IRepository<Type, List<AAssetImportPostProcessor>>)
					RepositoryFactory.BuildDictionaryRepository<Type, List<AAssetImportPostProcessor>>(),
				(IRepository<Type, IManagedPool<AAssetImporter>>)
					RepositoryFactory.BuildDictionaryRepository<Type, IManagedPool<AAssetImporter>>(),
				loggerResolver,
				logger);
		}

		public static ConcurrentAssetImportManager BuildConcurrentAssetImportManager(
			ILoggerResolver loggerResolver)
		{
			var logger =
				loggerResolver?.GetLogger<AssetImportManager>();

			return new ConcurrentAssetImportManager(
				(IRepository<Type, List<AAssetImportPostProcessor>>)
					RepositoryFactory.BuildDictionaryRepository<Type, List<AAssetImportPostProcessor>>(),
				(IRepository<Type, IManagedPool<AAssetImporter>>)
					RepositoryFactory.BuildDictionaryRepository<Type, IManagedPool<AAssetImporter>>(),
				new SemaphoreSlim(1, 1),
				new SemaphoreSlim(1, 1),
				loggerResolver,
				logger);
		}
	}
}
*/
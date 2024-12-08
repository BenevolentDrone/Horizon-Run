using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq; //error CS1061: 'IEnumerable<IReadOnlyResourceData>' does not contain a definition for 'ToArray'

using HereticalSolutions.Repositories;

using HereticalSolutions.Delegates.Notifiers;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentRuntimeResourceManager
		: IRuntimeResourceManager,
		  IAsyncContainsRootResources,
		  IContainsDependencyResources,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IRepository<int, string> rootResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> rootResourceRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> rootResourceAddedNotifier;

		private readonly SemaphoreSlim semaphore;

		private readonly ILogger logger;

		public ConcurrentRuntimeResourceManager(
			IRepository<int, string> rootResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> rootResourceRepository,
			IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> rootResourceAddedNotifier,
			SemaphoreSlim semaphore,
			ILogger logger = null)
		{
			this.rootResourceIDHashToID = rootResourceIDHashToID;

			this.rootResourceRepository = rootResourceRepository;

			this.rootResourceAddedNotifier = rootResourceAddedNotifier;

			this.semaphore = semaphore;

			this.logger = logger;
		}

		#region IRuntimeResourceManager

		#region IReadOnlyRuntimeResourceManager

		#region Has

		public bool HasRootResource(int rootResourceIDHash)
		{
			semaphore.Wait(); // Acquire the semaphore
			
			try
			{
				return rootResourceRepository.Has(rootResourceIDHash);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool HasRootResource(string rootResourceID)
		{
			return HasRootResource(rootResourceID.AddressToHash());
		}

		public bool HasResource(int[] resourcePathPartHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes);
		}

		public bool HasResource(string[] resourcePathParts)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathParts);
		}

		#endregion

		#region Get

		public IReadOnlyResourceData GetRootResource(int rootResourceIDHash)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					rootResourceIDHash,
					out resource))
				{
					return null;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return resource;
		}

		public IReadOnlyResourceData GetRootResource(string rootResourceID)
		{
			return GetRootResource(rootResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetResource(int[] resourcePathPartHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes))
				return null;

			return resource;
		}

		public IReadOnlyResourceData GetResource(string[] resourcePathParts)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathParts))
				return null;

			return resource;
		}

		#endregion

		#region Try get

		public bool TryGetRootResource(
			int rootResourceIDHash,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				return rootResourceRepository.TryGet(
					rootResourceIDHash,
					out resource);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}
		}

		public bool TryGetRootResource(
			string rootResourceID,
			out IReadOnlyResourceData resource)
		{
			return TryGetRootResource(
				rootResourceID.AddressToHash(),
				out resource);
		}

		public bool TryGetResource(
			int[] resourcePathPartHashes,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes);
		}

		public bool TryGetResource(
			string[] resourcePathParts,
			out IReadOnlyResourceData resource)
		{
			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return false;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return GetNestedResourceRecursive(
				ref resource,
				resourcePathParts);
		}

		#endregion

		#region Get default

		public IResourceVariantData GetDefaultRootResource(int rootResourceIDHash)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					rootResourceIDHash,
					out resource))
				{
					return null;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			return resource.DefaultVariant;
		}

		public IResourceVariantData GetDefaultRootResource(string rootResourceID)
		{
			return GetDefaultRootResource(rootResourceID.AddressToHash());
		}

		public IResourceVariantData GetDefaultResource(int[] resourcePathPartHashes)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathPartHashes[0],
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathPartHashes))
				return null;

			return resource.DefaultVariant;
		}

		public IResourceVariantData GetDefaultResource(string[] resourcePathParts)
		{
			IReadOnlyResourceData resource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out resource))
					return null;
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref resource,
				resourcePathParts))
				return null;

			return resource.DefaultVariant;
		}

		#endregion

		#region Try get default

		public bool TryGetDefaultRootResource(
			int rootResourceIDHash,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					rootResourceIDHash,
					out rootResource))
				{
					resource = null;

					return false;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		public bool TryGetDefaultRootResource(
			string rootResourceID,
			out IResourceVariantData resource)
		{
			return TryGetDefaultRootResource(
				rootResourceID.AddressToHash(),
				out resource);
		}

		public bool TryGetDefaultResource(
			int[] resourcePathPartHashes,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathPartHashes[0],
					out rootResource))
				{
					resource = null;

					return false;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref rootResource,
				resourcePathPartHashes))
			{
				resource = null;

				return false;
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		public bool TryGetDefaultResource(
			string[] resourcePathParts,
			out IResourceVariantData resource)
		{
			IReadOnlyResourceData rootResource;

			semaphore.Wait(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					resourcePathParts[0].AddressToHash(),
					out rootResource))
				{
					resource = null;

					return false;
				}
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (!GetNestedResourceRecursive(
				ref rootResource,
				resourcePathParts))
			{
				resource = null;

				return false;
			}

			resource = rootResource.DefaultVariant;

			return true;
		}

		#endregion

		#region All's

		public IEnumerable<int> RootResourceIDHashes
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return rootResourceRepository.Keys;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public IEnumerable<string> RootResourceIDs
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return rootResourceIDHashToID.Values;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllRootResources
		{
			get
			{
				semaphore.Wait(); // Acquire the semaphore

				try
				{
					return rootResourceRepository.Values;
				}
				finally
				{
					semaphore.Release(); // Release the semaphore
				}
			}
		}

		#endregion

		#endregion

		public async Task AddRootResource(
			IReadOnlyResourceData rootResource,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryAdd(
					rootResource.Descriptor.IDHash,
					rootResource))
				{
					progress?.Report(1f);

					return;
				}

				((IResourceData)rootResource).ParentResource = null;

				rootResourceIDHashToID.AddOrUpdate(
					rootResource.Descriptor.IDHash,
					rootResource.Descriptor.ID);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			progress?.Report(1f);
		}

		public async Task RemoveRootResource(
			int rootResourceIDHash = -1,
			bool free = true,
			IProgress<float> progress = null)
		{
			IReadOnlyResourceData resource;

			progress?.Report(0f);

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				if (!rootResourceRepository.TryGet(
					rootResourceIDHash,
					out resource))
				{
					progress?.Report(1f);

					return;
				}

				rootResourceRepository.TryRemove(rootResourceIDHash);

				rootResourceIDHashToID.TryRemove(rootResourceIDHash);
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (free)
			{
				progress?.Report(0.5f);

				IProgress<float> localProgress = progress.CreateLocalProgressWithRange(
					0.5f,
					1f);

				var task = ((IResourceData)resource)
					.Clear(
						free,
						localProgress);

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			progress?.Report(1f);
		}

		public async Task RemoveRootResource(
			string rootResourceID,
			bool free = true,
			IProgress<float> progress = null)
		{
			var task = RemoveRootResource(
				rootResourceID.AddressToHash(),
				free,
				progress);

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);
		}

		public async Task ClearAllRootResources(
			bool free = true,
			IProgress<float> progress = null)
		{
			progress?.Report(0f);

			IReadOnlyResourceData[] rootResourcesToFree;

			await semaphore.WaitAsync(); // Acquire the semaphore

			try
			{
				rootResourcesToFree = rootResourceRepository.Values.ToArray();

				rootResourceIDHashToID.Clear();

				rootResourceRepository.Clear();
			}
			finally
			{
				semaphore.Release(); // Release the semaphore
			}

			if (free)
			{
				int rootResourcesToFreeCount = rootResourcesToFree.Length;

				int totalStepsCount = rootResourcesToFreeCount + 1; //Clearing the repos counts as a step

				progress?.Report(1f / (float)totalStepsCount);

				for (int i = 0; i < rootResourcesToFreeCount; i++)
				{
					IResourceData rootResource = (IResourceData)rootResourcesToFree[i];

					IProgress<float> localProgress = progress.CreateLocalProgressForStep(
						(1f / (float)totalStepsCount),
						1f,
						i,
						rootResourcesToFreeCount);

					var task = rootResource
						.Clear(
							free,
							localProgress);

					await task;
						//.ConfigureAwait(false);

					await task
						.ThrowExceptionsIfAny(
							GetType(),
							logger);

					progress?.Report((float)(i + 2) / (float)totalStepsCount); // +1 for clearing the repo, +1 because the step is actually finished
				}
			}

			progress?.Report(1f);
		}

		#endregion

		#region IAsyncContainsRootResources

		#region Get

		public async Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(int rootResourceIDHash)
		{
			Task<IReadOnlyResourceData> waitForNotificationTask;

			semaphore.Wait();

			logger?.Log(
				GetType(),
				$"GetRootResourceWhenAvailable SEMAPHORE ACQUIRED");

			try
			{
				if (rootResourceRepository.TryGet(
					rootResourceIDHash,
					out var result))
				{
					return result;
				}

				var getWaitForNotificationTask = rootResourceAddedNotifier
					.GetWaitForNotificationTask(rootResourceIDHash);

				waitForNotificationTask = await getWaitForNotificationTask;
					//.ConfigureAwait(false);
					
				await waitForNotificationTask
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}
			finally
			{
				semaphore.Release();

				logger?.Log(
					GetType(),
					$"GetRootResourceWhenAvailable SEMAPHORE RELEASED");
			}

			/*
			return await rootResourceAddedNotifier
				.GetValueWhenNotified(resourceIDHash);
			*/

			logger?.Log(
				GetType(),
				$"GetRootResourceWhenAvailable AWAITING INITIATED");

			var awaitedResult = await waitForNotificationTask;
				//.ConfigureAwait(false);

			await waitForNotificationTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			logger?.Log(
				GetType(),
				$"GetRootResourceWhenAvailable AWAITING FINISHED");

			return awaitedResult;
		}

		public async Task<IReadOnlyResourceData> GetRootResourceWhenAvailable(
			string rootResourceID)
		{
			var task = GetRootResourceWhenAvailable(
				rootResourceID.AddressToHash());

			var result = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}

		public async Task<IReadOnlyResourceData> GetResourceWhenAvailable(int[] resourcePathPartHashes)
		{
			var getRootResourceTask = GetRootResourceWhenAvailable(
				resourcePathPartHashes[0]);

			var rootResource = await getRootResourceTask;
				//.ConfigureAwait(false);

			await getRootResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			var getNestedResourceTask = GetNestedResourceWhenAvailableRecursive(
				rootResource,
				resourcePathPartHashes);

			var result = await getNestedResourceTask;
				//.ConfigureAwait(false);

			await getNestedResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}

		public async Task<IReadOnlyResourceData> GetResourceWhenAvailable(string[] resourcePathParts)
		{
			var getRootResourceTask = GetRootResourceWhenAvailable(
				resourcePathParts[0]);

			var rootResource = await getRootResourceTask;
				//.ConfigureAwait(false);

			await getRootResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			var getNestedResourceTask = GetNestedResourceWhenAvailableRecursive(
				rootResource,
				resourcePathParts);

			var result = await getNestedResourceTask;
				//.ConfigureAwait(false);

			await getNestedResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}

		#endregion

		#region Get default

		public async Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(int rootResourceIDHash)
		{
			var task = GetRootResourceWhenAvailable(rootResourceIDHash);

			var rootResource = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return rootResource.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultRootResourceWhenAvailable(string rootResourceID)
		{
			var task = GetRootResourceWhenAvailable(rootResourceID);

			var rootResource = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return rootResource.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultResourceWhenAvailable(
			int[] resourcePathPartHashes)
		{
			var getRootResourceTask = GetRootResourceWhenAvailable(resourcePathPartHashes[0]);

			var rootResource = await getRootResourceTask;
				//.ConfigureAwait(false);

			await getRootResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			var getNestedResourceTask = GetNestedResourceWhenAvailableRecursive(
				rootResource,
				resourcePathPartHashes);

			var result = await getNestedResourceTask;
				//.ConfigureAwait(false);

			await getNestedResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result.DefaultVariant;
		}

		public async Task<IResourceVariantData> GetDefaultResourceWhenAvailable(
			string[] resourcePathParts)
		{
			var getRootResourceTask = GetRootResourceWhenAvailable(resourcePathParts[0]);

			var rootResource = await getRootResourceTask;
				//.ConfigureAwait(false);

			await getRootResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			var getNestedResourceTask = GetNestedResourceWhenAvailableRecursive(
				rootResource,
				resourcePathParts);

			var result = await getNestedResourceTask;
				//.ConfigureAwait(false);

			await getNestedResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result.DefaultVariant;
		}

		#endregion

		#endregion

		#region IContainsDependencyResources

		public async Task<IReadOnlyResourceStorageHandle> LoadDependency(
			string path,
			string variantID = null,
			IProgress<float> progress = null)
		{
			var getDependencyResourceTask = GetDependencyResource(path);

			IReadOnlyResourceData dependencyResource = await getDependencyResourceTask;
				//.ConfigureAwait(false);

			await getDependencyResourceTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			var getDependencyVariantTask = ((IContainsDependencyResourceVariants)dependencyResource)
				.GetDependencyResourceVariant(variantID);

			IResourceVariantData dependencyVariantData = await getDependencyVariantTask;
				//.ConfigureAwait(false);

			await getDependencyVariantTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);


			progress?.Report(0.5f);

			var dependencyStorageHandle = dependencyVariantData.StorageHandle;

			if (!dependencyStorageHandle.Allocated)
			{
				IProgress<float> localProgress = progress.CreateLocalProgressWithRange(
					0.5f,
					1f);

				var allocateTask = dependencyStorageHandle
					.Allocate(
						localProgress);

				await allocateTask;
					//.ConfigureAwait(false);

				await allocateTask	
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			progress?.Report(1f);

			return dependencyStorageHandle;
		}

		public async Task<IReadOnlyResourceData> GetDependencyResource(
			string path)
		{
			var task = GetResourceWhenAvailable(
				path.SplitAddressBySeparator());

			var result = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (rootResourceIDHashToID is ICleanuppable)
				(rootResourceIDHashToID as ICleanuppable).Cleanup();

			if (rootResourceRepository is ICleanuppable)
				(rootResourceRepository as ICleanuppable).Cleanup();

			if (rootResourceAddedNotifier is ICleanuppable)
				(rootResourceAddedNotifier as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (rootResourceIDHashToID is IDisposable)
				(rootResourceIDHashToID as IDisposable).Dispose();

			if (rootResourceRepository is IDisposable)
				(rootResourceRepository as IDisposable).Dispose();

			if (rootResourceAddedNotifier is IDisposable)
				(rootResourceAddedNotifier as IDisposable).Dispose();
		}

		#endregion

		private bool GetNestedResourceRecursive(
			ref IReadOnlyResourceData currentData,
			int[] resourcePathPartHashes)
		{
			for (int i = 1; i < resourcePathPartHashes.Length; i++)
			{
				if (!currentData.TryGetNestedResource(
					resourcePathPartHashes[i],
					out var newCurrentData))
					return false;

				currentData = newCurrentData;
			}

			return true;
		}

		private bool GetNestedResourceRecursive(
			ref IReadOnlyResourceData currentData,
			string[] resourcePathParts)
		{
			for (int i = 1; i < resourcePathParts.Length; i++)
			{
				if (!currentData.TryGetNestedResource(
					resourcePathParts[i],
					out var newCurrentData))
					return false;

				currentData = newCurrentData;
			}

			return true;
		}

		private async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailableRecursive(
			IReadOnlyResourceData currentData,
			int[] resourcePathPartHashes)
		{
			for (int i = 1; i < resourcePathPartHashes.Length; i++)
			{
				IAsyncContainsNestedResources concurrentCurrentData = currentData as IAsyncContainsNestedResources;

				if (concurrentCurrentData == null)
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"RESOURCE DATA {currentData.Descriptor.ID} IS NOT CONCURRENT"));

				var task = concurrentCurrentData
					.GetNestedResourceWhenAvailable(
						resourcePathPartHashes[i]);

				currentData = await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			return currentData;
		}

		private async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailableRecursive(
			IReadOnlyResourceData currentData,
			string[] resourcePathParts)
		{
			for (int i = 1; i < resourcePathParts.Length; i++)
			{
				IAsyncContainsNestedResources concurrentCurrentData = currentData as IAsyncContainsNestedResources;

				if (concurrentCurrentData == null)
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							$"RESOURCE DATA {currentData.Descriptor.ID} IS NOT CONCURRENT"));

				var task = concurrentCurrentData
					.GetNestedResourceWhenAvailable(
						resourcePathParts[i]);

				currentData = await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			return currentData;
		}
	}
}
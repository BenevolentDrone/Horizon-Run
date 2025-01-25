/*
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq; //error CS1061: 'IEnumerable<IReadOnlyResourceData>' does not contain a definition for 'ToArray'

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Delegates.Notifiers;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	public class ConcurrentResourceData
		: IResourceData,
		  IAsyncContainsResourceVariants,
		  IAsyncContainsNestedResources,
		  IContainsDependencyResourceVariants,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IRepository<int, string> variantIDHashToID;

		private readonly IRepository<int, IResourceVariantData> variantRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IResourceVariantData> variantAddedNotifier;

		private IResourceVariantData defaultVariant;


		private IReadOnlyResourceData parentResource;

		private readonly IRepository<int, string> nestedResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> nestedResourceRepository;

		private readonly IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> nestedResourceAddedNotifier;


		private readonly SemaphoreSlim semaphore;

		private readonly ILogger logger;

		public ConcurrentResourceData(
			ResourceDescriptor descriptor,

			IRepository<int, string> variantIDHashToID,
			IRepository<int, IResourceVariantData> variantRepository,
			IAsyncNotifierSingleArgGeneric<int, IResourceVariantData> variantAddedNotifier,

			IRepository<int, string> nestedResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> nestedResourceRepository,
			IAsyncNotifierSingleArgGeneric<int, IReadOnlyResourceData> nestedResourceAddedNotifier,

			SemaphoreSlim semaphore,

			ILogger logger)
		{
			Descriptor = descriptor;


			this.variantIDHashToID = variantIDHashToID;

			this.variantRepository = variantRepository;

			this.variantAddedNotifier = variantAddedNotifier;


			this.nestedResourceIDHashToID = nestedResourceIDHashToID;

			this.nestedResourceRepository = nestedResourceRepository;

			this.nestedResourceAddedNotifier = nestedResourceAddedNotifier;


			this.semaphore = semaphore;

			this.logger = logger;


			defaultVariant = null;

			parentResource = null;
		}

		#region IResourceData

		#region IReadOnlyResourceData

		public ResourceDescriptor Descriptor { get; private set; }

		#region IContainsResourceVariants

		public IResourceVariantData DefaultVariant
		{
			get
			{
				semaphore.Wait();

				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) DefaultVariant get SEMAPHORE ACQUIRED");

				try
				{
					return defaultVariant;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) DefaultVariant get SEMAPHORE RELEASED");
				}
			}
		}

		public bool HasVariant(int variantIDHash)
		{
			semaphore.Wait();
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) HasVariant SEMAPHORE ACQUIRED");

			try
			{
				return variantRepository.Has(variantIDHash);
			}
			finally
			{
				semaphore.Release();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) HasVariant SEMAPHORE RELEASED");
			}
		}

		public bool HasVariant(string variantID)
		{
			return HasVariant(variantID.AddressToHash());
		}

		public IResourceVariantData GetVariant(int variantIDHash)
		{
			semaphore.Wait();
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetVariant SEMAPHORE ACQUIRED");

			try
			{
				if (!variantRepository.TryGet(
					variantIDHash,
					out var variant))
					return null;

				return variant;
			}
			finally
			{
				semaphore.Release();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) GetVariant SEMAPHORE RELEASED");
			}
		}

		public IResourceVariantData GetVariant(string variantID)
		{
			return GetVariant(variantID.AddressToHash());
		}

		public bool TryGetVariant(
			int variantIDHash,
			out IResourceVariantData variant)
		{
			semaphore.Wait();
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) TryGetVariant SEMAPHORE ACQUIRED");

			try
			{
				return variantRepository.TryGet(variantIDHash, out variant);
			}
			finally
			{
				semaphore.Release();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) TryGetVariant SEMAPHORE RELEASED");
			}
		}

		public bool TryGetVariant(
			string variantID,
			out IResourceVariantData variant)
		{
			return TryGetVariant(variantID.AddressToHash(), out variant);
		}

		public IEnumerable<int> VariantIDHashes
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) VariantIDHashes get SEMAPHORE ACQUIRED");

				try
				{
					return variantRepository.Keys;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) VariantIDHashes get SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<string> VariantIDs
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) VariantIDs get SEMAPHORE ACQUIRED");

				try
				{
					return variantIDHashToID.Values;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) VariantIDs get SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<IResourceVariantData> AllVariants
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) AllVariants get SEMAPHORE ACQUIRED");

				try
				{
					return variantRepository.Values;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) AllVariants get SEMAPHORE RELEASED");
				}
			}
		}

		#endregion

		#region IContainsNestedResources

		public IReadOnlyResourceData ParentResource
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) ParentResource get SEMAPHORE ACQUIRED");

				try
				{
					return parentResource;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) ParentResource get SEMAPHORE RELEASED");
				}
			}
			set
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) ParentResource set SEMAPHORE ACQUIRED");

				try
				{
					parentResource = value;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) ParentResource set SEMAPHORE RELEASED");
				}
			}
		}

		public bool IsRoot
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) IsRoot get SEMAPHORE ACQUIRED");

				try
				{
					return parentResource == null;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) IsRoot get SEMAPHORE RELEASED");
				}
			}
		}

		public bool HasNestedResource(int nestedResourceIDHash)
		{
			semaphore.Wait();
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) HasNestedResource SEMAPHORE ACQUIRED");

			try
			{
				return nestedResourceRepository.Has(nestedResourceIDHash);
			}
			finally
			{
				semaphore.Release();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) HasNestedResource SEMAPHORE RELEASED");
			}
		}

		public bool HasNestedResource(string nestedResourceID)
		{
			return HasNestedResource(nestedResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
		{
			semaphore.Wait();
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetNestedResource SEMAPHORE ACQUIRED");

			try
			{
				if (!nestedResourceRepository.TryGet(
					nestedResourceIDHash,
					out var nestedResource))
					return null;

				return nestedResource;
			}
			finally
			{
				semaphore.Release();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) GetNestedResource SEMAPHORE RELEASED");
			}
		}

		public IReadOnlyResourceData GetNestedResource(string nestedResourceID)
		{
			return GetNestedResource(nestedResourceID.AddressToHash());
		}

		public bool TryGetNestedResource(
			int nestedResourceIDHash,
			out IReadOnlyResourceData nestedResource)
		{
			semaphore.Wait();
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) TryGetNestedResource SEMAPHORE ACQUIRED");

			try
			{
				return nestedResourceRepository.TryGet(
					nestedResourceIDHash,
					out nestedResource);
			}
			finally
			{
				semaphore.Release();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) TryGetNestedResource SEMAPHORE RELEASED");
			}
		}

		public bool TryGetNestedResource(
			string nestedResourceID,
			out IReadOnlyResourceData nestedResource)
		{
			return TryGetNestedResource(
				nestedResourceID.AddressToHash(),
				out nestedResource);
		}

		public IEnumerable<int> NestedResourceIDHashes
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) NestedResourceIDHashes get SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourceRepository.Keys;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) NestedResourceIDHashes get SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<string> NestedResourceIDs
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) NestedResourceIDs get SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourceIDHashToID.Values;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) NestedResourceIDs get SEMAPHORE RELEASED");
				}
			}
		}

		public IEnumerable<IReadOnlyResourceData> AllNestedResources
		{
			get
			{
				semaphore.Wait();
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) AllNestedResources get SEMAPHORE ACQUIRED");

				try
				{
					return nestedResourceRepository.Values;
				}
				finally
				{
					semaphore.Release();
					
					logger?.Log(
						GetType(),
						$"({Descriptor.ID}) AllNestedResources get SEMAPHORE RELEASED");
				}
			}
		}

		#endregion


		#endregion

		public async Task AddVariant(
			IResourceVariantData variant,
			bool allocate = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			logger?.Log(
				GetType(),
				$"{Descriptor.ID} ADDING VARIANT {variant.Descriptor.VariantID}");

			asyncContext?.Progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) AddVariant SEMAPHORE ACQUIRED ASYNC");

			try
			{
				if (!variantRepository.TryAdd(
					variant.Descriptor.VariantIDHash,
					variant))
				{
					asyncContext?.Progress?.Report(1f);

					return;
				}

				variantIDHashToID.AddOrUpdate(
					variant.Descriptor.VariantIDHash,
					variant.Descriptor.VariantID);

				UpdateDefaultVariant();

				var task = variantAddedNotifier
					.Notify(
						variant.Descriptor.VariantIDHash,
						variant,
						asyncContext);

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}
			finally
			{
				semaphore.Release(); 
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) AddVariant SEMAPHORE RELEASED");
			}

			if (allocate)
			{
				asyncContext?.Progress?.Report(0.5f);

				var task = variant
					.StorageHandle
					.Allocate(
						asyncContext.CreateLocalProgressWithRange(
							0.5f,
							1f));

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}
			
			asyncContext?.Progress?.Report(1f);
		}

		public async Task RemoveVariant(
			int variantIDHash = -1,
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IResourceVariantData variant = null;

			asyncContext?.Progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) RemoveVariant SEMAPHORE ACQUIRED ASYNC");

			try
			{
				if (!variantRepository.TryGet(
					variantIDHash,
					out variant))
				{
					asyncContext?.Progress?.Report(1f);

					return;
				}

				variantIDHashToID.TryRemove(variantIDHash);

				variantRepository.TryRemove(variantIDHash);

				UpdateDefaultVariant();
			}
			finally
			{
				semaphore.Release(); 
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) RemoveVariant SEMAPHORE RELEASED");
			}

			if (free)
			{
				asyncContext?.Progress?.Report(0.5f);

				var task = variant
					.StorageHandle
					.Free(
						asyncContext.CreateLocalProgressWithRange(
							0.5f,
							1f));

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			asyncContext?.Progress?.Report(1f);
		}

		public async Task RemoveVariant(
			string variantID,
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			logger?.Log(
				GetType(),
				$"{Descriptor.ID} REMOVING VARIANT {variantID}");

			var task = RemoveVariant(
				variantID.AddressToHash(),
				free,
				asyncContext);

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);
		}

		private void UpdateDefaultVariant()
		{
			defaultVariant = null;

			int topPriority = int.MinValue;

			foreach (var hashID in variantRepository.Keys)
			{
				var currentVariant = variantRepository.Get(hashID);

				var currentPriority = currentVariant.Descriptor.Priority;

				if (currentPriority > topPriority)
				{
					topPriority = currentPriority;

					defaultVariant = currentVariant;
				}
			}
		}

		public async Task ClearAllVariants(
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			IResourceVariantData[] variantsToFree;

			await semaphore.WaitAsync(); 
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) ClearAllVariants SEMAPHORE ACQUIRED ASYNC");

			try
			{
				variantsToFree = variantRepository.Values.ToArray();

				variantIDHashToID.Clear();

				variantRepository.Clear();

				defaultVariant = null;
			}
			finally
			{
				semaphore.Release(); 
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) ClearAllVariants SEMAPHORE RELEASED");
			}

			if (free)
			{
				int variantsToFreeCount = variantsToFree.Length;

				int totalStepsCount = variantsToFreeCount + 1; //Clearing the repos counts as a step

				asyncContext?.Progress?.Report(1f / (float)totalStepsCount);

				for (int i = 0; i < variantsToFree.Length; i++)
				{
					var task = variantsToFree[i]
						.StorageHandle
						.Free(
							asyncContext.CreateLocalProgressForStep(
								(1f / (float)totalStepsCount),
								1f,
								i,
								variantsToFreeCount));

					await task;
						//.ConfigureAwait(false);

					await task
						.ThrowExceptionsIfAny(
							GetType(),
							logger);

					asyncContext?.Progress?.Report((float)(i + 2) / (float)totalStepsCount); // +1 for clearing the repo, +1 because the step is actually finished
				}
			}

			asyncContext?.Progress?.Report(1f);
		}

		public async Task AddNestedResource(
			IReadOnlyResourceData nestedResource,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			logger?.Log(
				GetType(),
				$"{Descriptor.ID} ADDING NESTED RESOURCE {nestedResource.Descriptor.ID}");

			asyncContext?.Progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) AddNestedResource SEMAPHORE ACQUIRED ASYNC");

			try
			{
				if (!nestedResourceRepository.TryAdd(
					nestedResource.Descriptor.IDHash,
					nestedResource))
				{
					asyncContext?.Progress?.Report(1f);

					return;
				}

				((IResourceData)nestedResource).ParentResource = this; //Maybe this should be outside the critical section?

				nestedResourceIDHashToID.AddOrUpdate(
					nestedResource.Descriptor.IDHash,
					nestedResource.Descriptor.ID);

				var task = nestedResourceAddedNotifier
					.Notify(
						nestedResource.Descriptor.IDHash,
						nestedResource,
						
						asyncContext);

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}
			finally
			{
				semaphore.Release(); 
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) AddNestedResource SEMAPHORE RELEASED");
			}

			asyncContext?.Progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			int nestedResourceIDHash = -1,
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IReadOnlyResourceData nestedResource;

			asyncContext?.Progress?.Report(0f);

			await semaphore.WaitAsync(); 
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) RemoveNestedResource SEMAPHORE ACQUIRED ASYNC");

			try
			{
				if (!nestedResourceRepository.TryGet(
					nestedResourceIDHash,
					out nestedResource))
				{
					asyncContext?.Progress?.Report(1f);

					return;
				}

				((IResourceData)nestedResource).ParentResource = null;

				nestedResourceIDHashToID.TryRemove(nestedResourceIDHash);

				nestedResourceRepository.TryRemove(nestedResourceIDHash);
			}
			finally
			{
				semaphore.Release(); 
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) RemoveNestedResource SEMAPHORE RELEASED");
			}

			if (free)
			{
				asyncContext?.Progress?.Report(0.5f);

				var task = ((IResourceData)nestedResource)
					.Clear(
						free,
						asyncContext.CreateLocalProgressWithRange(
							0.5f,
							1f));

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}
			
			asyncContext?.Progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			string nestedResourceID,
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			logger?.Log(
				GetType(),
				$"{Descriptor.ID} REMOVING NESTED RESOURCE {nestedResourceID}");

			var task = RemoveNestedResource(
				nestedResourceID.AddressToHash(),
				free,
				asyncContext);

			await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);
		}

		public async Task ClearAllNestedResources(
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			IReadOnlyResourceData[] nestedResourcesToFree;

			await semaphore.WaitAsync(); 
			
			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) ClearAllNestedResources SEMAPHORE ACQUIRED ASYNC");

			try
			{
				nestedResourcesToFree = nestedResourceRepository.Values.ToArray();

				nestedResourceIDHashToID.Clear();

				nestedResourceRepository.Clear();
			}
			finally
			{
				semaphore.Release(); 
				
				logger?.Log(
					GetType(),
					$"({Descriptor.ID}) ClearAllNestedResources SEMAPHORE RELEASED");
			}

			if (free)
			{
				int nestedResourcesToFreeCount = nestedResourcesToFree.Length;

				int totalStepsCount = nestedResourcesToFreeCount + 1; //Clearing the repos counts as a step

				asyncContext?.Progress?.Report(1f / (float)totalStepsCount);

				for (int i = 0; i < nestedResourcesToFreeCount; i++)
				{
					IResourceData nestedResource = (IResourceData)nestedResourcesToFree[i];

					nestedResource.ParentResource = null;

					var task = nestedResource
						.Clear(
							free,
							asyncContext.CreateLocalProgressForStep(
								(1f / (float)totalStepsCount),
								1f,
								i,
								nestedResourcesToFreeCount));

					await task;
						//.ConfigureAwait(false);

					await task
						.ThrowExceptionsIfAny(
							GetType(),
							logger);

					asyncContext?.Progress?.Report((float)(i + 2) / (float)totalStepsCount); // +1 for clearing the repo, +1 because the step is actually finished
				}
			}

			asyncContext?.Progress?.Report(1f);
		}

		public async Task Clear(
			bool free = true,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			asyncContext?.Progress?.Report(0f);

			var clearVariantsTask = ClearAllVariants(
				free,
				asyncContext.CreateLocalProgressWithRange(
					0f,
					0.5f));

			await clearVariantsTask;
				//.ConfigureAwait(false);

			await clearVariantsTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			asyncContext?.Progress?.Report(0.5f);

			var clearNestedResourcesTask = ClearAllNestedResources(
				free,
				asyncContext.CreateLocalProgressWithRange(
					0.5f,
					1f));

			await clearNestedResourcesTask;
				//.ConfigureAwait(false);

			await clearNestedResourcesTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			asyncContext?.Progress?.Report(1f);
		}

		#endregion

		#region IAsyncContainsResourceVariants

		public async Task<IResourceVariantData> GetDefaultVariantWhenAvailable(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			Task<IResourceVariantData> waitForNotificationTask;

			semaphore.Wait();

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetDefaultVariantWhenAvailable SEMAPHORE ACQUIRED");

			try
			{
				if (defaultVariant != null)
				{
					return defaultVariant;
				}

				var getWaitForNotificationTask = variantAddedNotifier
					.GetWaitForNotificationTask(
						-1,
						true,
						
						asyncContext);

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
					$"({Descriptor.ID}) GetDefaultVariantWhenAvailable SEMAPHORE RELEASED");
			}

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetDefaultVariantWhenAvailable AWAITING INITIATED");

			var awaitedResult = await waitForNotificationTask;
				//.ConfigureAwait(false);

			await waitForNotificationTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetDefaultVariantWhenAvailable AWAITING FINISHED");

			return awaitedResult;
		}

		public async Task<IResourceVariantData> GetVariantWhenAvailable(
			int variantIDHash,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			Task<IResourceVariantData> waitForNotificationTask;

			semaphore.Wait();

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetVariantWhenAvailable SEMAPHORE ACQUIRED");

			try
			{
				if (variantRepository.TryGet(
					variantIDHash,
					out var result))
				{
					return result;
				}

				var getWaitForNotificationTask = variantAddedNotifier
					.GetWaitForNotificationTask(
						variantIDHash,
						
						asyncContext);

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
					$"({Descriptor.ID}) GetVariantWhenAvailable SEMAPHORE RELEASED");
			}

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetVariantWhenAvailable AWAITING INITIATED");

			var awaitedResult = await waitForNotificationTask;
				//.ConfigureAwait(false);

			await waitForNotificationTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetVariantWhenAvailable AWAITING FINISHED");

			return awaitedResult;
		}

		public async Task<IResourceVariantData> GetVariantWhenAvailable(
			string variantID,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var task = GetVariantWhenAvailable(
				variantID.AddressToHash(),
				
				asyncContext);

			var result = await task;
				//.ConfigureAwait(false);

			await task				
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}

		#endregion

		#region IAsyncContainsNestedResources

		public async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(
			int nestedResourceIDHash,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			Task<IReadOnlyResourceData> waitForNotificationTask;

			semaphore.Wait();

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetNestedResourceWhenAvailable SEMAPHORE ACQUIRED");

			try
			{
				if (nestedResourceRepository.TryGet(
					nestedResourceIDHash,
					out var result))
				{
					return result;
				}

				var getWaitForNotificationTask = nestedResourceAddedNotifier
					.GetWaitForNotificationTask(
						nestedResourceIDHash,

						asyncContext);

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
					$"({Descriptor.ID}) GetNestedResourceWhenAvailable SEMAPHORE RELEASED");
			}

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetNestedResourceWhenAvailable AWAITING INITIATED");

			var awaitedResult = await waitForNotificationTask;
				//.ConfigureAwait(false);

			await waitForNotificationTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			logger?.Log(
				GetType(),
				$"({Descriptor.ID}) GetNestedResourceWhenAvailable AWAITING FINISHED");

			return awaitedResult;
		}

		public async Task<IReadOnlyResourceData> GetNestedResourceWhenAvailable(
			string nestedResourceID,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var task = GetNestedResourceWhenAvailable(
				nestedResourceID.AddressToHash(),
				
				asyncContext);

			var result = await task;
				//.ConfigureAwait(false);

			await task
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			return result;
		}

		#endregion

		#region IContainsDependencyResourceVariants

		public async Task<IResourceVariantData> GetDependencyResourceVariant(
			string variantID = null,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			IResourceVariantData dependencyResourceVariant = null;

			if (string.IsNullOrEmpty(variantID))
			{
				var task1 = GetDefaultVariantWhenAvailable(

					asyncContext);

				dependencyResourceVariant = await task1;
					//.ConfigureAwait(false);

				await task1
				   .ThrowExceptionsIfAny(
						GetType(),
						logger);
			}
			else
			{
				var task2 = GetVariantWhenAvailable(
					variantID,
					
					asyncContext);

				dependencyResourceVariant = await task2;
					//.ConfigureAwait(false);

				await task2
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			return dependencyResourceVariant;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (variantIDHashToID is ICleanuppable)
				(variantIDHashToID as ICleanuppable).Cleanup();

			if (variantRepository is ICleanuppable)
				(variantRepository as ICleanuppable).Cleanup();

			if (nestedResourceIDHashToID is ICleanuppable)
				(nestedResourceIDHashToID as ICleanuppable).Cleanup();

			if (nestedResourceRepository is ICleanuppable)
				(nestedResourceRepository as ICleanuppable).Cleanup();

			if (nestedResourceAddedNotifier is ICleanuppable)
				(nestedResourceAddedNotifier as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (variantIDHashToID is IDisposable)
				(variantIDHashToID as IDisposable).Dispose();

			if (variantRepository is IDisposable)
				(variantRepository as IDisposable).Dispose();

			if (nestedResourceIDHashToID is IDisposable)
				(nestedResourceIDHashToID as IDisposable).Dispose();

			if (nestedResourceRepository is IDisposable)
				(nestedResourceRepository as IDisposable).Dispose();

			if (nestedResourceAddedNotifier is IDisposable)
				(nestedResourceAddedNotifier as IDisposable).Dispose();
		}

		#endregion
	}
}
*/
using System;
using System.Threading;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
	/// <summary>
	/// Represents resource data that can be read and modified
	/// </summary>
	public class ResourceData
		: IResourceData,
		  IContainsDependencyResourceVariants,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IRepository<int, string> variantIDHashToID;

		private readonly IRepository<int, IResourceVariantData> variantRepository;

		private IResourceVariantData defaultVariant;


		private readonly IRepository<int, string> nestedResourceIDHashToID;

		private readonly IRepository<int, IReadOnlyResourceData> nestedResourceRepository;


		private readonly ILogger logger;


		public ResourceData(
			ResourceDescriptor descriptor,
			IRepository<int, string> variantIDHashToID,
			IRepository<int, IResourceVariantData> variantRepository,
			IRepository<int, string> nestedResourceIDHashToID,
			IRepository<int, IReadOnlyResourceData> nestedResourceRepository,
			ILogger logger = null)
		{
			Descriptor = descriptor;
			
			this.variantIDHashToID = variantIDHashToID;

			this.variantRepository = variantRepository;

			this.nestedResourceIDHashToID = nestedResourceIDHashToID;

			this.nestedResourceRepository = nestedResourceRepository;

			this.logger = logger;


			defaultVariant = null;

			ParentResource = null;
		}

		#region IResourceData

		#region IReadOnlyResourceData

		/// <summary>
		/// Gets the descriptor of the resource
		/// </summary>
		public ResourceDescriptor Descriptor { get; private set; }

		#region IContainsResourceVariants

		/// <summary>
		/// Gets the default variant data of the resource
		/// </summary>
		public IResourceVariantData DefaultVariant => defaultVariant;

		public bool HasVariant(int variantIDHash)
		{
			return variantRepository.Has(variantIDHash);
		}

		public bool HasVariant(string variantID)
		{
			return HasVariant(variantID.AddressToHash());
		}

		/// <summary>
		/// Gets the variant data of the resource based on the variant ID hash
		/// </summary>
		/// <param name="variantIDHash">The hash of the variant ID.</param>
		/// <returns>The variant data associated with the specified variant ID hash.</returns>
		public IResourceVariantData GetVariant(int variantIDHash)
		{
			if (!variantRepository.TryGet(
				variantIDHash,
				out var variant))
				return null;

			return variant;
		}

		/// <summary>
		/// Gets the variant data of the resource based on the variant ID
		/// </summary>
		/// <param name="variantID">The ID of the variant.</param>
		/// <returns>The variant data associated with the specified variant ID.</returns>
		public IResourceVariantData GetVariant(string variantID)
		{
			return GetVariant(variantID.AddressToHash());
		}

		public bool TryGetVariant(
			int variantIDHash,
			out IResourceVariantData variant)
		{
			return variantRepository.TryGet(
				variantIDHash,
				out variant);
		}

		public bool TryGetVariant(
			string variantID,
			out IResourceVariantData variant)
		{
			return TryGetVariant(
				variantID.AddressToHash(),
				out variant);
		}

		/// <summary>
		/// Gets the variant hashes available for the resource
		/// </summary>
		public IEnumerable<int> VariantIDHashes => variantRepository.Keys;

		public IEnumerable<string> VariantIDs => variantIDHashToID.Values;

		public IEnumerable<IResourceVariantData> AllVariants => variantRepository.Values;

		#endregion

		#region IContainsNestedResources

		public IReadOnlyResourceData ParentResource { get; set; }

		public bool IsRoot { get => ParentResource == null; }

		public bool HasNestedResource(int nestedResourceIDHash)
		{
			return nestedResourceRepository.Has(nestedResourceIDHash);
		}

		public bool HasNestedResource(string nestedResourceID)
		{
			return HasNestedResource(nestedResourceID.AddressToHash());
		}

		public IReadOnlyResourceData GetNestedResource(int nestedResourceIDHash)
		{
			if (!nestedResourceRepository.TryGet(
				nestedResourceIDHash,
				out var nestedResource))
				return null;

			return nestedResource;
		}

		public IReadOnlyResourceData GetNestedResource(string nestedResourceID)
		{
			return GetNestedResource(nestedResourceID.AddressToHash());
		}

		public bool TryGetNestedResource(
			int nestedResourceIDHash,
			out IReadOnlyResourceData nestedResource)
		{
			return nestedResourceRepository.TryGet(
				nestedResourceIDHash,
				out nestedResource);
		}

		public bool TryGetNestedResource(
			string nestedResourceID,
			out IReadOnlyResourceData nestedResource)
		{
			return TryGetNestedResource(
				nestedResourceID.AddressToHash(),
				out nestedResource);
		}

		public IEnumerable<int> NestedResourceIDHashes => nestedResourceRepository.Keys;

		public IEnumerable<string> NestedResourceIDs => nestedResourceIDHashToID.Values;

		public IEnumerable<IReadOnlyResourceData> AllNestedResources => nestedResourceRepository.Values;

		#endregion


		#endregion

		/// <summary>
		/// Adds a variant to the resource
		/// </summary>
		/// <param name="variant">The variant data to add.</param>
		/// <param name="progress">An optional progress reporter for tracking the add operation.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public async Task AddVariant(
			IResourceVariantData variant,
			bool allocate = true,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			if (!variantRepository.TryAdd(
				variant.Descriptor.VariantIDHash,
				variant))
			{
				progress?.Report(1f);

				return;
			}

			variantIDHashToID.AddOrUpdate(
				variant.Descriptor.VariantIDHash,
				variant.Descriptor.VariantID);

			UpdateDefaultVariant();

			if (allocate)
			{
				var task = variant
					.StorageHandle
					.Allocate(
						progress: progress);

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			int variantIDHash = -1,
			bool free = true,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			if (!variantRepository.TryGet(
				variantIDHash,
				out var variant))
			{
				progress?.Report(1f);

				return;
			}

			variantIDHashToID.TryRemove(variantIDHash);

			variantRepository.TryRemove(variantIDHash);

			UpdateDefaultVariant();

			if (free)
			{
				var task = variant
					.StorageHandle
					.Free(
						progress: progress);

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			progress?.Report(1f);
		}

		public async Task RemoveVariant(
			string variantID,
			bool free = true,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			var task = RemoveVariant(
				variantID.AddressToHash(),
				free,
				progress: progress);

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
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			int totalVariantsCount = variantRepository.Count;

			int counter = 0;

			foreach (var key in variantRepository.Keys)
			{
				if (variantRepository.TryGet(
					key,
					out var variant))
				{
					if (free)
					{
						IProgress<float> localProgress = progress.CreateLocalProgressForStep(
							0f,
							1f,
							counter,
							totalVariantsCount);
					
						var task = variant
							.StorageHandle
							.Free(
								progress: localProgress);

						await task;
							//.ConfigureAwait(false);

						await task
							.ThrowExceptionsIfAny(
								GetType(),
								logger);
					}
				}

				counter++;

				progress?.Report((float)counter / (float)totalVariantsCount);
			}

			variantIDHashToID.Clear();

			variantRepository.Clear();

			defaultVariant = null;

			progress?.Report(1f);
		}

		public async Task AddNestedResource(
			IReadOnlyResourceData nestedResource,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			if (!nestedResourceRepository.TryAdd(
				nestedResource.Descriptor.IDHash,
				nestedResource))
			{
				progress?.Report(1f);

				return;
			}

			((IResourceData)nestedResource).ParentResource = this;

			nestedResourceIDHashToID.AddOrUpdate(
				nestedResource.Descriptor.IDHash,
				nestedResource.Descriptor.ID);

			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			int nestedResourceIDHash = -1,
			bool free = true,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			if (!nestedResourceRepository.TryGet(
				nestedResourceIDHash,
				out var nestedResource))
			{
				progress?.Report(1f);

				return;
			}

			((IResourceData)nestedResource).ParentResource = null;

			nestedResourceIDHashToID.TryRemove(nestedResourceIDHash);

			nestedResourceRepository.TryRemove(nestedResourceIDHash);

			if (free)
			{
				var task = ((IResourceData)nestedResource)
					.Clear(
						free,
						progress: progress);

				await task;
					//.ConfigureAwait(false);

				await task
					.ThrowExceptionsIfAny(
						GetType(),
						logger);
			}

			progress?.Report(1f);
		}

		public async Task RemoveNestedResource(
			string nestedResourceID,
			bool free = true,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			var task = RemoveNestedResource(
				nestedResourceID.AddressToHash(),
				free,
				progress: progress);

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
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			int totalNestedResourcesCount = nestedResourceRepository.Count;

			int counter = 0;

			foreach (var key in nestedResourceRepository.Keys)
			{
				if (!nestedResourceRepository.TryGet(
					key,
					out var nestedResource))
				{
					((IResourceData)nestedResource).ParentResource = null;

					IProgress<float> localProgress = progress.CreateLocalProgressForStep(
						0f,
						1f,
						counter,
						totalNestedResourcesCount);

					var task = ((IResourceData)nestedResource)
						.Clear(
							free,
							progress: localProgress);

					await task;
						//.ConfigureAwait(false);

					await task
						.ThrowExceptionsIfAny(
							GetType(),
							logger);
				}

				counter++;

				progress?.Report((float)counter / (float)totalNestedResourcesCount);
			}

			nestedResourceIDHashToID.Clear();

			nestedResourceRepository.Clear();

			progress?.Report(1f);
		}

		public async Task Clear(
			bool free = true,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			progress?.Report(0f);

			IProgress<float> localProgress = progress.CreateLocalProgressWithRange(
				0f,
				0.5f);

			var clearAllVariantsTask = ClearAllVariants(
				free,
				progress: localProgress);

			await clearAllVariantsTask;
				//.ConfigureAwait(false);

			await clearAllVariantsTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			progress?.Report(0.5f);

			localProgress = progress.CreateLocalProgressWithRange(
				0.5f,
				1f);

			var clearAllNestedResourcesTask = ClearAllNestedResources(
				free,
				progress: localProgress);

			await clearAllNestedResourcesTask;
				//.ConfigureAwait(false);

			await clearAllNestedResourcesTask
				.ThrowExceptionsIfAny(
					GetType(),
					logger);

			defaultVariant = null;

			ParentResource = null;

			progress?.Report(1f);
		}

		#endregion
	
		#region IContainsDependencyResourceVariants

		public async Task<IResourceVariantData> GetDependencyResourceVariant(
			string variantID = null,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null)
		{
			IResourceVariantData dependencyResourceVariant = null;

			if (string.IsNullOrEmpty(variantID))
			{
				dependencyResourceVariant = defaultVariant;
			}
			else
			{
				dependencyResourceVariant = GetVariant(
					variantID);
			}

			if (dependencyResourceVariant == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"VARIANT {(string.IsNullOrEmpty(variantID) ? "NULL" : variantID)} DOES NOT EXIST"));

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
		}

		#endregion
	}
}
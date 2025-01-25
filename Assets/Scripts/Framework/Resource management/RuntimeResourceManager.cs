/*
using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Repositories;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents a runtime resource manager
    /// </summary>
    public class RuntimeResourceManager
        : IRuntimeResourceManager,
          IContainsDependencyResources,
          ICleanuppable,
          IDisposable
    {
        private readonly IRepository<int, string> rootResourceIDHashToID;

        private readonly IRepository<int, IReadOnlyResourceData> rootResourceRepository;

        private readonly ILogger logger;

        public RuntimeResourceManager(
            IRepository<int, string> rootResourceIDHashToID,
            IRepository<int, IReadOnlyResourceData> rootResourceRepository,
            ILogger logger)
        {
            this.rootResourceIDHashToID = rootResourceIDHashToID;

            this.rootResourceRepository = rootResourceRepository;

            this.logger = logger;
        }

        #region IRuntimeResourceManager

        #region IReadOnlyRuntimeResourceManager

        #region Has

        public bool HasRootResource(int rootResourceIDHash)
        {
            return rootResourceRepository.Has(rootResourceIDHash);
        }

        public bool HasRootResource(string rootResourceID)
        {
            return HasRootResource(rootResourceID.AddressToHash());
        }

        public bool HasResource(int[] resourcePathPartHashes)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentData))
                return false;

            return GetNestedResourceRecursive(
                ref currentData,
                resourcePathPartHashes);
        }

        public bool HasResource(string[] resourcePathParts)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentData))
                return false;

            return GetNestedResourceRecursive(
                ref currentData,
                resourcePathParts);
        }

        #endregion

        #region Get

        public IReadOnlyResourceData GetRootResource(int rootResourceIDHash)
        {
            if (!rootResourceRepository.TryGet(
                rootResourceIDHash,
                out var resource))
                return null;

            return resource;
        }

        public IReadOnlyResourceData GetRootResource(string rootResourceID)
        {
            return GetRootResource(rootResourceID.AddressToHash());
        }

        public IReadOnlyResourceData GetResource(int[] resourcePathPartHashes)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathPartHashes))
                return null;

            return currentResource;
        }

        public IReadOnlyResourceData GetResource(string[] resourcePathParts)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathParts))
                return null;

            return currentResource;
        }

        #endregion

        #region Try get

        public bool TryGetRootResource(
            int rootResourceIDHash,
            out IReadOnlyResourceData resource)
        {
            return rootResourceRepository.TryGet(
                rootResourceIDHash,
                out resource);
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
            if (!rootResourceRepository.TryGet(
                resourcePathPartHashes[0],
                out resource))
                return false;

            return GetNestedResourceRecursive(
                ref resource,
                resourcePathPartHashes);
        }

        public bool TryGetResource(
            string[] resourcePathParts,
            out IReadOnlyResourceData resource)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out resource))
                return false;

            return GetNestedResourceRecursive(
                ref resource,
                resourcePathParts);
        }

        #endregion

        #region Get default

        public IResourceVariantData GetDefaultRootResource(int rootResourceIDHash)
        {
            if (!rootResourceRepository.TryGet(
                rootResourceIDHash,
                out var resource))
                return null;

            return resource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultRootResource(string rootResourceID)
        {
            return GetDefaultRootResource(rootResourceID.AddressToHash());
        }

        public IResourceVariantData GetDefaultResource(int[] resourcePathPartHashes)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathPartHashes))
                return null;

            return currentResource.DefaultVariant;
        }

        public IResourceVariantData GetDefaultResource(string[] resourcePathParts)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentResource))
                return null;

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathParts))
                return null;

            return currentResource.DefaultVariant;
        }

        #endregion

        #region Try get default

        public bool TryGetDefaultRootResource(
            int rootResourceIDHash,
            out IResourceVariantData resource)
        {
            if (!rootResourceRepository.TryGet(
                rootResourceIDHash,
                out var rootResource))
            {
                resource = null;

                return false;
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
            if (!rootResourceRepository.TryGet(
                resourcePathPartHashes[0],
                out var currentResource))
            {
                resource = null;

                return false;
            }

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathPartHashes))
            {
                resource = null;

                return false;
            }

            resource = currentResource.DefaultVariant;

            return true;
        }

        public bool TryGetDefaultResource(
            string[] resourcePathParts,
            out IResourceVariantData resource)
        {
            if (!rootResourceRepository.TryGet(
                resourcePathParts[0].AddressToHash(),
                out var currentResource))
            {
                resource = null;

                return false;
            }

            if (!GetNestedResourceRecursive(
                ref currentResource,
                resourcePathParts))
            {
                resource = null;

                return false;
            }

            resource = currentResource.DefaultVariant;

            return true;
        }

        #endregion

        #region All's

        public IEnumerable<int> RootResourceIDHashes { get => rootResourceRepository.Keys; }

        public IEnumerable<string> RootResourceIDs { get => rootResourceIDHashToID.Values; }

        public IEnumerable<IReadOnlyResourceData> AllRootResources { get => rootResourceRepository.Values; }

        #endregion

        #endregion

        public async Task AddRootResource(
            IReadOnlyResourceData rootResource,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            asyncContext?.Progress?.Report(0f);

            if (!rootResourceRepository.TryAdd(
                rootResource.Descriptor.IDHash,
                rootResource))
            {
                asyncContext?.Progress?.Report(1f);

                return;
            }

            ((IResourceData)rootResource).ParentResource = null;

            rootResourceIDHashToID.AddOrUpdate(
                rootResource.Descriptor.IDHash,
                rootResource.Descriptor.ID);

            asyncContext?.Progress?.Report(1f);
        }

        public async Task RemoveRootResource(
            int rootResourceIDHash = -1,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            asyncContext?.Progress?.Report(0f);

            if (!rootResourceRepository.TryGet(
                rootResourceIDHash,
                out var resource))
            {
                asyncContext?.Progress?.Report(1f);

                return;
            }

            rootResourceRepository.TryRemove(rootResourceIDHash);

            rootResourceIDHashToID.TryRemove(rootResourceIDHash);

            if (free)
            {
                var task = ((IResourceData)resource)
                    .Clear(
                        free,

                        asyncContext);

                await task;
                    //.ConfigureAwait(false);

                await task
                    .ThrowExceptionsIfAny(
                        GetType(),
                        logger);
            }

            asyncContext?.Progress?.Report(1f);
        }

        public async Task RemoveRootResource(
            string rootResourceID,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var task = RemoveRootResource(
                rootResourceID.AddressToHash(),
                free,

                asyncContext);

            await task;
                //.ConfigureAwait(false);

            await task
                .ThrowExceptionsIfAny(
                    GetType(),
                    logger);
        }

        public async Task ClearAllRootResources(
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            asyncContext?.Progress?.Report(0f);

            int totalRootResourcesCount = rootResourceRepository.Count;

            int counter = 0;

            foreach (var key in rootResourceRepository.Keys)
            {
                if (rootResourceRepository.TryGet(
                    key,
                    out var rootResource))
                {
                    var task = ((IResourceData)rootResource)
                        .Clear(
                            free,

                            asyncContext.CreateLocalProgressForStep(
                                0f,
                                1f,
                                counter,
                                totalRootResourcesCount));

                    await task;
                        //.ConfigureAwait(false);

                    await task
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);
                }

                counter++;

                asyncContext?.Progress?.Report((float)counter / (float)totalRootResourcesCount);

            }

            rootResourceIDHashToID.Clear();

            rootResourceRepository.Clear();

            asyncContext?.Progress?.Report(1f);
        }

        #endregion

        #region IContainsDependencyResources

        public async Task<IReadOnlyResourceStorageHandle> LoadDependency(
            string path,
            string variantID = null,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var getDependencyResourceTask = GetDependencyResource(
                path,
                
                asyncContext);

            IReadOnlyResourceData dependencyResource = await getDependencyResourceTask;
                //.ConfigureAwait(false);

            await getDependencyResourceTask
                .ThrowExceptionsIfAny(
                    GetType(),
                    logger);

            var getDependencyVariantTask = ((IContainsDependencyResourceVariants)dependencyResource)
                .GetDependencyResourceVariant(
                    variantID,

                    asyncContext.CreateLocalProgressWithRange(
                        0f,
                        0.5f));


            IResourceVariantData dependencyVariantData = await getDependencyVariantTask;
                //.ConfigureAwait(false);

            await getDependencyVariantTask
                .ThrowExceptionsIfAny(
                    GetType(),
                    logger);

            asyncContext?.Progress?.Report(0.5f);

            var dependencyStorageHandle = dependencyVariantData.StorageHandle;

            if (!dependencyStorageHandle.Allocated)
            {
                var allocateTask = dependencyStorageHandle
                    .Allocate(
                        asyncContext.CreateLocalProgressWithRange(
                            0.5f,
                            1f));

                await allocateTask;
                    //.ConfigureAwait(false);

                await allocateTask
                    .ThrowExceptionsIfAny(
                        GetType(),
                        logger);
            }

            asyncContext?.Progress?.Report(1f);

            return dependencyStorageHandle;
        }

        public async Task<IReadOnlyResourceData> GetDependencyResource(
            string path,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            IReadOnlyResourceData dependencyResource = GetResource(
                path.SplitAddressBySeparator());

            if (dependencyResource == null)
                throw new Exception(
                    logger.TryFormatException(
                        GetType(),
                        $"RESOURCE {path} DOES NOT EXIST"));

            return dependencyResource;
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (rootResourceIDHashToID is ICleanuppable)
                (rootResourceIDHashToID as ICleanuppable).Cleanup();

            if (rootResourceRepository is ICleanuppable)
                (rootResourceRepository as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (rootResourceIDHashToID is IDisposable)
                (rootResourceIDHashToID as IDisposable).Dispose();

            if (rootResourceRepository is IDisposable)
                (rootResourceRepository as IDisposable).Dispose();
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
    }
}
*/
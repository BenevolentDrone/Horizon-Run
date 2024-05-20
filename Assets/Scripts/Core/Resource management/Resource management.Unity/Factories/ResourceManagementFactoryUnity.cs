using HereticalSolutions.Logging;

using UnityEngine.AddressableAssets;

namespace HereticalSolutions.ResourceManagement.Factories
{
    public static class ResourceManagementFactoryUnity
    {   
        public static AddressableResourceStorageHandle<TResource> BuildAddressableResourceStorageHandle<TResource>(
            AssetReference assetReference,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AddressableResourceStorageHandle<TResource>>()
                ?? null;

            return new AddressableResourceStorageHandle<TResource>(
                assetReference,
                runtimeResourceManager,
                logger);
        }
    }
}
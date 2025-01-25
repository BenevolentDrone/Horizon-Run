/*
using HereticalSolutions.Logging;

using UnityEngine.AddressableAssets;

namespace HereticalSolutions.ResourceManagement.Factories
{
    public static class ResourceManagementFactoryUnity
    {   
        public static AddressableResourceStorageHandle<TResource> BuildAddressableResourceStorageHandle<TResource>(
            AssetReference assetReference,
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver)
        {
            ILogger logger =
                loggerResolver?.GetLogger<AddressableResourceStorageHandle<TResource>>();

            return new AddressableResourceStorageHandle<TResource>(
                assetReference,
                runtimeResourceManager,
                logger);
        }
    }
}
*/
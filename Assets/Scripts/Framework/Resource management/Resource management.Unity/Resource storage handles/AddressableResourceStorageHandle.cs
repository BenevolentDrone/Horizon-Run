using System;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

using UnityEngine.AddressableAssets;

namespace HereticalSolutions.ResourceManagement
{
    public class AddressableResourceStorageHandle<TResource>
        : AReadOnlyResourceStorageHandle<TResource>
    {
        private AssetReference assetReference;

        public AddressableResourceStorageHandle(
            AssetReference assetReference,
            IRuntimeResourceManager runtimeResourceManager,
            ILogger logger = null)
            : base(
                runtimeResourceManager,
                logger)
        {
            this.assetReference = assetReference;
        }
        protected override async Task<TResource> AllocateResource(
            IProgress<float> progress = null)
        {
            return await assetReference.LoadAssetAsync<TResource>().Task; //TODO: change to while() loop and report progress from handle
        }

        protected override async Task FreeResource(
            TResource resource,
            IProgress<float> progress = null)
        {
            assetReference.ReleaseAsset();
        }
    }
}
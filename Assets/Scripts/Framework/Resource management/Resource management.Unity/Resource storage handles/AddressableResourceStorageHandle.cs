using System;
using System.Threading;
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

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null)
        {
            return await assetReference.LoadAssetAsync<TResource>().Task; //TODO: change to while() loop and report progress from handle
        }

        protected override async Task FreeResource(
            TResource resource,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null)
        {
            assetReference.ReleaseAsset();
        }
    }
}
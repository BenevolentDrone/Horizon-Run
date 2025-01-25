/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

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
            ILogger logger)
            : base(
                runtimeResourceManager,
                logger)
        {
            this.assetReference = assetReference;
        }
        protected override async Task<TResource> AllocateResource(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            return await assetReference.LoadAssetAsync<TResource>().Task; //TODO: change to while() loop and report progress from handle
        }

        protected override async Task FreeResource(
            TResource resource,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            assetReference.ReleaseAsset();
        }
    }
}
*/
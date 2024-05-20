using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
    public class ResourceImporterFromAddressables : AAssetImporter
    {
        private AddressableResourcesSettings settings;

        public ResourceImporterFromAddressables(
            IRuntimeResourceManager runtimeResourceManager,
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
            : base(
                loggerResolver,
                logger)
        {
        }

        public void Initialize(
            IRuntimeResourceManager runtimeResourceManager,
            AddressableResourcesSettings settings)
        {
            InitializeInternal(runtimeResourceManager);

            this.settings = settings;
        }

        public override async Task<IResourceVariantData> Import(
            IProgress<float> progress = null)
        {
            int resourcesLoaded = 0;

            int totalResources = 0;

            foreach (var resourceDataSettings in settings.Resources)
                totalResources += resourceDataSettings.Variants.Length;

            progress?.Report(0f);

            foreach (var resourceDataSettings in settings.Resources)
            {
                string resourceID = resourceDataSettings.ResourceID;

                foreach (var resourceVariantDataSettings in resourceDataSettings.Variants)
                {
                    string variantID = resourceVariantDataSettings.VariantID;

                    var resourceAssetReference = resourceVariantDataSettings.AssetReference;

                    if (!resourceAssetReference.RuntimeKeyIsValid())
                    {
                        // Log an error if the runtime key is invalid for the asset
                        logger?.LogError<ResourceImporterFromAddressables>(
                            $"RUNTIME KEY IS INVALID FOR ASSET {resourceDataSettings.ResourceID} VARIANT {resourceVariantDataSettings.VariantID}");

                        continue;
                    }

                    logger?.Log<ResourceImporterFromAddressables>(
                        $"IMPORTING {resourceID} INITIATED");

                    IProgress<float> localProgress = progress.CreateLocalProgress(
                        0f,
                        1f,
                        resourcesLoaded,
                        totalResources);

                    var localResult = await AddAssetAsResourceVariant(
                        await GetOrCreateResourceData(
                            resourceID)
                            .ThrowExceptions<IResourceData, ResourceImporterFromAddressables>(logger),
                        new ResourceVariantDescriptor()
                        {
                            VariantID = variantID,
                            VariantIDHash = variantID.AddressToHash(),
                            Priority = 0,
                            Source = EResourceSources.LOCAL_STORAGE,
                            Storage = EResourceStorages.RAM,
                            ResourceType = typeof(UnityEngine.Object) // TODO: Find a better way
                        },
                        ResourceManagementFactoryUnity.BuildAddressableResourceStorageHandle<UnityEngine.Object>(
                            resourceVariantDataSettings.AssetReference,
                            runtimeResourceManager,
                            loggerResolver),
                        true,
                        localProgress)
                        .ThrowExceptions<IResourceVariantData, ResourceImporterFromAddressables>(logger);

                    logger?.Log<ResourceImporterFromAddressables>(
                        $"IMPORTING {resourceID} FINISHED");

                    resourcesLoaded++;

                    progress?.Report((float)resourcesLoaded / (float)totalResources);

                    await Task.Yield();
                }
            }

            progress?.Report(1f);

            return null;
        }

        public override void Cleanup()
        {
            settings = null;
        }
    }
}
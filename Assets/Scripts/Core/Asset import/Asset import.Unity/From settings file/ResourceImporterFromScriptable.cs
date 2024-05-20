using System;
using System.Threading.Tasks;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
    public class ResourceImporterFromScriptable : AAssetImporter
    {
        private ResourcesSettings settings;

        public ResourceImporterFromScriptable(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
            : base(
                loggerResolver,
                logger)
        {
        }

        public void Initialize(
            IRuntimeResourceManager runtimeResourceManager,
            ResourcesSettings settings)
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

                    logger?.Log<ResourceImporterFromScriptable>(
                        $"IMPORTING {resourceID} INITIATED");

                    IProgress<float> localProgress = progress.CreateLocalProgress(
                        0f,
                        1f,
                        resourcesLoaded,
                        totalResources);

                    var localResult = await AddAssetAsResourceVariant(
                        await GetOrCreateResourceData(
                            resourceID)
                            .ThrowExceptions<IResourceData, ResourceImporterFromScriptable>(logger),
                        new ResourceVariantDescriptor()
                        {
                            VariantID = variantID,
                            VariantIDHash = variantID.AddressToHash(),
                            Priority = 0,
                            Source = EResourceSources.LOCAL_STORAGE,
                            Storage = EResourceStorages.RAM,
                            ResourceType = resourceVariantDataSettings.Resource.GetType(),
                        },
#if USE_THREAD_SAFE_RESOURCE_MANAGEMENT
                        ResourceManagementFactory.BuildConcurrentPreallocatedResourceStorageHandle<UnityEngine.Object>(
                            resourceVariantDataSettings.Resource,
                            runtimeResourceManager,
                            loggerResolver),
#else
                        ResourceManagementFactory.BuildPreallocatedResourceStorageHandle<UnityEngine.Object>(
                            resourceVariantDataSettings.Resource,
                            runtimeResourceManager,
                            loggerResolver),
#endif
                        true,
                        localProgress)
                        .ThrowExceptions<IResourceVariantData, ResourceImporterFromScriptable>(logger);

                    logger?.Log<ResourceImporterFromScriptable>(
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
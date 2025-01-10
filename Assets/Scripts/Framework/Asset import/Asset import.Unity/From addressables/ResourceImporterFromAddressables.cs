using System;
using System.Threading;
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

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null)
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
                        logger?.LogError(
                            GetType(),
                            $"RUNTIME KEY IS INVALID FOR ASSET {resourceDataSettings.ResourceID} VARIANT {resourceVariantDataSettings.VariantID}");

                        continue;
                    }

                    logger?.Log(
                        GetType(),
                        $"IMPORTING {resourceID} INITIATED");

                    IProgress<float> localProgress = progress.CreateLocalProgressForStep(
                        0f,
                        1f,
                        resourcesLoaded,
                        totalResources);


                    var getOrCreateResourceTask = GetOrCreateResourceData(
                        resourceID);

                    var resource = await getOrCreateResourceTask;
                        //.ConfigureAwait(false);

                    await getOrCreateResourceTask
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);


                    var addAsVariantTask = AddAssetAsResourceVariant(
                        resource,
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
                        localProgress);

                    await addAsVariantTask;
                        //.ConfigureAwait(false);

                    await addAsVariantTask
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);
                            

                    logger?.Log(
                        GetType(),
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
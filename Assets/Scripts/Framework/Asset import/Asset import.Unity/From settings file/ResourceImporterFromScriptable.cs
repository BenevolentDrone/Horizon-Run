using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.ResourceManagement;
using HereticalSolutions.ResourceManagement.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.AssetImport
{
    public class ResourceImporterFromScriptable : AAssetImporter
    {
        private ResourcesSettings settings;

        public ResourceImporterFromScriptable(
            ILoggerResolver loggerResolver,
            ILogger logger)
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

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            int resourcesLoaded = 0;

            int totalResources = 0;

            foreach (var resourceDataSettings in settings.Resources)
                totalResources += resourceDataSettings.Variants.Length;

            asyncContext?.Progress?.Report(0f);

            foreach (var resourceDataSettings in settings.Resources)
            {
                string resourceID = resourceDataSettings.ResourceID;

                foreach (var resourceVariantDataSettings in resourceDataSettings.Variants)
                {
                    string variantID = resourceVariantDataSettings.VariantID;

                    logger?.Log(
                        GetType(),
                        $"IMPORTING {resourceID} INITIATED");

                    var localAsyncContext = asyncContext.CreateLocalProgressForStep(
                        0f,
                        1f,
                        resourcesLoaded,
                        totalResources);

                    var getOrCreateResourceTask = GetOrCreateResourceData(
                        resourceID,

                        localAsyncContext);

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
                        localAsyncContext);

                    var localResult = await addAsVariantTask;
                        //.ConfigureAwait(false);

                    await addAsVariantTask
                        .ThrowExceptionsIfAny(
                            GetType(),
                            logger);

                    logger?.Log(
                        GetType(),
                        $"IMPORTING {resourceID} FINISHED");

                    resourcesLoaded++;

                    asyncContext?.Progress?.Report((float)resourcesLoaded / (float)totalResources);

                    await Task.Yield();
                }
            }

            asyncContext?.Progress?.Report(1f);

            return null;
        }

        public override void Cleanup()
        {
            settings = null;
        }
    }
}
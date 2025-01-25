/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    public class PreallocatedResourceStorageHandle<TResource>
        : AReadOnlyResourceStorageHandle<TResource>
    {
        private TResource value;

        public PreallocatedResourceStorageHandle(
            TResource value,
            IRuntimeResourceManager runtimeResourceManager,
            ILogger logger)
            : base(
                runtimeResourceManager,
                logger)
        {
            this.value = value;
        }
        protected override async Task<TResource> AllocateResource(

            //Async tail
            AsyncExecutionContext asyncContext)
        {
            return value;
        }

        protected override async Task FreeResource(
            TResource resource,

            //Async tail
            AsyncExecutionContext asyncContext)
        {
        }
    }
}
*/
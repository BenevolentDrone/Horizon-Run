/*
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.ResourceManagement
{
    public interface IResourceData
        : IReadOnlyResourceData
    {
        Task AddVariant(
            IResourceVariantData variant,
            bool allocate = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task RemoveVariant(
            int variantIDHash,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task RemoveVariant(
            string variantID,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ClearAllVariants(
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        IReadOnlyResourceData ParentResource { set; }

        Task AddNestedResource(
            IReadOnlyResourceData nestedResource,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task RemoveNestedResource(
            int nestedResourceIDHash,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task RemoveNestedResource(
            string nestedResourceID,
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task ClearAllNestedResources(
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);

        Task Clear(
            bool free = true,

            //Async tail
            AsyncExecutionContext asyncContext);
    }
}
*/
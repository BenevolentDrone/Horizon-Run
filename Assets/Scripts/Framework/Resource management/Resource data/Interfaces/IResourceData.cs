using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.ResourceManagement
{
    /// <summary>
    /// Represents resource data
    /// </summary>
    public interface IResourceData
        : IReadOnlyResourceData
    {
        Task AddVariant(
            IResourceVariantData variant,
            bool allocate = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task RemoveVariant(
            int variantIDHash,
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task RemoveVariant(
            string variantID,
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task ClearAllVariants(
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        IReadOnlyResourceData ParentResource { set; }

        Task AddNestedResource(
            IReadOnlyResourceData nestedResource,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task RemoveNestedResource(
            int nestedResourceIDHash,
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task RemoveNestedResource(
            string nestedResourceID,
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task ClearAllNestedResources(
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);

        Task Clear(
            bool free = true,

            //Async tail
            CancellationToken cancellationToken = default,
            IProgress<float> progress = null,
            ILogger progressLogger = null);
    }
}
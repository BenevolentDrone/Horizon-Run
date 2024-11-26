using HereticalSolutions.Repositories;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Pools
{
    public interface IPoolElementFacadeWithMetadata<T>
        : IPoolElementFacade<T>
    {
        /// <summary>
        /// Gets the metadata of the pool element.
        /// </summary>
        StronglyTypedMetadata Metadata { get; }
    }
}
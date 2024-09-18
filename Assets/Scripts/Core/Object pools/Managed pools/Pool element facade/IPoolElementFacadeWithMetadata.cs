using HereticalSolutions.Repositories;

namespace HereticalSolutions.Pools
{
    public interface IPoolElementFacadeWithMetadata<T>
        : IPoolElementFacade<T>
    {
        /// <summary>
        /// Gets the metadata of the pool element.
        /// </summary>
        IReadOnlyObjectRepository Metadata { get; }
    }
}
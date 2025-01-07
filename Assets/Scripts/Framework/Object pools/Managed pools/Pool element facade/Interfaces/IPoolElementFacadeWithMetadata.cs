using HereticalSolutions.Metadata;

namespace HereticalSolutions.Pools
{
    public interface IPoolElementFacadeWithMetadata<T>
        : IPoolElementFacade<T>
    {
        IStronglyTypedMetadata Metadata { get; }
    }
}
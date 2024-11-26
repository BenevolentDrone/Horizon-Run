using HereticalSolutions.Repositories;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Synchronization
{
    public interface ISynchronizable
    {
        SynchronizationDescriptor Descriptor { get; }

        IStronglyTypedMetadata Metadata { get; }
    }
}
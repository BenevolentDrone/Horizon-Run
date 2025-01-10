using HereticalSolutions.Collections;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static class ObjectPoolMetadataFactory
    {
        public static IndexedMetadata BuildIndexedMetadata()
        {
            return new IndexedMetadata();
        }

        public static MetadataAllocationDescriptor BuildIndexedMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IIndexed),
                ConcreteType = typeof(IndexedMetadata)
            };
        }
    }
}
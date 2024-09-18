using HereticalSolutions.Collections;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static class ObjectPoolsMetadataFactory
    {
        /// <summary>
        /// Builds an indexed metadata object.
        /// </summary>
        /// <returns>An indexed metadata object.</returns>
        public static IndexedMetadata BuildIndexedMetadata()
        {
            return new IndexedMetadata();
        }

        /// <summary>
        /// Builds a metadata allocation descriptor for an indexed metadata object.
        /// </summary>
        /// <returns>A metadata allocation descriptor for an indexed metadata object.</returns>
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
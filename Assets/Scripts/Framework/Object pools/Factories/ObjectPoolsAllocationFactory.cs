using HereticalSolutions.Metadata.Allocations;
using HereticalSolutions.Metadata.Factories;

namespace HereticalSolutions.Pools.Factories
{
    public static class ObjectPoolsAllocationFactory
    {
        public static IPoolElementFacade<T> BuildPoolElementFacade<T>(
            MetadataAllocationDescriptor[] metadataDescriptors = null)
        {
            var metadata = MetadataFactory.BuildStronglyTypedMetadata(
                metadataDescriptors);
                
            return new PoolElementFacade<T>(
                metadata);
        }
        
        public static IPoolElementFacade<T> BuildPoolElementFacadeWithArrayIndex<T>(
            MetadataAllocationDescriptor[] metadataDescriptors = null)
        {
            var metadata = MetadataFactory.BuildStronglyTypedMetadata(
                metadataDescriptors);
                
            return new PoolElementFacadeWithArrayIndex<T>(
                metadata);
        }
        
        public static IPoolElementFacade<T> BuildPoolElementFacadeWithLinkedList<T>(
            MetadataAllocationDescriptor[] metadataDescriptors = null)
        {
            var metadata = MetadataFactory.BuildStronglyTypedMetadata(
                metadataDescriptors);
                
            return new PoolElementFacadeWithLinkedListLink<T>(
                metadata);
        }
    }
}
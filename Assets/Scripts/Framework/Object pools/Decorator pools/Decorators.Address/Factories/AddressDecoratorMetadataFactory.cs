using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static class AddressDecoratorMetadataFactory
    {
        public static AddressMetadata BuildAddressMetadata()
        {
            return new AddressMetadata();
        }

        public static MetadataAllocationDescriptor BuildAddressMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsAddress),
                
                ConcreteType = typeof(AddressMetadata)
            };
        }
    }
}
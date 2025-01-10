using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Pools.Factories
{
    public static class VariantDecoratorMetadataFactory
    {
        public static VariantMetadata BuildVariantMetadata()
        {
            return new VariantMetadata();
        }

        public static MetadataAllocationDescriptor BuildVariantMetadataDescriptor()
        {
            return new MetadataAllocationDescriptor
            {
                BindingType = typeof(IContainsVariant),
                ConcreteType = typeof(VariantMetadata)
            };
        }
    }
}
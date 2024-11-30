using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Metadata.Allocations;

namespace HereticalSolutions.Metadata.Factories
{
	public static class MetadataFactory
	{
		public static StronglyTypedMetadata BuildStronglyTypedMetadata()
		{
			return new StronglyTypedMetadata(
				RepositoriesFactory.BuildDictionaryObjectRepository());
		}

		public static StronglyTypedMetadata BuildStronglyTypedMetadata(
			MetadataAllocationDescriptor[] metadataDescriptors)
		{
			var repository = RepositoriesFactory.BuildDictionaryObjectRepository();

			if (metadataDescriptors != null)
				foreach (var descriptor in metadataDescriptors)
				{
					if (descriptor == null)
						continue;

					repository.Add(
						descriptor.BindingType,
						AllocationsFactory.ActivatorAllocationDelegate(
							descriptor.ConcreteType));
				}

			return new StronglyTypedMetadata(
				repository);
		}

		public static WeaklyTypedMetadata BuildWeaklyTypedMetadata()
		{
			return new WeaklyTypedMetadata(
				RepositoriesFactory.BuildDictionaryRepository<string, object>());
		}
	}
}
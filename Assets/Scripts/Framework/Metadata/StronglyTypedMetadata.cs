using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Metadata
{
	public class StronglyTypedMetadata
		: IStronglyTypedMetadata
	{
		private readonly IReadOnlyObjectRepository metadataRepository;

		public StronglyTypedMetadata(
			IReadOnlyObjectRepository metadataRepository)
		{
			this.metadataRepository = metadataRepository;
		}

		#region IStronglyTypedMetadata

		public bool Has<TMetadata>()
		{
			return metadataRepository.Has<TMetadata>();
		}

		public TMetadata Get<TMetadata>()
		{
			return metadataRepository.Get<TMetadata>();
		}

		#endregion
	}
}
using HereticalSolutions.Repositories;

namespace HereticalSolutions.Metadata
{
	public class StronglyTypedMetadata
		: IStronglyTypedMetadata
	{
		private readonly IInstanceRepository metadataRepository;

		public StronglyTypedMetadata(
			IInstanceRepository metadataRepository)
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

		public bool TryGet<TMetadata>(
			out TMetadata value)
		{
			return metadataRepository.TryGet<TMetadata>(
				out value);
		}

		public bool TryAdd<TMetadata>(
			TMetadata value)
		{
			return metadataRepository.TryAdd<TMetadata>(
				value);
		}

		public void AddOrUpdate<TMetadata>(
			TMetadata value)
		{
			metadataRepository.AddOrUpdate<TMetadata>(
				value);
		}

		public void Remove<TMetadata>()
		{
			metadataRepository.Remove<TMetadata>();
		}

		public bool TryRemove<TMetadata>()
		{
			return metadataRepository.TryRemove<TMetadata>();
		}

		public void Clear()
		{

		}

		#endregion
	}
}
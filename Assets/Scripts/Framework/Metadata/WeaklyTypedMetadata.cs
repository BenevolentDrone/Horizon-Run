using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Metadata
{
	public class WeaklyTypedMetadata
		: IWeaklyTypedMetadata
	{
		private readonly IRepository<string, object> metadataRepository;

		public WeaklyTypedMetadata(
			IRepository<string, object> metadataRepository)
		{
			this.metadataRepository = metadataRepository;
		}

		#region IWeaklyTypedMetadata

		public bool Has(
			string key)
		{
			return metadataRepository.Has(key);
		}

		public object Get(
			string key)
		{
			return metadataRepository.Get(key);
		}

		public TMetadata Get<TMetadata>(
			string key)
		{
			return Get(key).CastFromTo<object, TMetadata>();
		}

		public bool TryGet(
			string key,
			out object value)
		{
			return metadataRepository.TryGet(
				key,
				out value);
		}

		public bool TryGet<TMetadata>(
			string key,
			out TMetadata value)
		{
			if (TryGet(
				key,
				out object valueObject))
			{
				value = valueObject.CastFromTo<object, TMetadata>();

				return true;
			}

			value = default;

			return false;
		}

		public bool TryAdd(
			string key,
			object value)
		{
			return metadataRepository.TryAdd(
				key,
				value);
		}

		public void AddOrUpdate(
			string key,
			object value)
		{
			metadataRepository.AddOrUpdate(
				key,
				value);
		}

		public void Remove(
			string key)
		{
			metadataRepository.Remove(key);
		}

		public bool TryRemove(
			string key)
		{
			return metadataRepository.TryRemove(key);
		}

		public void Clear()
		{
			metadataRepository.Clear();
		}

		#endregion
	}
}
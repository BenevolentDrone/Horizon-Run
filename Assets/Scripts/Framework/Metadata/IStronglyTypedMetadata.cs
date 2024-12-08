using System;

namespace HereticalSolutions.Metadata
{
	public interface IStronglyTypedMetadata
	{
		bool Has<TMetadata>();

		TMetadata Get<TMetadata>();

		bool TryGet<TMetadata>(
			out TMetadata value);

		bool TryAdd<TMetadata>(
			TMetadata value);

		void AddOrUpdate<TMetadata>(
			TMetadata value);

		void Remove<TMetadata>();

		bool TryRemove<TMetadata>();

		void Clear();
	}
}
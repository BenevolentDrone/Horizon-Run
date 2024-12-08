using System;

namespace HereticalSolutions.Metadata
{
	public interface IWeaklyTypedMetadata
	{
		bool Has(
			string key);

		object Get(
			string key);

		TMetadata Get<TMetadata>(
			string key);

		bool TryGet(
			string key,
			out object value);

		bool TryGet<TMetadata>(
			string key,
			out TMetadata value);

		bool TryAdd(
			string key,
			object value);

		void AddOrUpdate(
			string key,
			object value);

		void Remove(
			string key);

		bool TryRemove(
			string key);

		void Clear();
	}
}
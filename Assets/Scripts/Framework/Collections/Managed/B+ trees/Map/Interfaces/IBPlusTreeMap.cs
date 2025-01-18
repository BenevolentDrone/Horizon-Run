using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public interface IBPlusTreeMap<TKey, TValue>
	{
		bool Search(
			TKey key,
			out TValue value);

		void Insert(
			TKey key,
			TValue value);

		bool Remove(
			TKey key);

		int Count { get; }

		IEnumerable<TKey> AllKeys { get; }

		IEnumerable<TValue> AllValues { get; }

		void Clear();
	}
}
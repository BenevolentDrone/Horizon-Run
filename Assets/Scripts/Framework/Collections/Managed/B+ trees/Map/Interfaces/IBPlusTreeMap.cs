using System;
using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public interface IBPlusTreeMap<TKey, TValue>
	{
		void Insert(
			TKey key,
			TValue value);

		bool Search(
			TKey key,
			out TValue value);

		bool Remove(
			TKey key);

		int Count { get; }

		IEnumerable<TKey> AllKeys { get; }

		IEnumerable<TValue> AllValues { get; }

		//Suggested by ChatGPT. ¯\_(ツ)_/¯
		void InOrderTraversal(
			Action<TKey, TValue> action);

		void Clear();
	}
}
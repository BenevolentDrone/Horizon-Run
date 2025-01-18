using System.Collections.Generic;

namespace HereticalSolutions.Collections.Managed
{
	public interface IBPlusTree<T>
	{
		bool Search(
			T key);

		void Insert(
			T key);
		
		bool Remove(
			T key);

		int Count { get; }

		IEnumerable<T> All { get; }

		void Clear();
	}
}
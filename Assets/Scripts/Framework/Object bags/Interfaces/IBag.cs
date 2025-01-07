using System.Collections.Generic;

namespace HereticalSolutions.Bags
{
	public interface IBag<T>
	{
		bool Push(
			T instance);

		bool Pop(
			T instance);

		int Count { get; }

		IEnumerable<T> All { get; }

		void Clear();
	}
}
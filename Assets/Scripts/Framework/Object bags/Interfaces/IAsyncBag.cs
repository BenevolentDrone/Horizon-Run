using System.Collections.Generic;
using System.Threading.Tasks;

namespace HereticalSolutions.Bags
{
	//TODO: design this properly
	public interface IAsyncBag<T>
	{
		Task<bool> Push(
			T instance);

		Task<bool> Pop(
			T instance);

		int Capacity { get; }

		IEnumerable<T> All { get; }
	}
}
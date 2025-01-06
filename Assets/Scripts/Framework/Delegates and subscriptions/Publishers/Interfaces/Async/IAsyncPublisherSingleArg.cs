using System;
using System.Threading.Tasks;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherSingleArg
	{
		Task Publish<TValue>(
			TValue value);

		Task Publish(
			Type valueType,
			object value);
	}
}
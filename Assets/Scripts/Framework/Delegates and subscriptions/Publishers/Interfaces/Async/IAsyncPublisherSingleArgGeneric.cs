using System.Threading.Tasks;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherSingleArgGeneric<TValue>
	{
		Task Publish(
			TValue value);
	}
}
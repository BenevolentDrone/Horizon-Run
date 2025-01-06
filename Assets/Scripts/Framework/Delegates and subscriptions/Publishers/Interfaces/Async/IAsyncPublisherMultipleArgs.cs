using System.Threading.Tasks;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherMultipleArgs
	{
		Task Publish(
			object[] values);
	}
}
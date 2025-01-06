using System.Threading.Tasks;

namespace HereticalSolutions.Delegates
{
	public interface IAsyncPublisherNoArgs
	{
		Task Publish();
	}
}
using System.Threading.Tasks;

namespace HereticalSolutions.Allocations
{
	public interface IAsyncAllocationResizeable
	{
		Task Resize();
	}
}
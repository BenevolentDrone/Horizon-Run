using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IREPL
	{
		Task<bool> Execute(
			string instruction,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}
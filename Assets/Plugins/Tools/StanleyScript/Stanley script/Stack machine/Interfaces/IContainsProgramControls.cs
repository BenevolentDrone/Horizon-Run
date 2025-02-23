using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsProgramControls
	{
		void SetCurrentProgramCounter(
			int line);

		void SetCurrentLineCounter(
			int line);

		void PollEvents(
			IStanleyContextInternal context);

		Task PollEventsAsync(
			IStanleyContextInternal context,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}
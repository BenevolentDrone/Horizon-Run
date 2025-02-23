using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsControls
	{
		void Start();

		Task StartAsync(

			//Async tail
			AsyncExecutionContext asyncContext);

		void Step();

		Task StepAsync(

			//Async tail
			AsyncExecutionContext asyncContext);

		void Pause();

		void Resume();

		void Stop();
	}
}
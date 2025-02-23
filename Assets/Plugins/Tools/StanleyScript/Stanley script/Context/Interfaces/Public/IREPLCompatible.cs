using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
	public interface IREPLCompatible
	{
		void Interpret(
			string input);

		Task InterpretAsync(
			string input,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}
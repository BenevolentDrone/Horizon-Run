using System;
using System.Threading;
using System.Threading.Tasks;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyOperation
	{
		string Opcode { get; }

		string[] Aliases { get; }

		bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment);

		Task<bool> Handle(
			string[] instructionTokens,
			IRuntimeEnvironment environment,

			//Async tail
			CancellationToken cancellationToken = default,
			IProgress<float> progress = null,
			ILogger progressLogger = null);
	}
}
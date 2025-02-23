using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class WaitMilliseconds
		: AStanleyInstructionHandler
	{
		public WaitMilliseconds(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_WAIT_MS;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(int)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 0;

		public override Type[] ReturnValueTypes => Array.Empty<Type>();

		#endregion

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			var currentLogger = SelectLogger(
				context);

			currentLogger?.LogError(
				GetType(),
				"WAITING IS NOT SUPPORTED IN SYNCHRONOUS CONTEXT");

			return false;
		}

		public override async Task<bool> Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var currentLogger = SelectLogger(
				context);

			var stack = context.StackMachine;

			if (!stack.Pop(
				out var durationVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<int>(
				durationVariable,
				currentLogger))
				return false;

			var duration = durationVariable.GetValue<int>();

			await Task.Delay(
				duration);

			return true;
		}

		#endregion
	}
}
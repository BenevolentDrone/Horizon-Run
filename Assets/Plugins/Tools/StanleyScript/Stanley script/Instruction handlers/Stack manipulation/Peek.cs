using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class Peek
		: AStanleyInstructionHandler
	{
		public Peek(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_STACK_PEEK;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 0;

		public override Type[] ArgumentTypes => Array.Empty<Type>();

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			null
		};

		#endregion

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			var currentLogger = SelectLogger(
				context);

			var stack = context.StackMachine;

			if (!stack.Peek(
				out var peekedVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			stack.Push(
				peekedVariable);

			return true;
		}

		#endregion
	}
}
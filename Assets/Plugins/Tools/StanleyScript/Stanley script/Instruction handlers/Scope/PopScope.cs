using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PopScope
		: AStanleyInstructionHandler
	{
		public PopScope(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_POP_SCOPE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 0;

		public override Type[] ArgumentTypes => Array.Empty<Type>();

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
			
			bool result = context.StackMachine.PopScope();

			if (!result)
			{
				currentLogger?.LogError(
					GetType(),
					"COULD NOT POP SCOPE");
			}

			return result;
		}

		#endregion
	}
}
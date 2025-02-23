using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class Return
		: AStanleyInstructionHandler
	{
		public Return(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_RETURN;

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

			var stack = context.StackMachine;

			currentLogger?.Log(
				GetType(),
				"PROGRAM WAS FINISHED SUCCESSFULLY");

			//¯\_(ツ)_/¯
			//TODO: find out how to make it proper way
			stack.SetCurrentProgramCounter(
				-2); //-2 AND NOT -1 BECAUSE IT RETURNS TRUE AND THUS THE ExecuteNext(Async) METHOD WOULD DO pc++ AND IT WOULD BE 0 WHICH, INSTEAD OF STOPPING THE PROGRAM, WOULD LOOP IT AGAIN

			return true;
		}

		#endregion
	}
}
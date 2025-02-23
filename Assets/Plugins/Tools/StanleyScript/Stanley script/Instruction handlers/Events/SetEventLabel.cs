using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class SetEventLabel
		: AStanleyInstructionHandler
	{
		public SetEventLabel(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_EVENT_SET_LABEL;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyEvent),
			typeof(string)
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

			var stack = context.StackMachine;

			if (!stack.Pop(
				out var eventVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyEvent>(
				eventVariable,
				currentLogger))
				return false;

			if (!stack.Pop(
				out var labelVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				labelVariable,
				currentLogger))
				return false;

			StanleyEvent @event = eventVariable.GetValue<StanleyEvent>();

			string labelString = labelVariable.GetValue<string>();

			@event.Label = labelString;

			return true;
		}

		#endregion
	}
}
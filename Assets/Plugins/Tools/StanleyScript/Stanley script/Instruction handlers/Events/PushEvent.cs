using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushEvent
		: AStanleyInstructionHandler
	{
		public PushEvent(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_EVENT;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(bool),
			typeof(string)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(StanleyEvent)
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

			if (!stack.Pop(
				out var jumpToLabelVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<bool>(
				jumpToLabelVariable,
				currentLogger))
				return false;

			if (!stack.Pop(
				out var labelStringVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				labelStringVariable,
				currentLogger))
				return false;

			bool jumpToLabel = jumpToLabelVariable.GetValue<bool>();

			string labelString = labelStringVariable.GetValue<string>();

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyEvent),
					StanleyFactory.BuildStanleyEvent(
						null,
						jumpToLabel,
						labelString),
					loggerResolver));

			return true;
		}

		#endregion
	}
}
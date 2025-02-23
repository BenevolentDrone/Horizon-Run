using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class AllocLabel
		: AStanleyInstructionHandler
	{
		public AllocLabel(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_ALLOC_LABEL;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(string),
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

			var stack = context.StackMachine;

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

			if (!stack.Pop(
				out var pcVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<int>(
				pcVariable,
				currentLogger))
				return false;

			var labelString = labelStringVariable.GetValue<string>();

			var pc = pcVariable.GetValue<int>();

			if (StanleyInlineDelegates.TryGetJumpTableVariable(
				stack.CurrentScope,
				out var jumpTableVariable))
			{
				var jumpTable = jumpTableVariable.GetValue<StanleyObject>();

				jumpTable.Properties.TryAddVariable(
					StanleyFactory.BuildValueVariable(
						labelString,
						typeof(int),
						pc,
						loggerResolver));

				return true;
			}

			currentLogger?.LogError(
				GetType(),
				$"COULD NOT FIND JUMP TABLE");

			return false;
		}

		#endregion
	}
}
using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class CopySameTypeVariable
		: AStanleyInstructionHandler
	{
		public CopySameTypeVariable(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_COPY;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 2;

		public override Type[] ArgumentTypes => new Type[]
		{
			null,
			null
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 0;

		public override Type[] ReturnValueTypes => Array.Empty<Type>();

		#endregion

		public override bool WillHandle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			if (!AssertInstruction(
				instruction))
				return false;

			var stack = context.StackMachine;

			if (!AssertMinStackSize(
				stack,
				2))
				return false;

			if (!stack.PeekFromTop(
				0,
				out var variable1))
			{
				return false;
			}

			if (!stack.PeekFromTop(
				1,
				out var variable2))
			{
				return false;
			}

			if (variable1.VariableType != variable2.VariableType)
			{
				return false;
			}

			return true;
		}

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			var currentLogger = SelectLogger(
				context);

			var stack = context.StackMachine;

			if (!stack.Pop(
				out var variable1))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!stack.Pop(
				out var variable2))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			variable2.SetValue(
				variable1.GetValue());

			return true;
		}

		#endregion
	}
}
using System;
using System.Globalization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushFloat
		: AStanleyInstructionHandler
	{
		public PushFloat(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_FLOAT;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 0;

		public override Type[] ArgumentTypes => Array.Empty<Type>();

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(float)
		};

		#endregion

		public override bool WillHandle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			if (!AssertInstruction(
				instruction))
				return false;

			if (!AssertMinInstructionLength(
				instructionTokens,
				2))
				return false;

			if (!AssertInstructionNotEmpty(
				instructionTokens,
				1))
				return false;

			return true;
		}

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			context.StackMachine.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(float),
					float.Parse(
						instructionTokens[1],
						CultureInfo.InvariantCulture),
					loggerResolver));

			return true;
		}

		#endregion
	}
}
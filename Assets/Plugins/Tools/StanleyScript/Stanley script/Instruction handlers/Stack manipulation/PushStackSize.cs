using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushStackSize
		: AStanleyInstructionHandler
	{
		public PushStackSize(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_STACK_PUSH_SIZE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 0;

		public override Type[] ArgumentTypes => Array.Empty<Type>();

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(int)
		};

		#endregion

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			var stack = context.StackMachine;

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(int),
					stack.StackSize,
					loggerResolver));

			return true;
		}

		#endregion
	}
}
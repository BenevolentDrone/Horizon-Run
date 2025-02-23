using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushObject
		: AStanleyInstructionHandler
	{
		public PushObject(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_OBJECT;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 0;

		public override Type[] ArgumentTypes => Array.Empty<Type>();

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(StanleyObject)
		};

		#endregion

		public override bool Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens)
		{
			context.StackMachine.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyObject),
					StanleyFactory.BuildStanleyObject(),
					loggerResolver));

			return true;
		}

		#endregion
	}
}
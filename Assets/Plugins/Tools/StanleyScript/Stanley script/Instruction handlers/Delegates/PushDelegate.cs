using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushDelegate
		: AStanleyInstructionHandler
	{
		public PushDelegate(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_DELEGATE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(string)
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(StanleyDelegate)
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

			var labelName = labelVariable.GetValue<string>();
			
			context.StackMachine.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyDelegate),
					StanleyFactory.BuildStanleyDelegate(
						context,
						(builder) => StanleyInstructionEmitter.EmitDelegateJump(
							builder,
							labelName)),
						//labelName),
					loggerResolver));

			return true;
		}
		
		public override async Task<bool> Handle(
			IStanleyContextInternal context,
			string instruction,
			string[] instructionTokens,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var currentLogger = SelectLogger(
				context);

			var stack = context.StackMachine;

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

			var labelName = labelVariable.GetValue<string>();
			
			context.StackMachine.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyDelegate),
					StanleyFactory.BuildStanleyDelegate(
						context,
						(builder) => StanleyInstructionEmitter.EmitDelegateJump(
							builder,
							labelName),
						//labelName,
						
						asyncContext),
					loggerResolver));

			return true;
		}

		#endregion
	}
}
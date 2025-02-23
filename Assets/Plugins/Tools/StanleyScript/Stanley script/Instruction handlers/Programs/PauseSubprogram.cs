using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PauseSubprogram
		: AStanleyInstructionHandler
	{
		public PauseSubprogram(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_SUBPROGRAM_PAUSE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyHandle)
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
				out var scopeHandleVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyHandle>(
				scopeHandleVariable,
				currentLogger))
				return false;

			var scopeHandle = scopeHandleVariable.GetValue<StanleyHandle>();

			if (!context.TryGetChildContext(
				scopeHandle,
				out var childContext))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT GET CHILD CONTEXT BY HANDLE: {scopeHandle.Value}");

				return false;
			}

			childContext.Pause();

			return true;
		}

		#endregion
	}
}
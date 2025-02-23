using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushSubprogram
		: AStanleyInstructionHandler
	{
		public PushSubprogram(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_SUBPROGRAM;

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
			typeof(StanleyHandle)
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
				out var programNameVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				programNameVariable,
				currentLogger))
				return false;

			string programName = programNameVariable.GetValue<string>();

			if (!context.CreateChildContext(
				out var handle))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT CREATE CHILD CONTEXT");

				return false;
			}

			if (!context.TryGetChildContext(
				handle,
				out var childContext))
			{
				currentLogger?.LogError(
					GetType(),
					$"COULD NOT GET CHILD CONTEXT BY HANDLE: {handle.Value}");

				return false;
			}

			childContext.LoadProgramFromLibrary(
				programName);

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyHandle),
					handle,
					loggerResolver));

			return true;
		}

		#endregion
	}
}
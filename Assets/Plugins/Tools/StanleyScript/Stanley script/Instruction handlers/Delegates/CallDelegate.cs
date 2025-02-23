using System;
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class CallDelegate
		: AStanleyInstructionHandler
	{
		public CallDelegate(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_DELEGATE_CALL;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			typeof(StanleyDelegate)
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
				out var variableNameVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyDelegate>(
				variableNameVariable,
				currentLogger))
				return false;

			var @delegate = variableNameVariable.GetValue<StanleyDelegate>();

			object instance = null;

			if (@delegate.IsInstanceMethod)
			{
				if (@delegate.Target != null)
				{
					instance = @delegate.Target;
				}
				else
				{
					if (!stack.Pop(
						    out var instanceVariable))
					{
						currentLogger?.LogError(
							GetType(),
							"STACK VARIABLE NOT FOUND");

						return false;
					}

					if (!AssertVariableType(
						    @delegate.InstanceType,
						    instanceVariable,
						    currentLogger))
						return false;

					instance = instanceVariable.GetValue();
				}
			}

			object[] arguments = new object[@delegate.ArgumentsCount];

			for (int i = 0; i < @delegate.ArgumentsCount; i++)
			{
				if (!stack.Pop(
					out var argumentVariable))
				{
					currentLogger?.LogError(
						GetType(),
						"STACK VARIABLE NOT FOUND");

					return false;
				}

				if (!AssertVariableType(
					@delegate.ArgumentTypes[i],
					argumentVariable,
					currentLogger))
					return false;

				arguments[i] = argumentVariable.GetValue();
			}

			object result = null;

			if (@delegate.IsAsync)
			{
				if (@delegate.AwaitCompletion)
				{
					currentLogger?.LogError(
						GetType(),
						"RUNNING AN ASYNC DELEGATE IN SYNC CONTEXT IS NOT SUPPORTED");

					return false;
				}
				else
				{
					// Just invoke and don't await - the task starts automatically
					_ = (Task)@delegate.MethodInfo.Invoke(
						instance,
						arguments);
				}
			}
			else
			{
				result = @delegate.MethodInfo.Invoke(
					instance,
					arguments);
			}

			if (@delegate.ReturnType != typeof(void))
			{
				stack.Push(
					StanleyFactory.BuildValueVariable(
						StanleyConsts.RVALUE_VARIABLE_NAME,
						@delegate.ReturnType,
						result,
						loggerResolver));
			}

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
				out var variableNameVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<StanleyDelegate>(
				variableNameVariable,
				currentLogger))
				return false;

			var @delegate = variableNameVariable.GetValue<StanleyDelegate>();

			object instance = null;

			if (@delegate.IsInstanceMethod)
			{
				if (@delegate.Target != null)
				{
					instance = @delegate.Target;
				}
				else
				{
					if (!stack.Pop(
						    out var instanceVariable))
					{
						currentLogger?.LogError(
							GetType(),
							"STACK VARIABLE NOT FOUND");

						return false;
					}

					if (!AssertVariableType(
						    @delegate.InstanceType,
						    instanceVariable,
						    currentLogger))
						return false;

					instance = instanceVariable.GetValue();
				}
			}

			object[] arguments = new object[@delegate.ArgumentsCount];

			//for (int i = @delegate.ArgumentsCount - 1; i >= 0; i--)
			for (int i = 0; i < @delegate.ArgumentsCount; i++)
			{
				if (!stack.Pop(
					out var argumentVariable))
				{
					currentLogger?.LogError(
						GetType(),
						"STACK VARIABLE NOT FOUND");

					return false;
				}

				if (!AssertVariableType(
					@delegate.ArgumentTypes[i],
					argumentVariable,
					currentLogger))
					return false;

				arguments[i] = argumentVariable.GetValue();
			}

			object result = null;

			if (@delegate.IsAsync)
			{
				Task task = (Task)@delegate.MethodInfo.Invoke(
					instance,
					arguments);

				if (@delegate.AwaitCompletion)
				{
					await task;

					result = task.GetType().GetProperty("Result").GetValue(task, null);
				}
				else
				{
					//FOR SOME REASON IT BREAKS THE EXECUTION SO JUST DON'T
					//task.Start();
				}
			}
			else
			{
				result = @delegate.MethodInfo.Invoke(
					instance,
					arguments);
			}

			if (@delegate.ReturnType != typeof(void))
			{
				stack.Push(
					StanleyFactory.BuildValueVariable(
						StanleyConsts.RVALUE_VARIABLE_NAME,
						@delegate.ReturnType,
						result,
						loggerResolver));
			}

			return true;
		}

		#endregion
	}
}
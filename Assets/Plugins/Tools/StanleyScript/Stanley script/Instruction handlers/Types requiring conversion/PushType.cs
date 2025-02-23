using System;
using System.Reflection;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushType
		: AStanleyInstructionHandler
	{
		public PushType(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_TYPE;

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
			typeof(Type)
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
				out var typeStringVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariableType<string>(
				typeStringVariable,
				currentLogger))
				return false;

			string typeString = typeStringVariable.GetValue<string>();

			//TODO: optimize
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				//ReflectionTypeLoadException: Exception of type 'System.Reflection.ReflectionTypeLoadException' was thrown.
				try
				{
					Type type = assembly.GetType(
						typeString,
						false, //do not throw exception, just gimme null
						true); //ignore case
	
					if (type != null)
					{
						stack.Push(
							StanleyFactory.BuildValueVariable(
								StanleyConsts.RVALUE_VARIABLE_NAME,
								typeof(Type),
								type,
								loggerResolver));
	
						return true;
					}
				}
				catch (ReflectionTypeLoadException e)
				{
					foreach (Exception ex in e.LoaderExceptions)
					{
						currentLogger?.LogError(
							GetType(),
							ex.Message);
					}

					continue;
				}
			}

			currentLogger?.LogError(
				GetType(),
				$"TYPE {typeString} NOT FOUND");

			return false;
		}

		#endregion
	}
}
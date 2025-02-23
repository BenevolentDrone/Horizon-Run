using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class PushHandle
		: AStanleyInstructionHandler
	{
		public PushHandle(
			ILoggerResolver loggerResolver,
			ILogger logger)
			: base(
				loggerResolver,
				logger)
		{
		}

		#region IStanleyInstructionHandler

		#region Instruction

		public override string Instruction => StanleyOpcodes.OP_PUSH_HANDLE;

		#endregion

		#region Arguments

		public override int ArgumentsCount => 1;

		public override Type[] ArgumentTypes => new Type[]
		{
			null
		};

		#endregion

		#region Return values

		public override int ReturnValuesCount => 1;

		public override Type[] ReturnValueTypes => new Type[]
		{
			typeof(StanleyHandle)
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

			var stack = context.StackMachine;

			if (!AssertMinStackSize(
				stack,
				1))
				return false;

			if (!stack.PeekFromTop(
				0,
				out var sourceVariable))
			{
				return false;
			}

			//The list is composed ont the base of this reference:
			//https://learn.microsoft.com/en-us/dotnet/api/system.convert.tobyte?view=net-8.0
			if (sourceVariable.VariableType != typeof(sbyte)
				&& sourceVariable.VariableType != typeof(ulong)
				&& sourceVariable.VariableType != typeof(uint)
				&& sourceVariable.VariableType != typeof(ushort)
				&& sourceVariable.VariableType != typeof(float)
				&& sourceVariable.VariableType != typeof(short)
				&& sourceVariable.VariableType != typeof(long)
				&& sourceVariable.VariableType != typeof(byte)
				&& sourceVariable.VariableType != typeof(char)
				&& sourceVariable.VariableType != typeof(DateTime)
				&& sourceVariable.VariableType != typeof(double)
				&& sourceVariable.VariableType != typeof(bool)
				&& sourceVariable.VariableType != typeof(int)
				&& sourceVariable.VariableType != typeof(decimal)
				&& sourceVariable.VariableType != typeof(StanleyHandle)
				&& sourceVariable.VariableType != typeof(string))
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
				out var handleSourceVariable))
			{
				currentLogger?.LogError(
					GetType(),
					"STACK VARIABLE NOT FOUND");

				return false;
			}

			var originalValue = handleSourceVariable.GetValue();

			byte convertedValue;

			switch (originalValue)
			{
				case string stringValue:
					convertedValue = byte.Parse(
						stringValue);

					break;

				case StanleyHandle handle:
					convertedValue = handle.Value;

					break;

				default:
					convertedValue = Convert.ToByte(
						originalValue);

					break;
			}

			stack.Push(
				StanleyFactory.BuildValueVariable(
					StanleyConsts.RVALUE_VARIABLE_NAME,
					typeof(StanleyHandle),
					StanleyFactory.BuildStanleyHandle(
						convertedValue),
					loggerResolver));

			return true;
		}

		#endregion
	}
}
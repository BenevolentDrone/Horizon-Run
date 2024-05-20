using System;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	public class ConcatenateToScalarObject
		: AStanleyOperation
	{
		#region IStanleyOperation

		public override string Opcode => "OP_SCAL";

		public override bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment)
		{
			if (!AssertOpcode(instructionTokens))
				return false;

			return true;
		}

		public override async Task<bool> Handle(
			string[] instructionTokens,
			IRuntimeEnvironment environment,
			CancellationToken token)
		{
			var stack = environment as IStackMachine;

			var reportable = environment as IReportable;

			//REMEMBER: when popping from the stack, the order is reversed

			//Get variable to concatenate
			if (!stack.Pop(
				out var variableToMultiply))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable(variableToMultiply, reportable))
				return false;

			//Get amount
			if (!stack.Pop(
				out var amount))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable(amount, reportable))
				return false;

			double amountValue;

			if (amount.VariableType == typeof(int))
			{
				amountValue = Convert.ToDouble(amount.GetValue<int>());
			}
			else if (amount.VariableType == typeof(float))
			{
				amountValue = Convert.ToDouble(amount.GetValue<float>());
			}
			else
			{
				reportable.Log("INVALID AMOUNT VARIABLE TYPE");

				return false;
			}

			if (variableToMultiply.VariableType == typeof(string))
			{
				stack.Push(
					new StanleyCachedVariable(
						StanleyConsts.TEMPORARY_VARIABLE,
						typeof(StanleyScalarPropertyObject),
						new StanleyScalarPropertyObject
						{
							Amount = amountValue,

							Property = variableToMultiply.GetValue<string>()
						}));
			}
			else
			{
				stack.Push(
					new StanleyCachedVariable(
						StanleyConsts.TEMPORARY_VARIABLE,
						typeof(StanleyScalarVariableObject),
						new StanleyScalarVariableObject
						{
							Amount = amountValue,

							Variable = variableToMultiply
						}));
			}

			return true;
		}

		#endregion
	}
}
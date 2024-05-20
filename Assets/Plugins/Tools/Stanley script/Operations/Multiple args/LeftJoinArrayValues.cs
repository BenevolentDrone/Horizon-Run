using System;
using System.Threading;
using System.Threading.Tasks;

namespace HereticalSolutions.StanleyScript
{
	//The signature should look like this:
	//opcode WHOM
	public class LeftJoinArrayValues
		: AStanleyOperation
	{
		private readonly string opcode;

		private readonly string[] aliases;

		public LeftJoinArrayValues(
			string opcode,
			string[] aliases)
		{
			this.opcode = opcode;

			this.aliases = aliases;
		}

		#region IStanleyOperation

		public override string Opcode => opcode;

		public override string[] Aliases => aliases;

		public override bool WillHandle(
			string[] instructionTokens,
			IRuntimeEnvironment environment)
		{
			if (!AssertOpcodeOrAlias(instructionTokens))
				return false;

			var stack = environment as IStackMachine;

			if (!AssertStackVariableTypeIsArray(stack, 0))
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

			var REPL = environment as IREPL;

			//Get target variable
			if (!stack.Pop(
				out var target))
			{
				reportable.Log("STACK VARIABLE NOT FOUND");

				return false;
			}

			if (!AssertVariable(target, reportable))
				return false;

			//Unwrap targets
			IStanleyVariable[] targets;

			if (target.VariableType.IsArray)
			{
				var underlyingType = target.VariableType.GetElementType();

				var valueAsArray = target.GetValue<Array>();

				var targetsCount = valueAsArray.Length;

				targets = new IStanleyVariable[targetsCount];

				for (int i = 0; i < targetsCount; i++)
				{
					targets[i] = new StanleyCachedVariable(
						StanleyConsts.TEMPORARY_VARIABLE,
						underlyingType,
						valueAsArray.GetValue(i));
				}
			}
			else
			{
				targets = new IStanleyVariable[] { target };
			}

			//Invoke for each
			foreach (var targetVariable in targets)
			{
				stack.Push(targetVariable);

				bool result = await REPL.Execute(
					opcode,
					token);
					//.ThrowExceptions();

				if (!result)
					return false;
			}

			return true;
		}

		#endregion
	}
}
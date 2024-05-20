using System.Collections.Generic;

using System.Text.RegularExpressions;

using System.Threading;
using System.Threading.Tasks;

//using TaskExtensions = HereticalSolutions.TaskExtensions;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyEnvironment
		: IRuntimeEnvironment,
		  IExecutable,
		  IStackMachine,
		  IREPL,
		  IReportable
	{
		private const string REGEX_SPLIT_BY_WHITESPACE_UNLESS_WITHIN_QUOTES = "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

		private readonly Dictionary<string, IStanleyVariable> inputVariables;

		private readonly Dictionary<string, List<IStanleyOperation>> operationsChainOfResponsibility;

		private readonly Dictionary<string, IStanleyVariable> runtimeVariables;

		private readonly Stack<IStanleyVariable> stack;

		private readonly List<string> report;


		private EExecutionStatus executionStatus;

		private int programCounter = 0;

		private string[] instructions;

		private int currentLine = -1;

		private bool stepMode = false;


		private CancellationTokenSource cancellationTokenSource;


		public StanleyEnvironment(
			Dictionary<string, IStanleyVariable> inputVariables,
			Dictionary<string, List<IStanleyOperation>> operationsChainOfResponsibility,
			Dictionary<string, IStanleyVariable> runtimeVariables,
			Stack<IStanleyVariable> stack,
			List<string> report)
		{
			this.inputVariables = inputVariables;

			this.operationsChainOfResponsibility = operationsChainOfResponsibility;

			this.runtimeVariables = runtimeVariables;

			this.stack = stack;

			this.report = report;

			executionStatus = EExecutionStatus.IDLE;

			instructions = null;

			currentLine = -1;

			programCounter = 0;

			stepMode = false;

			cancellationTokenSource = new CancellationTokenSource();
		}

		#region IRuntimeEnvironment

		public bool LoadInputVariable(
			IStanleyVariable variable)
		{
			string name = variable.Name;

			if (inputVariables.ContainsKey(name))
			{
				return false;
			}

			inputVariables.Add(
				name,
				variable);

			return true;
		}

		public bool LoadOperation(
			IStanleyOperation operation)
		{
			string opcode = operation.Opcode;

			if (operationsChainOfResponsibility.ContainsKey(opcode))
			{
				operationsChainOfResponsibility[opcode].Insert(
					0,
					operation);
			}
			else
			{
				operationsChainOfResponsibility.Add(
					opcode,
					new List<IStanleyOperation>
					{
						operation
					});
			}

			if (operation.Aliases != null)
			{
				foreach (string alias in operation.Aliases)
				{
					if (operationsChainOfResponsibility.ContainsKey(alias))
					{
						operationsChainOfResponsibility[alias].Insert(
							0,
							operation);
					}
					else
					{
						operationsChainOfResponsibility.Add(
							alias,
							new List<IStanleyOperation>
							{
								operation
							});
					}
				}
			}

			return true;
		}

		public bool AddRuntimeVariable(
			IStanleyVariable variable)
		{
			string name = variable.Name;

			if (runtimeVariables.ContainsKey(name))
			{
				return false;
			}

			runtimeVariables.Add(
				name,
				variable);

			return true;
		}

		public void SetCurrentLine(
			int line)
		{
			currentLine = line;
		}

		public bool GetRuntimeVariable(
			string name,
			out IStanleyVariable variable)
		{
			return runtimeVariables.TryGetValue(
				name,
				out variable);
		}

		public bool GetImportVariable(
			string name,
			out IStanleyVariable variable)
		{
			return inputVariables.TryGetValue(
				name,
				out variable);
		}

		public bool GetOperation(
			string opcode,
			out IEnumerable<IStanleyOperation> operations)
		{
			bool reult = operationsChainOfResponsibility.TryGetValue(
				opcode,
				out var operationsList);

			operations = (IEnumerable<IStanleyOperation>)operationsList;

			return reult;
		}

		#endregion

		#region IExecutable

		public EExecutionStatus Status { get => executionStatus; }

		public int CurrentLine { get => currentLine; }

		public int ProgramCounter { get => programCounter; }

		public string[] Instructions { get => instructions; }

		public void LoadProgram(
			string[] instructions)
		{
			Stop();

			this.instructions = instructions;

			currentLine = -1;

			programCounter = 0;

			stepMode = false;

			report.Clear();
		}

		public void Start()
		{
			switch (executionStatus)
			{
				case EExecutionStatus.IDLE:
				case EExecutionStatus.PAUSED:
				case EExecutionStatus.STOPPED:
				{
					if (instructions == null
						|| instructions.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;

					programCounter = 0;

					stepMode = false;

					report.Clear();

					cancellationTokenSource = new CancellationTokenSource();

						//RunInternal(cancellationTokenSource.Token)
						//	.ThrowExceptions();

						//try
						//{
						//	RunInternal(cancellationTokenSource.Token)
						//}
						//catch (Exception e)
						//{
						//}

						//Task.Run(async () =>
						//{
						//	await RunInternal(cancellationTokenSource.Token)
						//		.ThrowExceptions();
						//});

						TaskExtensions.RunSync<bool>(
							() => RunInternal(cancellationTokenSource.Token));
				}

				break;
			}
		}

		public void Step()
		{
			switch (executionStatus)
			{
				case EExecutionStatus.PAUSED:
				{
					stepMode = true;

					executionStatus = EExecutionStatus.RUNNING;

					//ExecuteInternal(cancellationTokenSource.Token);
				}

					break;

				case EExecutionStatus.IDLE:
				case EExecutionStatus.STOPPED:
				{
					if (instructions == null
						|| instructions.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;

					programCounter = 0;

					stepMode = true;

					report.Clear();

					cancellationTokenSource = new CancellationTokenSource();

					RunInternal(cancellationTokenSource.Token);

					//Task.Run(async () =>
					//{
					//	RunInternal(cancellationTokenSource.Token);
					//});
				}

					break;
			}
		}

		public void Pause()
		{
			switch (executionStatus)
			{
				case EExecutionStatus.RUNNING:
					executionStatus = EExecutionStatus.PAUSED;

					break;
			}
		}

		public void Resume()
		{
			switch (executionStatus)
			{
				case EExecutionStatus.PAUSED:

					stepMode = false;

					executionStatus = EExecutionStatus.RUNNING;

					break;
			}
		}

		public void Stop()
		{
			executionStatus = EExecutionStatus.STOPPED;

			stepMode = false;

			StopInternal();
		}

		#endregion

		#region IStackMachine

		public int StackSize => stack.Count;

		public void Push(
			IStanleyVariable variable)
		{
			stack.Push(variable);
		}

		public bool Pop(
			out IStanleyVariable variable)
		{
			if (stack.Count > 0)
			{
				variable = stack.Pop();

				return true;
			}

			variable = null;

			return false;
		}

		public bool Peek(
			out IStanleyVariable variable)
		{
			if (stack.Count > 0)
			{
				variable = stack.Peek();

				return true;
			}

			variable = null;

			return false;
		}

		public bool PeekFromTop(
			int relativeIndex,
			out IStanleyVariable variable)
		{
			if (stack.Count > relativeIndex)
			{
				variable = stack.ToArray()[relativeIndex];

				return true;
			}

			variable = null;

			return false;
		}

		public bool PeekFromBottom(
			int relativeIndex,
			out IStanleyVariable variable)
		{
			if (stack.Count > relativeIndex)
			{
				variable = stack.ToArray()[stack.Count - relativeIndex - 1];

				return true;
			}

			variable = null;

			return false;
		}

		#endregion

		#region IReportable

		public void Log(
			string message)
		{
			report.Add(message);
		}

		public string[] GetReport()
		{
			return report.ToArray();
		}

		#endregion

		#region IREPL

		public async Task<bool> Execute(
			string instruction,
			CancellationToken cancellationToken)
		{
			bool result = false;

			string[] instructionTokens = Regex.Split(
				instruction,
				REGEX_SPLIT_BY_WHITESPACE_UNLESS_WITHIN_QUOTES);

			//string[] instructionTokens = instruction.Split(' ');

			string opcode = instructionTokens[0];

			if (operationsChainOfResponsibility.ContainsKey(opcode))
			{
				bool handled = false;

				foreach (IStanleyOperation operation in operationsChainOfResponsibility[opcode])
				{
					if (operation.WillHandle(
						instructionTokens,
						this))
					{
						result = await operation.Handle(
							instructionTokens,
							this,
							cancellationToken);
							//.ThrowExceptions();

						handled = true;

						break;
					}
				}

				if (!handled)
				{
					Log("OPERATION NOT HANDLED: " + opcode);

					return false;
				}
			}
			else
			{
				Log("UNKNOWN OPERATION: " + opcode);

				return false;
			}

			return result;
		}

		#endregion

		private async Task<bool> RunInternal(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				switch (executionStatus)
				{
					case EExecutionStatus.RUNNING:
					{
						if (programCounter < instructions.Length)
						{
							var result = await ExecuteInternal(cancellationTokenSource.Token);
								//.ThrowExceptions();

							if (!result)
							{
								executionStatus = EExecutionStatus.STOPPED;

								Log("SCRIPT FINISHED DUE TO INTERNAL ERROR. CHECK LOGS FOR DETAILS");

								return false;
							}

							if (PauseBeforeNextLine())
							{
								executionStatus = EExecutionStatus.PAUSED;

								continue;
							}
						}
						else
						{
							stepMode = false;

							executionStatus = EExecutionStatus.FINISHED;

							Log("SCRIPT FINISHED WITH EXECUTION STATUS \"FINISHED\"");

							return true;
						}
					}

						break;

					case EExecutionStatus.PAUSED:

						await Task.Yield();

						continue;

					case EExecutionStatus.IDLE:
					case EExecutionStatus.STOPPED:
					case EExecutionStatus.FINISHED:
						Log($"SCRIPT INTERRUPTED WITH EXECUTION STATUS \"{executionStatus}\"");

						stepMode = false;

						return false;
				}
			}

			Log($"SCRIPT INTERRUPTED WITH EXECUTION STATUS \"{executionStatus}\"");

			stepMode = false;

			return false;
		}

		private void StopInternal()
		{
			//cancellationTokenSource?.Dispose();

			cancellationTokenSource?.Cancel();
		}

		private async Task<bool> ExecuteInternal(CancellationToken cancellationToken)
		{
			string instruction = instructions[programCounter];

			bool result = await Execute(
				instruction,
				cancellationToken);
				//.ThrowExceptions();

			programCounter++;

			return result;
		}

		private bool PauseBeforeNextLine()
		{
			if (!stepMode)
				return false;

			if ((programCounter + 1) >= instructions.Length)
				return false;

			if (instructions[programCounter + 1].StartsWith("OP_LINE"))
				return true;

			return false;
		}
	}
}
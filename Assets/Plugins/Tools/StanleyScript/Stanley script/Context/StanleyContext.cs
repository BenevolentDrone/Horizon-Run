using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyContext
		: IStanleyContextInternal
	{
		private readonly IStackMachine stackMachine;

		private readonly IStanleyCompiler compiler;

		private readonly IReportMaker reportMaker;

		private readonly IStanleyContextInternal parentContext;

		private readonly IRepository<byte, IStanleyContextInternal> childContexts;

		private readonly IIDAllocationController<byte> contextIDAllocationController;

		private readonly IRepository<string, string[]> programLibrary;

		private readonly bool emitShortcutInstructions;

		private readonly bool logAllExecutedCommands;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;


		private string currentProgram;

		private bool stepMode = false;

		private EExecutionStatus executionStatus;

		public StanleyContext(
			IStackMachine stackMachine,

			IStanleyCompiler compiler,

			IReportMaker reportMaker,

			IStanleyContextInternal parentContext,

			IRepository<byte, IStanleyContextInternal> childContexts,

			IIDAllocationController<byte> contextIDAllocationController,

			IRepository<string, string[]> programLibrary,
			
			bool emitShortcutInstructions,
			
			bool logAllExecutedCommands,

			ILoggerResolver loggerResolver,

			ILogger logger)
		{
			this.stackMachine = stackMachine;

			this.compiler = compiler;

			this.reportMaker = reportMaker;

			this.parentContext = parentContext;

			this.childContexts = childContexts;

			this.contextIDAllocationController = contextIDAllocationController;

			this.programLibrary = programLibrary;
			
			this.emitShortcutInstructions = emitShortcutInstructions;
			
			this.logAllExecutedCommands = logAllExecutedCommands;

			this.loggerResolver = loggerResolver;

			this.logger = logger;

			executionStatus = EExecutionStatus.IDLE;
		}

		#region IStanleyContextInternal

		#region IStanleyContext

		public EExecutionStatus ExecutionStatus => executionStatus;

		public IStackMachine StackMachine => stackMachine;

		public IStanleyCompiler Compiler => compiler;

		public IReportMaker ReportMaker => reportMaker;

		#region IContainsProgramLibrary

		public string CurrentProgram => currentProgram;

		public IEnumerable<string> AllProgramNames => programLibrary.Keys;

		public bool LoadProgramFromLibrary(
			string name)
		{
			switch (executionStatus)
			{
				case EExecutionStatus.IDLE:
				case EExecutionStatus.FINISHED:

					break;

				default:
				{
					Stop();

					break;
				}
			}

			if (!programLibrary.TryGet(
				name,
				out string[] program))
			{
				return false;
			}

			currentProgram = name;

			compiler.LoadProgram(
				program);

			if (!compiler.Compile(
				out string[] instructions))
			{
				return false;
			}

			stackMachine.LoadInstructions(
				instructions);

			stepMode = false;

			return true;
		}

		public bool GetProgramFromLibrary(
			string name,
			out string[] program)
		{
			return programLibrary.TryGet(
				name,
				out program);
		}

		public bool SaveProgramToLibrary(
			string name,
			string[] program)
		{
			return programLibrary.TryAdd(
				name,
				program);
		}

		public bool SaveCurrentProgramFromLibrary(
			string name)
		{
			return programLibrary.TryAdd(
				name,
				compiler.ProgramListing);
		}

		public bool RemoveProgramFromLibrary(
			string name)
		{
			return programLibrary.TryRemove(
				name);
		}

		#endregion

		#region IContainsControls

		public void Start()
		{
			switch (executionStatus)
			{
				case EExecutionStatus.IDLE:
				case EExecutionStatus.PAUSED:
				case EExecutionStatus.STOPPED:
				{
					if (stackMachine.InstructionsListing == null
						|| stackMachine.InstructionsListing.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;

					stackMachine.SetCurrentProgramCounter(0);

					stepMode = false;

					reportMaker.InitializeNewReport(
						this);

					bool result = true;

					do
					{
						switch (executionStatus)
						{
							case EExecutionStatus.RUNNING:
							{
								result = stackMachine.ExecuteNext(this);

								if (result)
								{
									stackMachine.PollEvents(
										this);
								}

								break;
							}

							default:
								break;
						}
					}
					while (result);

					reportMaker.FinalizeReport();

					executionStatus = EExecutionStatus.FINISHED;

					break;
				}

				default:
					break;
			}
		}

		public async Task StartAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		 {
			switch (executionStatus)
			{
				case EExecutionStatus.IDLE:
				case EExecutionStatus.PAUSED:
				case EExecutionStatus.STOPPED:
				case EExecutionStatus.FINISHED:
				{
					if (stackMachine.InstructionsListing == null
						|| stackMachine.InstructionsListing.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;

					stackMachine.SetCurrentProgramCounter(0);

					stepMode = false;

					reportMaker.InitializeNewReport(
						this);

					bool result = true;

					do
					{
						switch (executionStatus)
						{
							case EExecutionStatus.RUNNING:
							{
								result = await stackMachine.ExecuteNextAsync(
									this,
									
									asyncContext);

								if (result)
								{
									await stackMachine.PollEventsAsync(
										this,
										
										asyncContext);
								}

								break;
							}

							default:
							{
								await Task.Yield();

								break;
							}
						}
					}
					while (result);

					reportMaker.FinalizeReport();

					executionStatus = EExecutionStatus.FINISHED;

					break;
				}

				default:
					break;
			}
		}

		public void Step()
		{
			switch (executionStatus)
			{
				case EExecutionStatus.PAUSED:
				{
					if (stackMachine.InstructionsListing == null
						|| stackMachine.InstructionsListing.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;
					
					stepMode = true;

					if (!stackMachine.ExecuteNext(this))
					{
						reportMaker.FinalizeReport();
	
						executionStatus = EExecutionStatus.FINISHED;
					}

					break;
				}

				case EExecutionStatus.IDLE:
				case EExecutionStatus.STOPPED:
				{
					if (stackMachine.InstructionsListing == null
						|| stackMachine.InstructionsListing.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;

					stackMachine.SetCurrentProgramCounter(0);

					stepMode = true;

					reportMaker.InitializeNewReport(
						this);

					if (!stackMachine.ExecuteNext(this))
					{
						reportMaker.FinalizeReport();
	
						executionStatus = EExecutionStatus.FINISHED;
					}

					break;
				}

				default:
					break;
			}
		}

		public async Task StepAsync(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			switch (executionStatus)
			{
				case EExecutionStatus.PAUSED:
				{
					if (stackMachine.InstructionsListing == null
						|| stackMachine.InstructionsListing.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;
					
					stepMode = true;

					bool result = await stackMachine.ExecuteNextAsync(
						this,
						
						asyncContext);

					if (!result)
					{
						reportMaker.FinalizeReport();
	
						executionStatus = EExecutionStatus.FINISHED;
					}

					break;
				}

				case EExecutionStatus.IDLE:
				case EExecutionStatus.STOPPED:
				{
					if (stackMachine.InstructionsListing == null
						|| stackMachine.InstructionsListing.Length == 0)
						return;

					executionStatus = EExecutionStatus.RUNNING;

					stackMachine.SetCurrentProgramCounter(0);

					stepMode = true;

					reportMaker.InitializeNewReport(
						this);

					bool result = await stackMachine.ExecuteNextAsync(
						this,
						
						asyncContext);

					if (!result)
					{
						reportMaker.FinalizeReport();
	
						executionStatus = EExecutionStatus.FINISHED;
					}

					break;
				}

				default:
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
		}

		#endregion

		#region IREPLCompatible

		public void Interpret(
			string input)
		{
			compiler.CompileImmediately(
				input,
				out string[] instructions);

			foreach (var instruction in instructions)
			{
				if (!stackMachine.ExecuteImmediately(
					this,
					instruction))
				{
					return;
				}
			}
		}

		public async Task InterpretAsync(
			string input,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			compiler.CompileImmediately(
				input,
				out string[] instructions);

			foreach (var instruction in instructions)
			{
				var result = await stackMachine.ExecuteImmediatelyAsync(
					this,
					instruction,
					
					asyncContext);
					
				if (!result)
				{
					return;
				}
			}
		}

		#endregion

		#endregion

		#region IContainsContextHierarchy

		public IStanleyContextInternal ParentContext => parentContext;

		public bool TryGetChildContext(
			StanleyHandle handle,
			out IStanleyContextInternal context)
		{
			return childContexts.TryGet(
				handle.Value,
				out context);
		}

		public bool CreateChildContext(
			out StanleyHandle handle)
		{
			if (!contextIDAllocationController.AllocateID(
				out byte id))
			{
				handle = default;

				return false;
			}
			
			handle = StanleyFactory.BuildStanleyHandle(
				id);

			var context = StanleyFactory.BuildStanleyContext(
				this,
				emitShortcutInstructions,
				logAllExecutedCommands,
				loggerResolver);

			childContexts.Add(
				id,
				context);

			return true;
		}

		public bool DestroyChildContext(
			StanleyHandle handle)
		{
			if (!childContexts.TryRemove(
				handle.Value))
			{
				return false;
			}

			contextIDAllocationController.FreeID(
				handle.Value);

			return true;
		}

		public IEnumerable<IStanleyContextInternal> ChildContexts => childContexts.Values;

		#endregion

		public bool EmitShortcutInstructions => emitShortcutInstructions;

		public bool LogAllExecutedCommands => logAllExecutedCommands;
		
		public void Clear()
		{
			Stop();

			//TODO: add more functionality
		}

		#endregion
	}
}
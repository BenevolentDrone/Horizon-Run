using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;	//TODO: GET RID OF LINQ
using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Persistence;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;

namespace HereticalSolutions.StanleyScript
{
	public static class StanleyFactory
	{
		public static ObjectSymbolTable BuildObjectSymbolTable()
		{
			IRepository<string, IStanleyVariable> properties =
				RepositoryFactory.BuildDictionaryRepository<string, IStanleyVariable>();

			return new ObjectSymbolTable(
				properties);
		}

		public static ScopeSymbolTable BuildScopeSymbolTable(
			StanleyScope parentScope,
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<ScopeSymbolTable>();

			IRepository<string, IStanleyVariable> variables =
				RepositoryFactory.BuildDictionaryRepository<string, IStanleyVariable>();

			return new ScopeSymbolTable(
				parentScope,
				variables,
				loggerResolver,
				logger);
		}

		public static StanleyObject BuildStanleyObject()
		{
			return new StanleyObject(
				BuildObjectSymbolTable());
		}

		public static StanleyObject BuildStanleyObject(
			ISymbolTable properties)
		{
			return new StanleyObject(
				properties);
		}
		
		public static StanleyScopeObject BuildStanleyScopeObject()
		{
			return new StanleyScopeObject(
				BuildObjectSymbolTable());
		}

		public static StanleyScopeObject BuildStanleyScopeObject(
			ISymbolTable properties)
		{
			return new StanleyScopeObject(
				properties);
		}
		
		public static StanleyProgramObject BuildStanleyProgramObject(
			IStanleyContextInternal context,
			ILoggerResolver loggerResolver)
		{
			var result = new StanleyProgramObject(
				BuildObjectSymbolTable());
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
					typeof(StanleyHandle),
					BuildStanleyHandle(0),
					loggerResolver));
			
			Action startProgramDelegate = () =>
				StanleyInlineDelegates.StartProgram(
					context,
					result);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
				BuildStanleyDelegate(
						startProgramDelegate.Target,
						startProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Action pauseProgramDelegate = () =>
				StanleyInlineDelegates.PauseProgram(
					context,
					result);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						pauseProgramDelegate.Target,
						pauseProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Action resumeProgramDelegate = () =>
				StanleyInlineDelegates.ResumeProgram(
					context,
					result);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						resumeProgramDelegate.Target,
						resumeProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Action stopProgramDelegate = () =>
				StanleyInlineDelegates.StopProgram(
					context,
					result);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						stopProgramDelegate.Target,
						stopProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Action stepProgramDelegate = () =>
				StanleyInlineDelegates.StepProgram(
					context,
					result);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						stepProgramDelegate.Target,
						stepProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Action discardProgramDelegate = () =>
				StanleyInlineDelegates.DiscardProgram(
					context,
					result);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						discardProgramDelegate.Target,
						discardProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			return result;
		}
		
		public static StanleyProgramObject BuildStanleyProgramObject(
			IStanleyContextInternal context,
			ILoggerResolver loggerResolver,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			var result = new StanleyProgramObject(
				BuildObjectSymbolTable());
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
					typeof(StanleyHandle),
					BuildStanleyHandle(0),
					loggerResolver));
			
			Func<Task> startProgramDelegate = () =>
				StanleyInlineDelegates.StartProgramAsync(
					context,
					result,
					
					asyncContext);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
				BuildStanleyDelegate(
						startProgramDelegate.Target,
						startProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Func<Task> pauseProgramDelegate = () =>
				StanleyInlineDelegates.PauseProgramAsync(
					context,
					result,
					
					asyncContext);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						pauseProgramDelegate.Target,
						pauseProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Func<Task> resumeProgramDelegate = () =>
				StanleyInlineDelegates.ResumeProgramAsync(
					context,
					result,
					
					asyncContext);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						resumeProgramDelegate.Target,
						resumeProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Func<Task> stopProgramDelegate = () =>
				StanleyInlineDelegates.StopProgramAsync(
					context,
					result,
					
					asyncContext);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						stopProgramDelegate.Target,
						stopProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Func<Task> stepProgramDelegate = () =>
				StanleyInlineDelegates.StepProgramAsync(
					context,
					result,
					
					asyncContext);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						stepProgramDelegate.Target,
						stepProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			Func<Task> discardProgramDelegate = () =>
				StanleyInlineDelegates.DiscardProgramAsync(
					context,
					result,
					
					asyncContext);
			
			result.Properties.TryAddVariable(
				BuildValueVariable(
					StanleyConsts.PROGRAM_STARTED_VARIABLE_NAME,
					typeof(StanleyDelegate),
					BuildStanleyDelegate(
						discardProgramDelegate.Target,
						discardProgramDelegate.Method,
						false), //NOT awaiting
					loggerResolver));
			
			return result;
		}

		public static StanleyProgramObject BuildStanleyProgramObject(
			ISymbolTable properties)
		{
			return new StanleyProgramObject(
				properties);
		}

		public static StanleyDelegate BuildStanleyDelegate(
			object target,
			MethodInfo methodInfo,
			bool awaitCompletion)
		{
			return new StanleyDelegate(
				target,
				methodInfo,
				awaitCompletion);
		}
		
		public static StanleyDelegate BuildStanleyDelegate(
			IStanleyContextInternal context,
			Action<InstructionBuilder> emitterDelegate)
		{
			InstructionBuilder instructionBuilder = new InstructionBuilder(
				new StringBuilder());
						
			emitterDelegate?.Invoke(
				instructionBuilder);
			
			//StanleyInstructionEmitter.EmitDelegateJump(
			//	instructionBuilder,
			//	label);

			string[] instructions = instructionBuilder
				.Instructions;

			Action @delegate = () =>
			{
				StanleyInlineDelegates.ExecuteInline(
					context,
					instructions);
			};

			return BuildStanleyDelegate(
				@delegate.Target,
				@delegate.Method,
				false);
		}
		
		public static StanleyDelegate BuildStanleyDelegate(
			IStanleyContextInternal context,
			Action<InstructionBuilder> emitterDelegate,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			InstructionBuilder instructionBuilder = new InstructionBuilder(
				new StringBuilder());
			
			emitterDelegate?.Invoke(
				instructionBuilder);
			
			//StanleyInstructionEmitter.EmitDelegateJump(
			//	instructionBuilder,
			//	label);

			string[] instructions = instructionBuilder
				.Instructions;

			Func<Task> @delegate = () => StanleyInlineDelegates.ExecuteInlineAsync(
				context,
				instructions,
				asyncContext);
			
			return BuildStanleyDelegate(
				@delegate.Target,
				@delegate.Method,
				true);
		}

		public static StanleyEvent BuildStanleyEvent(
			Func<bool> poller,
			bool jumpToLabel,
			string label)
		{
			return new StanleyEvent(
				poller,
				jumpToLabel,
				label);
		}

		public static StanleyHandle BuildStanleyHandle(
			int value)
		{
			if (value < 0 || value > 255)
				throw new ArgumentOutOfRangeException(
					nameof(value),
					"Stanley handles must be between 0 and 255");

			return new StanleyHandle(
				(byte)value);
		}

		public static StanleyList BuildStanleyList()
		{
			return new StanleyList(
				new List<IStanleyVariable>());
		}

		public static StanleyScope BuildScope(
			StanleyHandle handle,
			StanleyScope parentScope,
			ILoggerResolver loggerResolver)
		{
			bool isGlobalScope = parentScope == null;

			var result = new StanleyScope(
				handle,
				parentScope,
				BuildScopeSymbolTable(
					parentScope,
					loggerResolver));

			IVariableImporter resultAsImporter = result.Variables as IVariableImporter;

			//Every scope should have an object "SCOPE"
			var scopeObject = StanleyFactory.BuildValueVariable(
				StanleyConsts.SCOPE_OBJECT_NAME,
				typeof(StanleyScopeObject),
				StanleyFactory.BuildStanleyScopeObject(),
				loggerResolver);

			resultAsImporter.ImportVariable(
				string.Empty,
				scopeObject);
			
			//resultAsImporter.ImportScopeObject(
			//	StanleyConsts.SCOPE_OBJECT_NAME,
			//	out _);

			//That object should have a reference to its own handle, "SCOPE.HANDLE"
			resultAsImporter.ImportVariable(
				StanleyConsts.SCOPE_OBJECT_NAME,
				BuildValueVariable(
					StanleyConsts.SCOPE_HANDLE_VARIABLE_NAME,
					typeof(StanleyHandle),
					result.Handle,
					loggerResolver));

			//That object should have a pc value to set to after completion, "SCOPE.RETURN_PC"
			resultAsImporter.ImportVariable(
				StanleyConsts.SCOPE_OBJECT_NAME,
				BuildValueVariable(
					StanleyConsts.SCOPE_RETURN_PC_VARIABLE_NAME,
					typeof(int),
					0,
					loggerResolver));

			//That object should have a reference to the scope to return to after completion, "SCOPE.RETURN_SCOPE"
			resultAsImporter.ImportVariable(
				StanleyConsts.SCOPE_OBJECT_NAME,
				BuildValueVariable(
					StanleyConsts.SCOPE_RETURN_SCOPE_VARIABLE_NAME,
					typeof(StanleyHandle),
					BuildStanleyHandle(0),
					loggerResolver));

			//That object should have a reference to the list of events, "SCOPE.EVENT_LIST"
			resultAsImporter.ImportVariable(
				StanleyConsts.SCOPE_OBJECT_NAME,
				BuildValueVariable(
					StanleyConsts.SCOPE_EVENT_LIST_VARIABLE_NAME,
					typeof(StanleyList),
					BuildStanleyList(),
					loggerResolver));

			//The idea that the global scope should be the only one to have the jump table looked natural
			//Util (thanks to Sonnet) it was decided that the program would start with pushing its own scope
			//and popping it upon completion
			//That was a good idea as subprograms share the same global scope as the root program and ensured
			//that their programs would not conflict in the jump table with the root program
			//Buuut that means that each scope should have a jump table now. Even the method scopes
			//Now only the scopes that are child to global scope should have a jump table
			bool isChildToGlobalScope =
				!isGlobalScope
				&& parentScope.ParentScope == null;
			
			if (isChildToGlobalScope)
			{
				//That object should have a jump table, "SCOPE.JUMP_TABLE"
				resultAsImporter.ImportVariable(
					StanleyConsts.SCOPE_OBJECT_NAME,
					BuildValueVariable(
						StanleyConsts.SCOPE_JUMP_TABLE_VARIABLE_NAME,
						typeof(StanleyObject),
						BuildStanleyObject(),
						loggerResolver));
			}

			return result;
		}

		public static StanleyValueVariable BuildValueVariable(
			string name,
			Type variableType,
			object value,
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<StanleyValueVariable>();

			return new StanleyValueVariable(
				name,
				variableType,
				value,
				loggerResolver,
				logger);
		}

		public static StanleyPollableVariable BuildPollableVariable(
			string name,
			Type variableType,
			Func<object> getter,
			Action<object> setter,
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<StanleyPollableVariable>();

			return new StanleyPollableVariable(
				name,
				variableType,
				getter,
				setter,
				loggerResolver,
				logger);
		}

		public static object GetDefaultValue(
			Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}

			return null;
		}

		public static StanleyCompiler BuildStanleyCompiler()
		{
			return new StanleyCompiler(
				BuildStanleyASTWalker());
		}

		public static StanleyASTWalker BuildStanleyASTWalker()
		{
			return new StanleyASTWalker();
		}

		public static ReportMaker BuildReportMaker()
		{
			return new ReportMaker();
		}

		public static void BuildReportMakerLogger(
			IStanleyContext context,
			string reportFileName,
			out ILogger reportLogger,
			out ISerializer serializer)
		{
			ILoggerBuilder loggerBuilder = LoggerFactory.BuildLoggerBuilder();

			var loggerResolver = loggerBuilder
				.NewLogger()
				.ToggleAllowedByDefault(
					true)
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithSourceTypePrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithLogTypePrefix())
				.AddWrapperBelow(
					new LoggerWrapperWithContext(
						context))
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithThreadIndexPrefix())
				.AddWrapperBelow(
					LoggerFactory.BuildLoggerWrapperWithTimestampPrefix(false))
				.Build();

			var pathSettings = new FileAtApplicationDataPathSettings()
			{
				RelativePath = $"../Reports/{reportFileName}.log"
			};

			int postfixIndex = 0;
			
			while (IOHelpers.FileExists(
				pathSettings.FullPath,
				null))
			{
				pathSettings = new FileAtApplicationDataPathSettings()
				{
					RelativePath = $"../Reports/{reportFileName}_{postfixIndex}.log"
				};

				postfixIndex++;
			}
			
			var fileSink = LoggerFactory.BuildFileSink(
				pathSettings,
				loggerResolver);

			serializer = fileSink.Serializer;

			loggerBuilder.AddSink(
				fileSink);

			reportLogger = loggerResolver.GetLogger<ReportMaker>();
		}

		public static StackMachine BuildStackMachine(
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<StackMachine>();

			IRepository<string, List<IStanleyInstructionHandler>> handlersChainOfResponsibility =
				RepositoryFactory.BuildDictionaryRepository<string, List<IStanleyInstructionHandler>>();

			IIDAllocationController<byte> scopeIDAllocationController =
				IDAllocationFactory.BuildByteIDAllocationController(
					loggerResolver);

			scopeIDAllocationController.AllocateID(
				out var globalScopeID);

			StanleyScope globalScope = BuildScope(
				BuildStanleyHandle(
					globalScopeID),
				null,
				loggerResolver);

			IRepository<byte, StanleyScope> scopes =
				RepositoryFactory.BuildDictionaryRepository<byte, StanleyScope>();

			scopes.TryAdd(
				globalScopeID,
				globalScope);
			
			List<ITypecaster> typecasters = new List<ITypecaster>();
			
			typecasters.Add(
				new DefaultTypecasterToInt());
			
			typecasters.Add(
				new DefaultTypecasterToFloat());
			
			typecasters.Add(
				new DefaultTypecasterToBool());
			
			typecasters.Add(
				new DefaultTypecasterToString());

			return new StackMachine(
				BuildStanleyList(),
				handlersChainOfResponsibility,
				scopeIDAllocationController,
				globalScope,
				scopes,
				typecasters,
	
				loggerResolver,
				logger);
		}

		public static StackMachine BuildStackMachine(
			IStanleyContext parentContext,
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<StackMachine>();

			IRepository<string, List<IStanleyInstructionHandler>> handlersChainOfResponsibility =
				RepositoryFactory.BuildDictionaryRepository<string, List<IStanleyInstructionHandler>>();

			IIDAllocationController<byte> scopeIDAllocationController =
				IDAllocationFactory.BuildByteIDAllocationController(
					loggerResolver);

			scopeIDAllocationController.AllocateID(
				out var globalScopeID);

			//First of all, I think that they should SHARE the global scope as each program
			//is working in its own child context
			//Second, StanleyScope has no IClonable implementation and I'm too lazy to implement it
			var globalScope = parentContext.StackMachine.GlobalScope;
			
			//var sourceGlobalScopeAsClonable = parentContext.StackMachine.GlobalScope as IClonable;
			//
			//StanleyScope globalScope = (StanleyScope)sourceGlobalScopeAsClonable.Clone();

			IRepository<byte, StanleyScope> scopes =
				RepositoryFactory.BuildDictionaryRepository<byte, StanleyScope>();

			scopes.TryAdd(
				globalScopeID,
				globalScope);
			
			List<ITypecaster> typecasters = new List<ITypecaster>();
			
			typecasters.Add(
				new DefaultTypecasterToInt());
			
			typecasters.Add(
				new DefaultTypecasterToFloat());
			
			typecasters.Add(
				new DefaultTypecasterToBool());
			
			typecasters.Add(
				new DefaultTypecasterToString());

			var result = new StackMachine(
				BuildStanleyList(),
				handlersChainOfResponsibility,
				scopeIDAllocationController,
				globalScope,
				scopes,
				typecasters,

				loggerResolver,
				logger);

			foreach (var handler in parentContext.StackMachine.AllInstructionHandlers)
			{
				result.AddHandler(
					handler);
			}

			return result;
		}

		public static StanleyContext BuildStanleyContext(
			StanleyContext parentContext,
			bool emitShortcutInstructions,
			bool logAllExecutedCommands,
			ILoggerResolver loggerResolver)
		{
			ILogger logger = loggerResolver?.GetLogger<StanleyContext>();

			bool isRootContext = parentContext == null;

			var result = new StanleyContext(
				(isRootContext)
					? BuildStackMachine(
						loggerResolver)
					: BuildStackMachine(
						parentContext,
						loggerResolver),
				BuildStanleyCompiler(),
				BuildReportMaker(),
				parentContext,
				RepositoryFactory.BuildDictionaryRepository<byte, IStanleyContextInternal>(),
				IDAllocationFactory.BuildByteIDAllocationController(
					loggerResolver),
				RepositoryFactory.BuildDictionaryRepository<string, string[]>(),
				emitShortcutInstructions,
				logAllExecutedCommands,
				loggerResolver,
				logger);

			if (!isRootContext)
			{
				foreach (var programName in parentContext.AllProgramNames)
				{
					parentContext.GetProgramFromLibrary(
						programName,
						out var program);

					result.SaveProgramToLibrary(
						programName,
						program);
				}
			}

			//if (isRootContext)
			{
				AddDefaultInstructionHandlers(result);
			}

			return result;
		}

		private static void AddDefaultInstructionHandlers(
			StanleyContext context)
		{
			IEnumerable<IStanleyInstructionHandler> handlers =
				typeof(IStanleyInstructionHandler)
					.Assembly
					.GetTypes()
					.Where(t =>
						t.IsSubclassOf(typeof(AStanleyInstructionHandler))
						&& !t.IsAbstract)
					.Select(type =>
						Activator.CreateInstance(
								type,
								new object[]
								{
									null,
									null
								})
							as IStanleyInstructionHandler);

			foreach (IStanleyInstructionHandler handler in handlers)
			{
				context.StackMachine.AddHandler(
					handler);
			}
		}
	}
}
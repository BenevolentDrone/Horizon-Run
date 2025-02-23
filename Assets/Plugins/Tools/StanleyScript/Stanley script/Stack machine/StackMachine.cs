using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Text;

using System.Linq; //TODO: remove linq

using System.Text.RegularExpressions;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;

using HereticalSolutions.Repositories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class StackMachine
		: IStackMachine
	{
		//Forgive me father for I am about to commit heresy and use a LIST as a STACK in a STACK MACHINE
		private readonly StanleyList stack;
		//private readonly Stack<IStanleyVariable> stack;


		private readonly IRepository<string, List<IStanleyInstructionHandler>> handlersChainOfResponsibility;


		private readonly IIDAllocationController<byte> scopeIDAllocationController;

		private readonly StanleyScope globalScope;

		private readonly IRepository<byte, StanleyScope> scopes;

		private readonly List<ITypecaster> typecasters;

		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;


		private int programCounter = -1;

		private int lineCounter = -1;


		private StanleyScope currentScope;


		private string[] instructionsListing;

		public StackMachine(
			StanleyList stack,
			IRepository<string, List<IStanleyInstructionHandler>> handlersChainOfResponsibility,
			IIDAllocationController<byte> scopeIDAllocationController,
			StanleyScope globalScope,
			IRepository<byte, StanleyScope> scopes,
			List<ITypecaster> typecasters,

			ILoggerResolver loggerResolver,
			ILogger logger)
		{
			this.stack = stack;

			this.handlersChainOfResponsibility = handlersChainOfResponsibility;

			this.scopeIDAllocationController = scopeIDAllocationController;

			this.globalScope = globalScope;

			this.scopes = scopes;

			this.typecasters = typecasters;


			this.loggerResolver = loggerResolver;

			this.logger = logger;


			currentScope = globalScope;

			programCounter = -1;

			lineCounter = -1;

			instructionsListing = Array.Empty<string>();
		}

		#region IStackMachine

		public int StackSize => stack.List.Count;

		public int ProgramCounter => programCounter;

		#region Stack operations

		public void Push(
			IStanleyVariable variable)
		{
			stack.Push(
				variable);
		}

		public bool Pop(
			out IStanleyVariable variable)
		{
			if (stack.Count == 0)
			{
				variable = null;

				return false;
			}

			variable = stack.Pop();

			return true;
		}

		public bool Peek(
			out IStanleyVariable variable)
		{
			if (stack.Count == 0)
			{
				variable = null;

				return false;
			}

			variable = stack.Peek();

			return true;
		}

		public bool PushToTop(
			int offset,
			IStanleyVariable variable)
		{
			if (offset == 0)
			{
				stack.Push(
					variable);

				return true;
			}

			if (stack.Count >= offset)
			{
				stack.InsertAt(
					stack.Count - offset,
					variable);

				return true;
			}

			return false;
		}

		public bool PopFromTop(
			int offset,
			out IStanleyVariable variable)
		{
			if (stack.Count == 0)
			{
				variable = null;

				return false;
			}

			if (stack.Count > offset)
			{
				variable = stack[stack.Count - offset - 1];

				stack.RemoveAt(
					stack.Count - offset - 1);

				return true;
			}

			variable = null;

			return false;
		}

		public bool PeekFromTop(
			int offset,
			out IStanleyVariable variable)
		{
			if (stack.Count == 0)
			{
				variable = null;

				return false;
			}

			if (stack.Count > offset)
			{
				variable = stack[stack.Count - offset - 1];

				return true;
			}

			variable = null;

			return false;
		}


		public bool PushToBottom(
			int offset,
			IStanleyVariable variable)
		{
			if (offset == stack.Count)
			{
				stack.Push(
					variable);

				return true;
			}

			if (stack.Count > offset)
			{
				stack.InsertAt(
					offset,
					variable);

				return true;
			}

			return false;
		}

		public bool PopFromBottom(
			int offset,
			out IStanleyVariable variable)
		{
			if (stack.Count == 0)
			{
				variable = null;

				return false;
			}

			if (stack.Count > offset)
			{
				variable = stack[offset];

				stack.RemoveAt(
					offset);

				return true;
			}

			variable = null;

			return false;
		}

		public bool PeekFromBottom(
			int offset,
			out IStanleyVariable variable)
		{
			if (stack.Count == 0)
			{
				variable = null;

				return false;
			}

			if (stack.Count > offset)
			{
				variable = stack[offset];

				return true;
			}

			variable = null;

			return false;
		}

		#endregion

		#region IContainsScopes

		public IIDAllocationController<byte> ScopeIDAllocationController => scopeIDAllocationController;

		public StanleyScope GlobalScope => globalScope;

		public StanleyScope CurrentScope => currentScope;

		public bool TryGetScope(
			StanleyHandle handle,
			out StanleyScope scope)
		{
			return scopes.TryGet(
				handle.Value,
				out scope);
		}

		public bool PushScope(
			out StanleyHandle handle)
		{
			if (!scopeIDAllocationController.AllocateID(
				out byte id))
			{
				handle = default;

				return false;
			}

			handle = StanleyFactory.BuildStanleyHandle(
				id);

			var scope = StanleyFactory.BuildScope(
				handle,
				currentScope,
				loggerResolver);

			scopes.TryAdd(
				id,
				scope);

			currentScope = scope;

			return true;
		}

		public bool PopScope()
		{
			if (currentScope == globalScope)
				return false;

			var previousScope = currentScope;

			if (!RemoveScopeRecursively(
				currentScope.Handle))
			{
				return false;
			}

			currentScope = previousScope.ParentScope;

			return true;
		}

		public bool SwitchScope(
			StanleyHandle handle)
		{
			if (scopes.TryGet(
				handle.Value,
				out var scope))
			{
				currentScope = scope;

				return true;
			}

			return false;
		}

		public bool RemoveScope(
			StanleyHandle handle)
		{
			if (currentScope.Handle == handle)
			{
				return PopScope();
			}

			return RemoveScopeRecursively(
				handle);
		}

		#endregion

		#region IContainsInterpreter

		public string[] InstructionsListing => instructionsListing;

		public int LineCounter => lineCounter;

		public void LoadInstructions(
			string[] instructions)
		{
			instructionsListing = instructions;

			lineCounter = -1;

			programCounter = 0;
		}

		public bool ExecuteNext(
			IStanleyContextInternal context)
		{
			if (programCounter < 0
				|| programCounter >= instructionsListing.Length)
			{
				return false;
			}

			var nextInstruction = instructionsListing[programCounter];

			if (!ExecuteImmediately(
				context,
				nextInstruction))
			{
				return false;
			}

			programCounter++;

			return true;
		}

		public async Task<bool> ExecuteNextAsync(
			IStanleyContextInternal context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (programCounter < 0
				|| programCounter >= instructionsListing.Length)
			{
				return false;
			}

			var nextInstruction = instructionsListing[programCounter];

			bool result = await ExecuteImmediatelyAsync(
				context,
				nextInstruction,

				asyncContext);

			if (!result)
			{
				return false;
			}

			programCounter++;

			return true;
		}

		public bool ExecuteImmediately(
			IStanleyContextInternal context,
			string instructionLine)
		{
			bool result = false;

			ILogger currentLogger = context.ReportMaker.ReportLogger ?? logger;

			if (string.IsNullOrEmpty(instructionLine))
			{
				return true;
			}

			if (context.LogAllExecutedCommands)
				currentLogger?.Log($"{instructionLine}");
			
			InterpretInstruction(
				instructionLine,
				out string instruction,
				out string[] instructionTokens);
			
			if (string.IsNullOrEmpty(instruction)
				|| instruction == StanleyConsts.LABEL
				|| instruction.StartsWith(
					StanleyConsts.LINE_COMMENT_PREFIX,
					StringComparison.Ordinal))
			{
				return true;
			}

			if (!handlersChainOfResponsibility.Has(instruction))
			{
				currentLogger?.LogError(
					GetType(),
					$"UNKNOWN INSTRUCTION {instruction}");

				return false;
			}

			bool handled = false;

			foreach (IStanleyInstructionHandler handler in handlersChainOfResponsibility.Get(instruction))
			{
				if (handler.WillHandle(
					context,
					instruction,
					instructionTokens))
				{
					try
					{
						result = handler
							.Handle(
								context,
								instruction,
								instructionTokens);
					}
					catch (Exception e)
					{
						currentLogger?.LogError(
							GetType(),
							$"ERROR WHILE HANDLING INSTRUCTION {instruction}: {e.Message}");

						return false;
					}

					handled = true;

					break;
				}
			}

			if (!handled)
			{
				currentLogger?.LogError(
					GetType(),
					$"INSTRUCTION NOT HANDLED: {instruction}");

				currentLogger?.LogError(
					GetType(),
					"PROGRAM WAS FINISHED UNSUCCESSFULLY");

				return false;
			}

			return result;
		}

		public async Task<bool> ExecuteImmediatelyAsync(
			IStanleyContextInternal context,
			string instructionLine,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			bool result = false;

			ILogger currentLogger = context.ReportMaker.ReportLogger ?? logger;

			if (string.IsNullOrEmpty(instructionLine))
			{
				return true;
			}

			if (context.LogAllExecutedCommands)
				currentLogger?.Log($"{instructionLine}");
			
			InterpretInstruction(
				instructionLine,
				out string instruction,
				out string[] instructionTokens);

			if (string.IsNullOrEmpty(instruction)
				|| instruction == StanleyConsts.LABEL
				|| instruction.StartsWith(
					StanleyConsts.LINE_COMMENT_PREFIX,
					StringComparison.Ordinal))
			{
				return true;
			}

			if (!handlersChainOfResponsibility.Has(instruction))
			{
				currentLogger?.LogError(
					GetType(),
					$"UNKNOWN INSTRUCTION {instruction}");

				return false;
			}

			
			bool handled = false;

			foreach (IStanleyInstructionHandler handler in handlersChainOfResponsibility.Get(instruction))
			{
				if (handler.WillHandle(
					context,
					instruction,
					instructionTokens))
				{
					result = await handler
						.Handle(
							context,
							instruction,
							instructionTokens,

							asyncContext)
						.ThrowExceptionsIfAny<bool>(
							GetType(),
							currentLogger);

					handled = true;

					break;
				}
			}

			if (!handled)
			{
				currentLogger?.LogError(
					GetType(),
					$"INSTRUCTION NOT HANDLED: {instruction}");

				currentLogger?.LogError(
					GetType(),
					"PROGRAM WAS FINISHED UNSUCCESSFULLY");

				return false;
			}

			return result;
		}

		#endregion

		#region IContainsInstructionHandlers

		public IEnumerable<string> AllInstructions => handlersChainOfResponsibility.Keys;

		public IEnumerable<IStanleyInstructionHandler> AllInstructionHandlers
		{
			get
			{
				foreach (var handlers in handlersChainOfResponsibility.Values)
				{
					foreach (var handler in handlers)
					{
						yield return handler;
					}
				}
			}
		}

		public bool GetHandlers(
			string instructionOrAlias,
			out IEnumerable<IStanleyInstructionHandler> handlers)
		{
			bool reult = handlersChainOfResponsibility.TryGet(
				instructionOrAlias,
				out var operationsList);

			handlers = operationsList;

			return reult;
		}

		public bool AddHandler(
			IStanleyInstructionHandler handler)
		{
			string instruction = handler.Instruction;

			AddInstructionHandler(
				instruction,
				handler);

			if (handler.Aliases != null)
			{
				foreach (string alias in handler.Aliases)
				{
					if (!string.IsNullOrEmpty(alias))
					{
						AddInstructionHandler(
							alias,
							handler);
					}
				}
			}

			return true;
		}

		public bool RemoveHandler(
			IStanleyInstructionHandler handler)
		{
			string instruction = handler.Instruction;

			if (!handlersChainOfResponsibility.Has(instruction))
			{
				return false;
			}

			RemoveInstructionHandlerWithCleanup(
				instruction,
				handler);

			if (handler.Aliases != null)
			{
				foreach (string alias in handler.Aliases)
				{
					if (!string.IsNullOrEmpty(alias))
					{
						RemoveInstructionHandlerWithCleanup(
							alias,
							handler);
					}
				}
			}

			return true;
		}

		#endregion

		#region IContainsTypecasters

		public bool HasTypecaster(
			ITypecaster typecaster)
		{
			return typecasters.Contains(typecaster);
		}

		public bool TryRegisterTypecaster(
			ITypecaster typecaster)
		{
			if (HasTypecaster(typecaster))
				return false;
			
			typecasters.Add(typecaster);
			
			return true;
		}

		public bool TryRemoveTypecaster(
			ITypecaster typecaster)
		{
			if (!HasTypecaster(typecaster))
				return false;
			
			typecasters.Remove(typecaster);
			
			return true;
		}

		public bool CanCast<TSource, TValue>()
		{
			foreach (var typecaster in typecasters)
			{
				if (typecaster.TargetType != typeof(TValue))
					continue;
				
				if (typecaster.WillCast<TSource>())
					return true;
			}

			return false;
		}

		public bool CanCast(
			Type sourceType,
			Type targetType)
		{
			foreach (var typecaster in typecasters)
			{
				if (typecaster.TargetType != targetType)
					continue;
				
				if (typecaster.WillCast(sourceType))
					return true;
			}

			return false;
		}

		public bool TryCast<TSource, TValue>(
			TSource source,
			out TValue result)
		{
			foreach (var typecaster in typecasters)
			{
				if (typecaster.TargetType != typeof(TValue))
					continue;
				
				if (!typecaster.WillCast<TSource>())
					continue;

				if (typecaster.TryCast<TSource, TValue>(
					    source,
					    out result))
				{
					return true;
				}
			}

			result = default;

			return false;
		}

		public bool TryCast(
			object source,
			Type targetType,
			out object result)
		{
			foreach (var typecaster in typecasters)
			{
				if (typecaster.TargetType != targetType)
					continue;
				
				if (!typecaster.WillCast(source.GetType()))
					continue;

				if (typecaster.TryCast(
					source,
					out result))
				{
					return true;
				}
			}

			result = default;

			return false;
		}

		#endregion
		
		#region IContainsProgramControls

		public void SetCurrentProgramCounter(
			int line)
		{
			programCounter = line;
		}

		public void SetCurrentLineCounter(
			int line)
		{
			lineCounter = line;
		}

		public void PollEvents(
			IStanleyContextInternal context)
		{
			PollEventsOnScopeRecursively(
				context,
				globalScope);
		}

		public async Task PollEventsAsync(
			IStanleyContextInternal context,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			await PollEventsOnScopeRecursivelyAsync(
				context,
				globalScope,
				
				asyncContext);
		}

		#endregion

		public void Clear()
		{
			stack.Clear();

			programCounter = -1;

			lineCounter = -1;

			instructionsListing = Array.Empty<string>();
		}

		#endregion

		private void AddInstructionHandler(
			string instructionOrAlias,
			IStanleyInstructionHandler handler)
		{
			if (handlersChainOfResponsibility.Has(instructionOrAlias))
			{
				handlersChainOfResponsibility
					.Get(instructionOrAlias)
					.Insert(
						0,
						handler);
			}
			else
			{
				handlersChainOfResponsibility.Add(
					instructionOrAlias,
					new List<IStanleyInstructionHandler>
					{
						handler
					});
			}
		}

		private void RemoveInstructionHandlerWithCleanup(
			string instructionOrAlias,
			IStanleyInstructionHandler handler)
		{
			if (!handlersChainOfResponsibility.TryGet(
				instructionOrAlias,
				out var handlers))
			{
				return;
			}

			handlers.Remove(
				handler);

			if (handlers.Count == 0)
			{
				handlersChainOfResponsibility.Remove(
					instructionOrAlias);
			}
		}

		private bool RemoveScopeRecursively(
			StanleyHandle handle)
		{
			if (!scopes.TryGet(
				handle.Value,
				out var scope))
			{
				return false;
			}

			if (!scopes.TryRemove(
				handle.Value))
			{
				return false;
			}

			scopeIDAllocationController.FreeID(
				handle.Value);

			var handles = scopes.Keys.ToArray();

			foreach (byte id in handles)
			{
				if (scopes
					.TryGet(
						id,
						out var childCandidate)
					&& childCandidate.ParentScope == scope)
				{
					RemoveScopeRecursively(
						scopes[id].Handle);
				}
			}

			ClearScopeVariables(
				scope);

			return true;
		}

		private void ClearScopeVariables(
			StanleyScope stanleyScope)
		{
			var allVariables = stanleyScope.Variables.AllVariables.ToArray();

			foreach (IStanleyVariable variable in allVariables)
			{
				stanleyScope.Variables.TryRemoveVariable(
					variable.Name);
			}
		}

		private void InterpretInstruction(
			string instructionLine,
			out string instruction,
			out string[] instructionTokens)
		{
			instructionTokens = Regex.Split(
				instructionLine,
				StanleyConsts.REGEX_SPLIT_LINE_BY_WHITESPACE_UNLESS_WITHIN_QUOTES);

			for (int i = 0; i < instructionTokens.Length; i++)
			{
				instructionTokens[i] = instructionTokens[i].Trim('"');
			}

			instruction = instructionTokens[0];
		}

		private void PollEventsOnScopeRecursively(
			IStanleyContextInternal context,
			StanleyScope scope)
		{
			if (StanleyInlineDelegates.TryGetScopeEventListVariable(
				scope,
				out var eventListVariable))
			{
				var eventVariables = eventListVariable.GetValue<StanleyList>().List.ToArray();

				foreach (var eventVariable in eventVariables)
				{
					var stanleyEvent = eventVariable.GetValue<StanleyEvent>();

					stanleyEvent.Poll();
				}

				foreach (var eventVariable in eventVariables)
				{
					var stanleyEvent = eventVariable.GetValue<StanleyEvent>();

					if (stanleyEvent.Raised
						&& stanleyEvent.JumpToLabel)
					{
						stanleyEvent.Reset();

						InstructionBuilder instructionBuilder = new InstructionBuilder(
							new StringBuilder());
						
						StanleyInstructionEmitter.EmitEventJump(
							instructionBuilder,
							stanleyEvent,
							currentScope.Handle);

						string[] instructions = instructionBuilder
							.Instructions;

						foreach (string instruction in instructions)
						{
							if (!ExecuteImmediately(
								context,
								instruction))
							{
								return;
							}
						}
					}
				}
			}

			var handles = scopes.Keys.ToArray();

			foreach (byte id in handles)
			{
				if (scopes
					.TryGet(
						id,
						out var childCandidate)
					&& childCandidate.ParentScope == scope)
				{
					PollEventsOnScopeRecursively(
						context,
						scopes[id]);
				}
			}
		}

		private async Task PollEventsOnScopeRecursivelyAsync(
			IStanleyContextInternal context,
			StanleyScope scope,
			
			//Async tail
			AsyncExecutionContext asyncContext)
		{
			if (StanleyInlineDelegates.TryGetScopeEventListVariable(
				scope,
				out var eventListVariable))
			{
				var eventVariables = eventListVariable.GetValue<StanleyList>().List.ToArray();

				foreach (var eventVariable in eventVariables)
				{
					var stanleyEvent = eventVariable.GetValue<StanleyEvent>();

					stanleyEvent.Poll();
				}

				foreach (var eventVariable in eventVariables)
				{
					var stanleyEvent = eventVariable.GetValue<StanleyEvent>();

					if (stanleyEvent.Raised
						&& stanleyEvent.JumpToLabel)
					{
						stanleyEvent.Reset();

						InstructionBuilder instructionBuilder = new InstructionBuilder(
							new StringBuilder());
						
						StanleyInstructionEmitter.EmitEventJump(
							instructionBuilder,
							stanleyEvent,
							currentScope.Handle);

						string[] instructions = instructionBuilder
							.Instructions;

						foreach (string instruction in instructions)
						{
							bool result = await ExecuteImmediatelyAsync(
								context,
								instruction,
								
								asyncContext);

							if (!result)
							{
								return;
							}
						}
					}
				}
			}

			var handles = scopes.Keys.ToArray();

			foreach (byte id in handles)
			{
				if (scopes
					.TryGet(
						id,
						out var childCandidate)
					&& childCandidate.ParentScope == scope)
				{
					await PollEventsOnScopeRecursivelyAsync(
						context,
						scopes[id],
						
						asyncContext);
				}
			}
		}
	}
}
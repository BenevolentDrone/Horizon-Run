using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.StanleyScript
{
    public static class StanleyInlineDelegates
    {
        #region Execute
        
        public static void ExecuteInline(
            IStanleyContextInternal context,
            string[] instructions)
        {
            var stackMachine = context.StackMachine;
            
            foreach (string instruction in instructions)
            {
                if (!stackMachine.ExecuteImmediately(
                    context,
                    instruction))
                {
                    return;
                }
            }
        }
        
        public static async Task ExecuteInlineAsync(
            IStanleyContextInternal context,
            string[] instructions,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            var stackMachine = context.StackMachine;
            
            foreach (string instruction in instructions)
            {
                bool result = await stackMachine.ExecuteImmediatelyAsync(
                    context,
                    instruction,
								
                    asyncContext);

                if (!result)
                {
                    return;
                }
            }
        }
        
        #endregion
        
        #region Program

        public static async Task StartProgramAsync(
            IStanleyContextInternal context,
            StanleyProgramObject programObject,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            //NOT awaiting
            childContext.StartAsync(
                
                asyncContext);
        }

        public static void StartProgram(
            IStanleyContextInternal context,
            StanleyProgramObject programObject)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                handle,
                out var childContext))
            {
                return;
            }
            
            childContext.Start();
        }
        
        public static async Task PauseProgramAsync(
            IStanleyContextInternal context,
            StanleyProgramObject programObject,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Pause();
        }

        public static void PauseProgram(
            IStanleyContextInternal context,
            StanleyProgramObject programObject)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Pause();
        }
        
        public static async Task ResumeProgramAsync(
            IStanleyContextInternal context,
            StanleyProgramObject programObject,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Resume();
        }

        public static void ResumeProgram(
            IStanleyContextInternal context,
            StanleyProgramObject programObject)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Resume();
        }
        
        public static async Task StopProgramAsync(
            IStanleyContextInternal context,
            StanleyProgramObject programObject,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Stop();
        }

        public static void StopProgram(
            IStanleyContextInternal context,
            StanleyProgramObject programObject)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Stop();
        }
        
        public static async Task StepProgramAsync(
            IStanleyContextInternal context,
            StanleyProgramObject programObject,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            //NOT awaiting
            childContext.StepAsync(
                
                asyncContext);
        }

        public static void StepProgram(
            IStanleyContextInternal context,
            StanleyProgramObject programObject)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Step();
        }
        
        public static async Task DiscardProgramAsync(
            IStanleyContextInternal context,
            StanleyProgramObject programObject,
            
            //Async tail
            AsyncExecutionContext asyncContext)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Stop();

            context.DestroyChildContext(
                handle);

            programHandleVariable.SetValue<StanleyHandle>(
                StanleyFactory.BuildStanleyHandle(0));
        }

        public static void DiscardProgram(
            IStanleyContextInternal context,
            StanleyProgramObject programObject)
        {
            if (!programObject
                    .Properties
                    .TryGetVariable(
                        StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME,
                        out var programHandleVariable))
            {
                return;
            }

            var handle = programHandleVariable.GetValue<StanleyHandle>();

            if (!context.TryGetChildContext(
                    handle,
                    out var childContext))
            {
                return;
            }
            
            childContext.Stop();

            context.DestroyChildContext(
                handle);
            
            programHandleVariable.SetValue<StanleyHandle>(
                StanleyFactory.BuildStanleyHandle(0));
        }

        #endregion
        
        #region Get scope variables

        public static bool TryGetScopeHandleVariable(
            StanleyScope currentScope,
            out IStanleyVariable handleVariable)
        {
            if (!currentScope
                .Variables
                .TryGetVariable(
                    StanleyConsts.SCOPE_OBJECT_NAME,
                    out var scopeObjectVariable))
            {
                handleVariable = null;
                
                return false;
            }
            
            return scopeObjectVariable
                .GetValue<StanleyObject>()
                .Properties
                .TryGetVariable(
                    StanleyConsts.SCOPE_HANDLE_VARIABLE_NAME,
                    out handleVariable);
        }
        
        public static bool TryGetScopeReturnPCVariable(
            StanleyScope currentScope,
            out IStanleyVariable handleVariable)
        {
            if (!currentScope
                    .Variables
                    .TryGetVariable(
                        StanleyConsts.SCOPE_OBJECT_NAME,
                        out var scopeObjectVariable))
            {
                handleVariable = null;
                
                return false;
            }
            
            return scopeObjectVariable
                .GetValue<StanleyObject>()
                .Properties
                .TryGetVariable(
                    StanleyConsts.SCOPE_RETURN_PC_VARIABLE_NAME,
                    out handleVariable);
        }
        
        public static bool TryGetScopeReturnScopeVariable(
            StanleyScope currentScope,
            out IStanleyVariable handleVariable)
        {
            if (!currentScope
                    .Variables
                    .TryGetVariable(
                        StanleyConsts.SCOPE_OBJECT_NAME,
                        out var scopeObjectVariable))
            {
                handleVariable = null;
                
                return false;
            }
            
            return scopeObjectVariable
                .GetValue<StanleyObject>()
                .Properties
                .TryGetVariable(
                    StanleyConsts.SCOPE_RETURN_SCOPE_VARIABLE_NAME,
                    out handleVariable);
        }
        
        public static bool TryGetScopeEventListVariable(
            StanleyScope currentScope,
            out IStanleyVariable handleVariable)
        {
            if (!currentScope
                    .Variables
                    .TryGetVariable(
                        StanleyConsts.SCOPE_OBJECT_NAME,
                        out var scopeObjectVariable))
            {
                handleVariable = null;
                
                return false;
            }
            
            return scopeObjectVariable
                .GetValue<StanleyObject>()
                .Properties
                .TryGetVariable(
                    StanleyConsts.SCOPE_EVENT_LIST_VARIABLE_NAME,
                    out handleVariable);
        }

        public static bool TryGetJumpTableVariable(
            StanleyScope currentScope,
            out IStanleyVariable jumpTableVariable)
        {
            if (!currentScope
                .Variables
                .TryGetVariable(
                    StanleyConsts.SCOPE_OBJECT_NAME,
                    out var scopeObjectVariable))
            {
                jumpTableVariable = null;
                
                return false;
            }

            if (!scopeObjectVariable
                .GetValue<StanleyObject>()
                .Properties
                .TryGetVariable(
                    StanleyConsts.SCOPE_JUMP_TABLE_VARIABLE_NAME,
                    out jumpTableVariable))
            {
                if (currentScope.ParentScope == null)
                {
                    jumpTableVariable = null;
                    
                    return false;
                }
                
                return TryGetJumpTableVariable(
                    currentScope.ParentScope,
                    out jumpTableVariable);
            }
            
            return true;
        }
        
        #endregion
    }
}
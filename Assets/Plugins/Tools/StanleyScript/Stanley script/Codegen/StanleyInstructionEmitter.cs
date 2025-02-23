namespace HereticalSolutions.StanleyScript
{
	public static class StanleyInstructionEmitter
	{
		#region General emissions

		public static void Emit(
			IInstructionBuilder instructionBuilder,
			string opcode)
		{
			instructionBuilder.StringBuilder.AppendLine(opcode);
            
			instructionBuilder.InstructionsCount++;
		}
        
		public static void Emit(
			IInstructionBuilder instructionBuilder,
            string opcode,
            params string[] args)
        {
            string line = $"{opcode} {string.Join(" ", args)}";
            
            instructionBuilder.StringBuilder.AppendLine(line);
            
            instructionBuilder.InstructionsCount++;
        }
		
		public static void EmitComment(
			IInstructionBuilder instructionBuilder,
			string comment)
        {
	        instructionBuilder.StringBuilder.AppendLine($"// {comment}");
	        
	        instructionBuilder.InstructionsCount++;
        }

		public static void EmitEmptyLine(
			IInstructionBuilder instructionBuilder)
        {
            instructionBuilder.StringBuilder.AppendLine();
	        
            instructionBuilder.InstructionsCount++;
        }
		
		public static void EmitLineNumber(
			IInstructionBuilder instructionBuilder,
			int lineNumber,
			string line)
		{
			//EmitEmptyLine(instructionBuilder);
            
			EmitComment(instructionBuilder, $"Line {lineNumber}");
			
			if (!string.IsNullOrEmpty(line))
				EmitComment(instructionBuilder, line);

			EmitEmptyLine(instructionBuilder);
            
			EmitPushInt(instructionBuilder, lineNumber);
			Emit(instructionBuilder, StanleyOpcodes.OP_STACK_SET_LC);
            
			EmitEmptyLine(instructionBuilder);
		}

		#endregion

		#region Literal emissions
		
		public static void EmitPushInt(
			IInstructionBuilder instructionBuilder,
			int value)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_INT} {value}");
            
			instructionBuilder.InstructionsCount++;
		}
		
		public static void EmitPushInt(
			IInstructionBuilder instructionBuilder,
			string value)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_INT} {value}");
            
			instructionBuilder.InstructionsCount++;
		}
		
		public static void EmitPushFloat(
        	IInstructionBuilder instructionBuilder,
        	float value)
        {
        	instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_FLOAT} {value}");
	        
        	instructionBuilder.InstructionsCount++;
        }
		
		public static void EmitPushFloat(
			IInstructionBuilder instructionBuilder,
			string value)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_FLOAT} {value}");
	        
			instructionBuilder.InstructionsCount++;
		}
		
		public static void EmitPushBool(
			IInstructionBuilder instructionBuilder,
			bool value)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_BOOL} {value}");
	        
			instructionBuilder.InstructionsCount++;
		}
		
		public static void EmitPushBool(
			IInstructionBuilder instructionBuilder,
			string value)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_BOOL} {value}");
	        
			instructionBuilder.InstructionsCount++;
		}
		
		public static void EmitPushString(
			IInstructionBuilder instructionBuilder,
			string value)
        {
            instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{value}\"");
            
            instructionBuilder.InstructionsCount++;
        }
		
		#endregion

		#region Variables and Properties

        public static void EmitAllocVariable(
            IInstructionBuilder instructionBuilder,
            string variableName,
            string variableType)
        {
            // Push in reverse order since last push will be first argument
            EmitPushString(instructionBuilder, variableType);
            Emit(instructionBuilder, StanleyOpcodes.OP_PUSH_TYPE);
            EmitPushString(instructionBuilder, variableName);
            Emit(instructionBuilder, StanleyOpcodes.OP_ALLOC_VARIABLE);
        }

        public static void EmitAllocProperty(
            IInstructionBuilder instructionBuilder,
            string objectName,
            string propertyName,
            string propertyType)
        {
            // Push in reverse order since last push will be first argument
            EmitPushString(instructionBuilder, propertyType);
            Emit(instructionBuilder, StanleyOpcodes.OP_PUSH_TYPE);
            EmitPushString(instructionBuilder, propertyName);
            EmitPushVariable(instructionBuilder, objectName);
            Emit(instructionBuilder, StanleyOpcodes.OP_ALLOC_PROPERTY);
        }

        #endregion

		#region Variables

		public static void EmitPushVariable(
			IInstructionBuilder instructionBuilder,
			string variableName)
        {
            EmitPushString(instructionBuilder, variableName);
            
            Emit(instructionBuilder, StanleyOpcodes.OP_PUSH_VARIABLE);
        }
		
		#endregion

		#region Objects

		public static void EmitPushProperty(
			IInstructionBuilder instructionBuilder,
			string objectName,
			string propertyName)
        {
            EmitPushString(instructionBuilder, propertyName);
         
            EmitPushVariable(instructionBuilder, objectName);
            
            Emit(instructionBuilder, StanleyOpcodes.OP_PUSH_PROPERTY);
        }
		
		#endregion

		#region Labels

		public static void EmitLabel(
			IInstructionBuilder instructionBuilder,
			string label)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.LABEL} {label}");
	        
			instructionBuilder.InstructionsCount++;
		}

		public static void EmitLabelAddressPlaceholder(
			IInstructionBuilder instructionBuilder,
			string label)
		{
			// Push in reverse order since last push will be first argument
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_INT} <ADDR_{label}>");
			
			instructionBuilder.InstructionsCount++;
			
			EmitPushString(instructionBuilder, label);
			
			Emit(instructionBuilder, StanleyOpcodes.OP_ALLOC_LABEL);
		}

		#endregion
		
		/*
		//JUMP HEADER
		- The PC is pushed to stack (OP_STACK_PUSH_PC)
		- The scope handle is pushed to stack (SCOPE.HANDLE)

		//EVENT JUMP
		- The scope is switched (OP_SCOPE_SWITCH) to the handle stored in the SCOPE object (SCOPE.HANDLE) of the event variable's scope (provided by the method, as it's currently processing this scope)
		- The jump is performed (OP_JUMP_LABEL) to the label stored in the event variable (OP_EVENT_GET_LABEL)
		
		//METHOD HEADER
		- The new scope is pushed (OP_PUSH_SCOPE)
		- The scope handle from stack is stored inside the SCOPE object (SCOPE.RETURN_SCOPE)
		- The pc from stack is stored inside the SCOPE object (SCOPE.RETURN_PC)

		//METHOD FOOTER
		- The pc stored inside the SCOPE object (SCOPE.RETURN_PC) is pushed to stack
		- The scope handle stored inside the SCOPE object (SCOPE.RETURN_SCOPE) is pushed to stack
		- The current scope is popped (OP_POP_SCOPE)
		- The scope is switched to the scope handle from stack (OP_SCOPE_SWITCH)
		- The jump is performed to the pc from stack (OP_JUMP)
		*/

		#region Scope emissions

		public static void EmitGetScopeHandle(
			IInstructionBuilder instructionBuilder)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{StanleyConsts.SCOPE_HANDLE_VARIABLE_NAME}\"");
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{StanleyConsts.SCOPE_OBJECT_NAME}\"");
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_VARIABLE);
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_PROPERTY);
			
			instructionBuilder.InstructionsCount += 4;
		}

		public static void EmitGetScopeReturnPC(
			IInstructionBuilder instructionBuilder)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{StanleyConsts.SCOPE_RETURN_PC_VARIABLE_NAME}\"");
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{StanleyConsts.SCOPE_OBJECT_NAME}\"");
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_VARIABLE);
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_PROPERTY);
			
			instructionBuilder.InstructionsCount += 4;
		}

		public static void EmitGetScopeReturnScope(
			IInstructionBuilder instructionBuilder)
		{
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{StanleyConsts.SCOPE_RETURN_SCOPE_VARIABLE_NAME}\"");
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} \"{StanleyConsts.SCOPE_OBJECT_NAME}\"");
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_VARIABLE);
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_PROPERTY);
			
			instructionBuilder.InstructionsCount += 4;
		}
		
		#endregion

		#region Jump emissions

		public static void EmitJumpHeader(
			IInstructionBuilder instructionBuilder)
		{
			Emit(instructionBuilder, StanleyOpcodes.OP_STACK_PUSH_PC);
			
			EmitGetScopeHandle(instructionBuilder);
			Emit(instructionBuilder, StanleyOpcodes.OP_STACK_RVALUE);
		}

		public static void EmitEventJump(
			IInstructionBuilder instructionBuilder,
			StanleyEvent @event,
			StanleyHandle eventVariableScopeHandle)
		{
			EmitJumpHeader(instructionBuilder);
			
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_INT} {eventVariableScopeHandle.Value}");
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_PUSH_HANDLE);
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_STACK_RVALUE);
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_SCOPE_SWITCH);
			
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} {@event.Label}");
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_JUMP_LABEL);
			
			instructionBuilder.InstructionsCount += 6;
		}

		public static void EmitDelegateJump(
			IInstructionBuilder instructionBuilder,
			string label)
		{
			EmitJumpHeader(instructionBuilder);
			
			instructionBuilder.StringBuilder.AppendLine($"{StanleyOpcodes.OP_PUSH_STRING} {label}");
			instructionBuilder.StringBuilder.AppendLine(StanleyOpcodes.OP_JUMP_LABEL);
			
			instructionBuilder.InstructionsCount += 2;
		}
		
		#endregion

		#region Methods
		
		public static void EmitMethodHeader(
			IInstructionBuilder instructionBuilder)
		{
			EmitComment(instructionBuilder, $"<Header>");
			
			EmitEmptyLine(instructionBuilder);
			
			Emit(instructionBuilder, StanleyOpcodes.OP_PUSH_SCOPE);
			Emit(instructionBuilder, StanleyOpcodes.OP_STACK_POP);

			EmitPushProperty(instructionBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_RETURN_SCOPE_VARIABLE_NAME);
			Emit(instructionBuilder, StanleyOpcodes.OP_REVERSE_COPY);
			
			EmitPushProperty(instructionBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_RETURN_PC_VARIABLE_NAME);
			Emit(instructionBuilder, StanleyOpcodes.OP_REVERSE_COPY);

			EmitEmptyLine(instructionBuilder);
			
			EmitComment(instructionBuilder, $"</Header>");
			
			EmitEmptyLine(instructionBuilder);
		}

		public static void EmitMethodFooter(
			IInstructionBuilder instructionBuilder)
		{
			EmitComment(instructionBuilder, $"<Footer>");
			
			EmitEmptyLine(instructionBuilder);
			
			
			EmitPushProperty(instructionBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_RETURN_PC_VARIABLE_NAME);
			Emit(instructionBuilder, StanleyOpcodes.OP_STACK_RVALUE);
			
			
			EmitPushProperty(instructionBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_RETURN_SCOPE_VARIABLE_NAME);
			Emit(instructionBuilder, StanleyOpcodes.OP_STACK_RVALUE);
			
			
			Emit(instructionBuilder, StanleyOpcodes.OP_POP_SCOPE);
			
			
			Emit(instructionBuilder, StanleyOpcodes.OP_SCOPE_SWITCH);
			
			Emit(instructionBuilder, StanleyOpcodes.OP_JUMP);

			EmitEmptyLine(instructionBuilder);
			
			EmitComment(instructionBuilder, $"</Footer>");
			
			EmitEmptyLine(instructionBuilder);
		}

		#endregion
	}
}
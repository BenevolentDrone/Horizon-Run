using System;
using System.Text;
using System.Globalization;

using Antlr4.Runtime.Misc;

namespace HereticalSolutions.StanleyScript
{
    public class StanleyASTWalker
        : StanleyParserBaseVisitor<object>
    {
        private IInstructionBuilder headerBuilder;

        private IInstructionBuilder labelBuilder;
        
        private IInstructionBuilder mainBuilder;
        
        private IInstructionBuilder footerBuilder;
        
        private IInstructionBuilder methodBuilder;
        
        private IInstructionBuilder currentBuilder;

        //private int currentLine = -1;

        private int currentLabelCount = 0;

        private string[] programListing;
        
        public string[] ProgramListing
        {
            get => programListing;
            set => programListing = value;
        }

        public string[] Instructions
        {
            get
            {
                string[] headerInstructions = headerBuilder.Instructions;
                string[] labelInstructions = labelBuilder.Instructions;
                string[] mainInstructions = mainBuilder.Instructions;
                string[] footerInstructions = footerBuilder.Instructions;
                string[] methodInstructions = methodBuilder.Instructions;
                
                var result = new string[
                    headerInstructions.Length
                    + labelInstructions.Length
                    + mainInstructions.Length
                    + footerInstructions.Length
                    + methodInstructions.Length];
                
                Array.Copy(
                    headerInstructions,
                    result,
                    headerInstructions.Length);

                int offset = headerInstructions.Length;
                
                Array.Copy(
                    labelInstructions,
                    0,
                    result,
                    offset,
                    labelInstructions.Length);
                
                offset += labelInstructions.Length;
                
                Array.Copy(
                    mainInstructions,
                    0,
                    result,
                    offset,
                    mainInstructions.Length);
                
                offset += mainInstructions.Length;
                
                Array.Copy(
                    footerInstructions,
                    0,
                    result,
                    offset,
                    footerInstructions.Length);
                
                offset += footerInstructions.Length;
                
                Array.Copy(
                    methodInstructions,
                    0,
                    result,
                    offset,
                    methodInstructions.Length);
                
                return result;
            }
        }
        
        public override object VisitProgram([NotNull] StanleyParser.ProgramContext context)
        {
            //currentLine = -1;

            currentLabelCount = 0;
            
            headerBuilder = new InstructionBuilder(
                new StringBuilder());
            
            labelBuilder = new InstructionBuilder(
                new StringBuilder());
            
            mainBuilder = new InstructionBuilder(
                new StringBuilder());
            
            footerBuilder = new InstructionBuilder(
                new StringBuilder());
            
            methodBuilder = new InstructionBuilder(
                new StringBuilder());

            currentBuilder = mainBuilder;
            
            //Header
            StanleyInstructionEmitter.EmitComment(headerBuilder, "Program start");
            
            StanleyInstructionEmitter.Emit(headerBuilder, StanleyOpcodes.OP_PUSH_SCOPE);
            StanleyInstructionEmitter.Emit(headerBuilder, StanleyOpcodes.OP_STACK_POP);
            
            StanleyInstructionEmitter.EmitEmptyLine(headerBuilder);
            
            //Labels block start
            StanleyInstructionEmitter.EmitComment(labelBuilder, "<Labels allocation>");
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
            
            //Methods block start
            StanleyInstructionEmitter.EmitComment(methodBuilder, "Methods");
            StanleyInstructionEmitter.EmitEmptyLine(methodBuilder);
            
            base.VisitProgram(context);
            
            //Footer
            StanleyInstructionEmitter.EmitComment(footerBuilder, "Program end");
            StanleyInstructionEmitter.Emit(footerBuilder, StanleyOpcodes.OP_POP_SCOPE);
            StanleyInstructionEmitter.Emit(footerBuilder, StanleyOpcodes.OP_RETURN);
            
            StanleyInstructionEmitter.EmitEmptyLine(footerBuilder);
            
            //Labels block finish
            StanleyInstructionEmitter.EmitComment(labelBuilder, "</Labels allocation>");
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
            
            return null;
        }

        #region Statements
        
        public override object VisitStatement([NotNull] StanleyParser.StatementContext context)
        {
            // Track line number using ANTLR's line info (convert from 1-based to 0-based)
            var currentLine = context.Start.Line - 1;
            
            string text = context.GetText();
            
            bool isComment = context.COMMENT() != null;

            if (!isComment)
            {
                string lineText = text;

                if (programListing != null
                    && currentLine >= 0
                    && currentLine < programListing.Length)
                {
                    lineText = programListing[currentLine];
                }
                
                StanleyInstructionEmitter.EmitLineNumber(currentBuilder, currentLine + 1, lineText);

                base.VisitStatement(context);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
            }

            return null;
        }

        public override object VisitDeclarationStatement([NotNull] StanleyParser.DeclarationStatementContext context)
        {
            string typeName = MapType(context.typeName().GetText());
            var subjectReference = context.subjectReference();

            var identifiers = subjectReference.IDENTIFIER();
            
            if (subjectReference.POSSESSIVE() != null
                && subjectReference.POSSESSIVE().Length > 0)
            {
                string objectName = identifiers[0].GetText();
                string allocatedPropertyName = identifiers[identifiers.Length - 1].GetText();
                
                StanleyInstructionEmitter.EmitPushString(currentBuilder, typeName);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_TYPE);
                StanleyInstructionEmitter.EmitPushString(currentBuilder, allocatedPropertyName);
                
                for (int i = identifiers.Length - 2; i >= 1; i--)
                {
                    StanleyInstructionEmitter.EmitPushString(currentBuilder, identifiers[i].GetText());
                }
                
                StanleyInstructionEmitter.EmitPushVariable(currentBuilder, objectName);

                for (int i = identifiers.Length - 2; i >= 1; i--)
                {
                    StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_PROPERTY);
                }

                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_ALLOC_PROPERTY);
            }
            else
            {
                // This is a local variable
                string varName = identifiers[0].GetText();
                
                StanleyInstructionEmitter.EmitAllocVariable(
                    currentBuilder,
                    varName,
                    typeName);
            }

            return null;
        }

        public override object VisitActionStatement([NotNull] StanleyParser.ActionStatementContext context)
        {
            // Handle arguments first if present
            if (context.actionArguments() != null)
            {
                var args = context.actionArguments();
                
                // Handle WITH arguments in reverse order
                var argumentList = args.argumentList();
                
                if (argumentList != null
                    && argumentList.Length > 0)
                {
                    for (int i = argumentList.Length - 1; i >= 0; i--)
                    {
                        var expressions = argumentList[i].expression();

                        for (int j = expressions.Length - 1; j >= 0; j--)
                        {
                            Visit(expressions[j]);
                        }
                    }
                }
            }
            
            if (context.HAS() != null)
            {
                // Handle "has <verb>" form
                string verb = context.VERB().GetText();
                
                StanleyInstructionEmitter.EmitPushString(currentBuilder, verb);
            }

            // Visit the subject reference to push the action onto the stack
            Visit(context.subjectReference());

            if (context.HAS() != null)
            {
                // Handle "has <verb>" form
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_PROPERTY);
            }
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_DELEGATE_CALL);

            return null;
        }

        public override object VisitAssignmentStatement([NotNull] StanleyParser.AssignmentStatementContext context)
        {
            Visit(context.expression()); // Push value to assign
            
            Visit(context.subject()); // Push target
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_REVERSE_COPY);

            return null;
        }
  
        public override object VisitEventHandlerStatement([NotNull] StanleyParser.EventHandlerStatementContext context)
        {
            string label;
            
            var subjectReference = context.subjectReference();

            bool isProperty =
                subjectReference.POSSESSIVE() != null
                && subjectReference.POSSESSIVE().Length > 0;

            if (isProperty)
            {
                string objectName = subjectReference.IDENTIFIER()[0].GetText();
                
                string eventName = subjectReference.IDENTIFIER()[1].GetText();
                
                label = $"evt_{objectName}_{eventName}_{currentLabelCount++}";
            }
            else
            {
                string name = subjectReference.IDENTIFIER()[0].GetText();
                
                label = $"evt_{name}_{currentLabelCount++}";
            }

            #region Emit method body
            
            // Switch to tail section for method body
            var previousBuilder = currentBuilder;
            currentBuilder = methodBuilder;
            
            // Emit method header
            StanleyInstructionEmitter.EmitComment(currentBuilder, $"Event handler {label}");
            StanleyInstructionEmitter.EmitLabel(currentBuilder, label);
            
            StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
            
            StanleyInstructionEmitter.EmitMethodHeader(currentBuilder);
            
            // Visit method body
            foreach (var stmt in context.statement())
            {
                Visit(stmt);
            }
            
            StanleyInstructionEmitter.EmitMethodFooter(currentBuilder);
            
            // Switch back to previous section
            currentBuilder = previousBuilder;
            
            #endregion
            
            // Emit label allocation
            StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, label);
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
            
            #region Emit event variable assignment

            StanleyInstructionEmitter.EmitPushString(currentBuilder, label);
            StanleyInstructionEmitter.EmitPushBool(currentBuilder, true);
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_EVENT);
            
            if (isProperty)
            {
                string objectName = subjectReference.IDENTIFIER()[0].GetText();
                string eventName = subjectReference.IDENTIFIER()[1].GetText();
                
                StanleyInstructionEmitter.EmitPushProperty(
                    currentBuilder,
                    objectName,
                    eventName);
            }
            else
            {
                string name = subjectReference.IDENTIFIER()[0].GetText();
                
                StanleyInstructionEmitter.EmitPushVariable(
                    currentBuilder,
                    name);
            }
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_REVERSE_COPY);
            
            #endregion
            
            return null;
        }

        public override object VisitDelegateHandlerStatement([NotNull] StanleyParser.DelegateHandlerStatementContext context)
        {
            string label;
            
            var subjectReference = context.subjectReference();
            
            bool isProperty = 
                subjectReference.POSSESSIVE() != null
                && subjectReference.POSSESSIVE().Length > 0;

            if (isProperty)
            {
                string objectName = subjectReference.IDENTIFIER()[0].GetText();
                
                string actionName = subjectReference.IDENTIFIER()[1].GetText();
                
                label = $"act_{objectName}_{actionName}_{currentLabelCount++}";
            }
            else
            {
                string name = subjectReference.IDENTIFIER()[0].GetText();
                
                label = $"act_{name}_{currentLabelCount++}";
            }

            #region Emit method body
            
            // Switch to tail section for method body
            var previousBuilder = currentBuilder;
            currentBuilder = methodBuilder;
            
            // Emit method header
            StanleyInstructionEmitter.EmitComment(currentBuilder, $"Delegate handler {label}");
            StanleyInstructionEmitter.EmitLabel(currentBuilder, label);
            
            StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
            
            StanleyInstructionEmitter.EmitMethodHeader(currentBuilder);
            
            // Visit method body
            foreach (var stmt in context.statement())
            {
                Visit(stmt);
            }
            
            StanleyInstructionEmitter.EmitMethodFooter(currentBuilder);
            
            // Switch back to previous section
            currentBuilder = previousBuilder;
            
            #endregion
            
            // Emit label allocation
            StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, label);
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
            
            #region Emit delegate variable assignment

            StanleyInstructionEmitter.EmitPushString(currentBuilder, label);
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_DELEGATE);
            
            if (isProperty)
            {
                string objectName = subjectReference.IDENTIFIER()[0].GetText();
                string actionName = subjectReference.IDENTIFIER()[1].GetText();
                
                StanleyInstructionEmitter.EmitPushProperty(
                    currentBuilder,
                    objectName,
                    actionName);
            }
            else
            {
                string name = subjectReference.IDENTIFIER()[0].GetText();
                
                StanleyInstructionEmitter.EmitPushVariable(
                    currentBuilder,
                    name);
            }
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_REVERSE_COPY);
            
            #endregion
            
            return null;
        }

        public override object VisitAssertStatement([NotNull] StanleyParser.AssertStatementContext context)
        {
            Visit(context.condition());
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_ASSERT);
            
            return null;
        }

        public override object VisitTimeStatement([NotNull] StanleyParser.TimeStatementContext context)
        {
            if (context.timeExpression() != null)
            {
                Visit(context.timeExpression());
                
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_WAIT_MS);
            }
            else if (context.subjectReference() != null)
            {
                Visit(context.subjectReference());
                
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_WAIT_EVENT);
            }
            
            return null;
        }

        public override object VisitEventStatement([NotNull] StanleyParser.EventStatementContext context)
        {
            Visit(context.subjectReference());
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_EVENT_RAISE);
            
            return null;
        }

        public override object VisitLoopStatement([NotNull] StanleyParser.LoopStatementContext context)
        {
            string startLabel = $"loop_start_{currentLabelCount}";
            string endLabel = $"loop_end_{currentLabelCount}";

            currentLabelCount++;
            
            // Emit label allocation
            StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, startLabel);
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
            
            StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, endLabel);
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);

            if (context.THIS_WAS_REPEATED() != null)
            {
                // Count-based loop: "This was repeated 5 times:"
                Visit(context.expression()); // Push count to stack
                
                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

                // Create new scope for loop variables
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_SCOPE);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_POP); // Pop scope handle

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Allocate and initialize count in context
                StanleyInstructionEmitter.EmitAllocProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNT_VARIABLE_NAME, typeof(int).FullName);
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNT_VARIABLE_NAME);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_REVERSE_COPY);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Allocate and initialize counter in context
                StanleyInstructionEmitter.EmitAllocProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME, typeof(int).FullName);
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME);
                StanleyInstructionEmitter.EmitPushInt(currentBuilder, 0);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_COPY);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Start of loop
                StanleyInstructionEmitter.EmitComment(currentBuilder, $"Start of loop {startLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, startLabel);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Stack: []
                // Compare counter >= count
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNT_VARIABLE_NAME); // Stack: [count]
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_RVALUE);

                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME); // Stack: [count, counter]
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_RVALUE);

                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MEQ); // counter >= count
                StanleyInstructionEmitter.EmitPushString(currentBuilder, endLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL_CONDITIONAL);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Loop body
                foreach (var stmt in context.statement())
                {
                    Visit(stmt);
                }

                // Increment counter
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_DUPE);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_INC);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_COPY);

                // Jump back to start
                StanleyInstructionEmitter.EmitPushString(currentBuilder, startLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // End of loop
                StanleyInstructionEmitter.EmitComment(currentBuilder, $"End of loop {endLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, endLabel);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Pop scope (this will clean up the counter variable)
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_POP_SCOPE);
            }
            else if (context.AS_LONG_AS() != null)
            {
                // While loop: "As long as Counter was less than 10:"
                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

                // Create new scope for loop variables
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_SCOPE);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_POP); // Pop scope handle

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Start of loop
                StanleyInstructionEmitter.EmitComment(currentBuilder, $"Start of loop {startLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, startLabel);
                
                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

                // Check condition
                Visit(context.condition()); // Pushes bool to stack
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_NOT); // Invert condition
                StanleyInstructionEmitter.EmitPushString(currentBuilder, endLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL_CONDITIONAL);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Loop body
                foreach (var stmt in context.statement())
                {
                    Visit(stmt);
                }

                // Jump back to start
                StanleyInstructionEmitter.EmitPushString(currentBuilder, startLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL);
                
                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // End of loop
                StanleyInstructionEmitter.EmitComment(currentBuilder, $"End of loop {endLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, endLabel);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Pop scope (this will clean up the counter variable)
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_POP_SCOPE);
            }
            else if (context.FOR_EACH() != null)
            {
                // For-each loop: "For each Item in Items:"
                string itemVar = context.IDENTIFIER(0).GetText();
                string listVar = context.IDENTIFIER(1).GetText();

                // Create new scope for loop variables
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_SCOPE);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_POP); // Pop scope handle

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Allocate and initialize counter in context
                StanleyInstructionEmitter.EmitAllocProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME, typeof(int).FullName);
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME);
                StanleyInstructionEmitter.EmitPushInt(currentBuilder, 0);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_COPY);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Push list reference
                StanleyInstructionEmitter.EmitPushVariable(currentBuilder, listVar);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Start of loop
                StanleyInstructionEmitter.EmitComment(currentBuilder, $"Start of loop {startLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, startLabel);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Stack: [list]
                // Check if counter >= list.Count
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_DUPE); // Stack: [list, list]
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_COUNT); // Stack: [list, count]

                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME); // Stack: [list, count, counter]
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_RVALUE);

                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MEQ); // counter >= count
                StanleyInstructionEmitter.EmitPushString(currentBuilder, endLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL_CONDITIONAL);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Get current item
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME); // Stack: [list, counter]
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_RVALUE);
                
                StanleyInstructionEmitter.EmitPushInt(currentBuilder, 1);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_DUPE_FROM_TOP); // Stack: [list, counter, list]

                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_VALATINDEX); // Stack: [list, item]
                
                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Allocate and set loop variable
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_DUPE); // Stack: [list, item, item]
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_TYPE_GET); // Stack: [list, item, type]

                StanleyInstructionEmitter.EmitPushString(currentBuilder, itemVar);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_ALLOC_VARIABLE); // Stack: [list, item]

                StanleyInstructionEmitter.EmitPushVariable(currentBuilder, itemVar);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_REVERSE_COPY); // Stack: [list]

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Loop body
                foreach (var stmt in context.statement())
                {
                    Visit(stmt);
                }

                // Increment counter
                StanleyInstructionEmitter.EmitPushProperty(currentBuilder, StanleyConsts.SCOPE_OBJECT_NAME, StanleyConsts.SCOPE_COUNTER_VARIABLE_NAME);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_DUPE);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_INC);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_COPY);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Jump back to start
                StanleyInstructionEmitter.EmitPushString(currentBuilder, startLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // End of loop
                StanleyInstructionEmitter.EmitComment(currentBuilder, $"End of loop {endLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, endLabel);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                
                // Clean up loop variables from stack
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_POP); // Pop list

                // Pop scope (this will clean up the item variable)
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_POP_SCOPE);
            }

            return null;
        }

        public override object VisitListOperationStatement([NotNull] StanleyParser.ListOperationStatementContext context)
        {
            var subjects = context.subjectReference();
            var expressions = context.expression();
            
            // Handle list operation based on context
            if (context.WAS_ADDED() != null || context.HAD_ADDED() != null)
            {
                // Push the value first, then the list
                if (expressions != null && expressions.Length > 0)
                {
                    Visit(expressions[0]); // Value to add
                }
                Visit(subjects[0]); // List variable
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_PUSH);
            }
            else if (context.WAS_INSERTED() != null || context.HAD_INSERTED() != null)
            {
                if (expressions != null && expressions.Length >= 2)
                {
                    Visit(expressions[0]); // Value to insert
                    Visit(expressions[1]); // Position to insert at
                    Visit(subjects[0]); // List variable
                    StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_INSERTAT);
                }
            }
            else if (context.WAS_REMOVED() != null || context.HAD_REMOVED() != null)
            {
                if (expressions != null && expressions.Length > 0)
                {
                    Visit(expressions[0]); // Position to remove from
                }
                Visit(subjects[0]); // List variable
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_REMOVEAT);
            }
            else if (context.BECAME_EMPTY() != null)
            {
                Visit(subjects[0]); // List variable
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_CLEAR);
            }
            else if (context.WAS_COMBINED() != null || context.HAD_COMBINED() != null)
            {
                if (subjects.Length >= 2)
                {
                    Visit(subjects[0]); // First list
                    Visit(subjects[1]); // Second list
                    StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_CONCAT);
                }
            }
            
            return null;
        }

        public override object VisitProgramStatement([NotNull] StanleyParser.ProgramStatementContext context)
        {
            // Program "file.txt" was loaded into ScriptA
            string filename = context.STRING().GetText();
            
            // Remove quotes
            filename = filename.Substring(1, filename.Length - 2);
            
            //Load the program
            StanleyInstructionEmitter.EmitPushString(currentBuilder, filename);
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_SUBPROGRAM);
            
            //Load the variable's HANDLE property
            StanleyInstructionEmitter.EmitPushString(currentBuilder, StanleyConsts.PROGRAM_HANDLE_VARIABLE_NAME);
            
            // Visit the target variable reference
            Visit(context.subjectReference());
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_PROPERTY);

            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_REVERSE_COPY);

            return null;
        }
        
        public override object VisitConditionalStatement([NotNull] StanleyParser.ConditionalStatementContext context)
        {
            // Generate unique labels for branching
            int currentElseCount = 0;

            bool hasAnyElse = false;

            var elseIfClauses = context.elseIfClause();

            if (elseIfClauses != null
                && elseIfClauses.Length > 0)
            {
                hasAnyElse = true;

                for (int i = 0; i < elseIfClauses.Length; i++)
                {
                    var elseLabel = $"else_{currentLabelCount}_{currentElseCount}";

                    currentElseCount++;

                    // Emit label allocation
                    StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, elseLabel);
                    StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
                }
            }

            if (context.elseClause() != null)
            {
                hasAnyElse = true;

                var elseLabel = $"else_{currentLabelCount}_{currentElseCount}";

                // Emit label allocation
                StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, elseLabel);
                StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);
            }

            var endLabel = $"endif_{currentLabelCount}";

            // Emit label allocation
            StanleyInstructionEmitter.EmitLabelAddressPlaceholder(labelBuilder, endLabel);
            StanleyInstructionEmitter.EmitEmptyLine(labelBuilder);

            currentElseCount = 0;

            // Visit the main condition - this will leave a boolean on the stack
            Visit(context.condition());
            
            // Jump to else label if condition is false
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_NOT); // Invert condition

            string targetLabel = (hasAnyElse)
                ? $"else_{currentLabelCount}_{currentElseCount}"
                : endLabel;

            StanleyInstructionEmitter.EmitPushString(currentBuilder, targetLabel);
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL_CONDITIONAL);

            StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

            // Visit the main block
            foreach (var statement in context.statement())
            {
                Visit(statement);
            }

            StanleyInstructionEmitter.EmitPushString(currentBuilder, endLabel);
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL);

            StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

            // Handle else-if clauses
            if (elseIfClauses != null
                && elseIfClauses.Length > 0)
            {
                for (int i = 0; i < elseIfClauses.Length; i++)
                {
                    var elseLabel = $"else_{currentLabelCount}_{currentElseCount}";

                    currentElseCount++;

                    StanleyInstructionEmitter.EmitComment(currentBuilder, $"Else if {elseLabel}");
                    StanleyInstructionEmitter.EmitLabel(currentBuilder, elseLabel);

                    StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

                    Visit(elseIfClauses[i].condition());
            
                    // Jump to else label if condition is false
                    StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_NOT); // Invert condition

                    bool isLastElse =
                        (i == elseIfClauses.Length - 1)
                        && context.elseClause() == null;

                    targetLabel = (!isLastElse)
                        ? $"else_{currentLabelCount}_{currentElseCount}"
                        : endLabel;

                    StanleyInstructionEmitter.EmitPushString(currentBuilder, targetLabel);
                    StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL_CONDITIONAL);

                    StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

                    // Visit the main block
                    foreach (var statement in elseIfClauses[i].statement())
                    {
                        Visit(statement);
                    }

                    StanleyInstructionEmitter.EmitPushString(currentBuilder, endLabel);
                    StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL);

                    StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
                }
            }

            // Handle else clause if present
            if (context.elseClause() != null)
            {
                var elseLabel = $"else_{currentLabelCount}_{currentElseCount}";

                StanleyInstructionEmitter.EmitComment(currentBuilder, $"Else {elseLabel}");
                StanleyInstructionEmitter.EmitLabel(currentBuilder, elseLabel);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

                // Visit the main block
                foreach (var statement in context.elseClause().statement())
                {
                    Visit(statement);
                }

                StanleyInstructionEmitter.EmitPushString(currentBuilder, endLabel);
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_JUMP_LABEL);

                StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);
            }

            // Mark the end of the entire if statement
            StanleyInstructionEmitter.EmitComment(currentBuilder, $"End of if statement {endLabel}");
            StanleyInstructionEmitter.EmitLabel(currentBuilder, endLabel);

            StanleyInstructionEmitter.EmitEmptyLine(currentBuilder);

            currentLabelCount++;

            return null;
        }

        #endregion

        public override object VisitSubject([NotNull] StanleyParser.SubjectContext context)
        {
            if (context.subjectReference() != null)
            {
                Visit(context.subjectReference());
            }
            else if (context.listValue() != null)
            {
                Visit(context.listValue());
            }

            return null;
        }
        
        public override object VisitElseIfClause([NotNull] StanleyParser.ElseIfClauseContext context)
        {
            // This is handled by VisitConditionalStatement
            return null;
        }

        public override object VisitElseClause([NotNull] StanleyParser.ElseClauseContext context)
        {
            // This is handled by VisitConditionalStatement
            return null;
        }
        
        public override object VisitExpression([NotNull] StanleyParser.ExpressionContext context)
        {
            if (context.REAL_NUMBER() != null)
            {
                /*
                float value = float.Parse(
                    context.REAL_NUMBER().ToString(),
                    CultureInfo.InvariantCulture);
                */
                    
                StanleyInstructionEmitter.EmitPushFloat(currentBuilder, context.REAL_NUMBER().GetText());
            }
            else if (context.NUMBER() != null)
            {
                //int value = int.Parse(context.NUMBER().GetText());
                
                StanleyInstructionEmitter.EmitPushInt(currentBuilder, context.NUMBER().GetText());
            }
            else if (context.STRING() != null)
            {
                string text = context.STRING().GetText();
                
                // Remove quotes
                text = text.Substring(1, text.Length - 2);
                
                StanleyInstructionEmitter.EmitPushString(currentBuilder, text);
            }
            else if (context.COUNT() != null)
            {
                // Get list count
                Visit(context.subjectReference());
                
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_COUNT);
            }
            else if (context.operation() != null)
            {
                Visit(context.expression(1));
                
                Visit(context.expression(0));
                
                Visit(context.operation());
            }
            else if (context.listValue() != null)
            {
                Visit(context.listValue());
            }
            else if (context.LPAREN() != null)
            {
                Visit(context.expression(0));
            }
            else if (context.subjectReference() != null)
            {
                Visit(context.subjectReference());
            }
            
            StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_STACK_RVALUE);

            return null;
        }

        public override object VisitCondition([NotNull] StanleyParser.ConditionContext context)
        {
            // Visit second expression
            Visit(context.expression(1));
            
            // Visit first expression
            Visit(context.expression(0));

            // Handle comparison
            var comparison = context.comparison();
            
            if (comparison != null)
            {
                switch (comparison.GetText())
                {
                    case "was":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_EQ);
                        break;
                    case "was not":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_NEQ);
                        break;
                    case "was greater than":
                    case "became greater than":
                    case "had been greater than":
                    case ">":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MORE);
                        break;
                    case "was less than":
                    case "became less than":
                    case "had been less than":
                    case "<":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LESS);
                        break;
                    case "was at least":
                    case "became at least":
                    case "had been at least":
                    case ">=":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MEQ);
                        break;
                    case "was at most":
                    case "became at most":
                    case "had been at most":
                    case "<=":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LEQ);
                        break;
                    case "==":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_EQ);
                        break;
                    case "!=":
                        StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_NEQ);
                        break;
                }
            }
            else
            {
                // No comparison operator means equality check
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_EQ);
            }

            return null;
        }

        public override object VisitTimeExpression([NotNull] StanleyParser.TimeExpressionContext context)
        {
            long totalMilliseconds = 0;
            
            do
            {
                long value = long.Parse(context.NUMBER().GetText());
                
                string unit = context.timeUnit().GetText();
                
                switch (unit)
                {
                    case "milliseconds":
                        totalMilliseconds += value;
                        break;
                    case "seconds":
                        totalMilliseconds += value * 1000;
                        break;
                    case "minutes":
                        totalMilliseconds += value * 60 * 1000;
                        break;
                }
                
                context = context.timeExpression();
            }
            while (context != null);
            
            StanleyInstructionEmitter.EmitPushInt(currentBuilder, Convert.ToInt32(totalMilliseconds));
            
            return null;
        }

        public override object VisitSubjectReference([NotNull] StanleyParser.SubjectReferenceContext context)
        {
            var identifiers = context.IDENTIFIER();
            
            if (identifiers == null || identifiers.Length == 0)
            {
                throw new Exception("Subject reference must have at least one identifier");
            }

            // For single identifier, just push it as a variable
            if (identifiers.Length == 1)
            {
                StanleyInstructionEmitter.EmitPushVariable(currentBuilder, identifiers[0].GetText());
                
                return null;
            }

            // For object properties (e.g. Display's OnUpdate), emit get property
            string objectName = identifiers[0].GetText();

            for (int i = identifiers.Length - 1; i >= 1; i--)
            {
                StanleyInstructionEmitter.EmitPushString(currentBuilder, identifiers[i].GetText());
            }
                
            StanleyInstructionEmitter.EmitPushVariable(currentBuilder, objectName);

            for (int i = identifiers.Length - 1; i >= 1; i--)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_PUSH_PROPERTY);
            }

            return null;
        }

        public override object VisitListValue([NotNull] StanleyParser.ListValueContext context)
        {
            if (context.FIRST() != null)
            {
                // Get first value from list
                Visit(context.subjectReference());
                
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_FIRSTVAL);
            }
            else if (context.LAST() != null)
            {
                // Get last value from list
                Visit(context.subjectReference());
                
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_LASTVAL);
            }
            else if (context.AT_POSITION() != null)
            {
                // Get value at index from list
                Visit(context.expression()); // Push index to stack
                
                Visit(context.subjectReference());
                
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LIST_VALATINDEX);
            }

            return null;
        }

        public override object VisitOperation([NotNull] StanleyParser.OperationContext context)
        {
            if (context.PLUS() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_ADD);
            }
            else if (context.MINUS() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_SUBSTRACT);
            }
            else if (context.MULTIPLY() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MULTIPLY);
            }
            else if (context.DIVIDE() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_DIVIDE);
            }
            else if (context.POWER() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_POWER);
            }
            else if (context.MOD() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MODULO);
            }
            else if (context.EQUAL() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_EQ);
            }
            else if (context.NOT_EQUAL() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_NEQ);
            }
            else if (context.GREATER_THAN() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MORE);
            }
            else if (context.LESS_THAN() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LESS);
            }
            else if (context.GREATER_THAN_OR_EQUAL() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_MEQ);
            }
            else if (context.LESS_THAN_OR_EQUAL() != null)
            {
                StanleyInstructionEmitter.Emit(currentBuilder, StanleyOpcodes.OP_LEQ);
            }

            return null;
        }

        private string MapType(string type)
        {
            return type.ToLower() switch
            {
                "action" => typeof(StanleyDelegate).FullName,
                "event" => typeof(StanleyEvent).FullName,
                "number" => typeof(int).FullName,
                "real number" => typeof(float).FullName,
                "text" => typeof(string).FullName,
                "fact" => typeof(bool).FullName,
                "program" => typeof(StanleyProgramObject).FullName,
                "object" => typeof(StanleyObject).FullName,
                "list" => typeof(StanleyList).FullName,
                _ => type
            };
        }
    }
}
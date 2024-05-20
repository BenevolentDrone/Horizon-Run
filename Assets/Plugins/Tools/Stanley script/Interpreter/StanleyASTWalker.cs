using System.Collections.Generic;

using HereticalSolutions.StanleyScript.Grammars;

using Antlr4.Runtime.Misc;

namespace HereticalSolutions.StanleyScript
{
	public class StanleyASTWalker
		: StanleyParserBaseVisitor<object>
	{
		//Walking runtime

		private readonly List<string> instructions;

		private int currentStackDepth = 0;

		private int actionTargetStackIndex = -1;

		private bool flush = false;

		private bool objectCanBeRuntimeVariable = true;

		public StanleyASTWalker(
			IRuntimeEnvironment runtimeEnvironment,
			List<string> instructions)
		{
			this.instructions = instructions;

			Initialize();
		}

		public void Initialize()
		{
			instructions.Clear();

			currentStackDepth = 0;

			actionTargetStackIndex = -1;

			flush = false;

			objectCanBeRuntimeVariable = true;
		}

		public string[] GetInstructions()
		{
			return instructions.ToArray();
		}

		public override object VisitScript(StanleyParser.ScriptContext context)
		{
			base.VisitScript(context);

			return null;
		}

		public override object VisitStatement(StanleyParser.StatementContext context)
		{
			int stackDepthAtContextStart = currentStackDepth;

			int lineIndex = context.Start.Line;

			instructions.Add(
				$"OP_PUSH_INT {lineIndex}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				"OP_LINE");

			base.VisitStatement(context);

			return null;
		}

        public override object VisitStoryHeader([NotNull] StanleyParser.StoryHeaderContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			int lineIndex = context.Start.Line;

			instructions.Add(
				$"OP_PUSH_INT {lineIndex}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				"OP_LINE");

			objectCanBeRuntimeVariable = false;

			base.VisitStoryHeader(context);

			objectCanBeRuntimeVariable = true;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_STORY");

			return null;
        }

        public override object VisitDefineStatement([NotNull] StanleyParser.DefineStatementContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			objectCanBeRuntimeVariable = false;

			base.VisitDefineStatement(context);

			objectCanBeRuntimeVariable = true;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_ALLOC_RTM");

			return null;
        }

        public override object VisitActionStatement([NotNull] StanleyParser.ActionStatementContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			actionTargetStackIndex = stackDepthAtContextStart;

			base.VisitActionStatement(context);

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_INVOKE");

			if (flush)
			{
				//Flushing because (plural)subjectExpression will put the target at the beginning of the stack so that it can be peeked later
				instructions.Add(
					$"OP_FLUSH");
			}
			
			flush = false;

			actionTargetStackIndex = -1;

			return null;
		}

/*
        public override object VisitActionExpression([NotNull] StanleyParser.ActionExpressionContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			base.VisitActionExpression(context);

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_INVOKE");

			return null;
		}
*/

        public override object VisitActionWithArguments([NotNull] StanleyParser.ActionWithArgumentsContext context)
		{
			//int stackDepthAtContextStart = currentStackDepth;

			//BE CAREFUL WHEN REORDERING VISITS
			//DO NOT USE base.VisitRight or base.VisitLeft
			//BECAUSE YOU MAY HAVE OVERRIDEN THOSE VISITS
			//BUT YOU STILL CALL TO THEIR BASE IMPLEMENTATIONS
			VisitObjectArgument(context.objectArgument());

			instructions.Add(
				$"OP_PUSH_INT {actionTargetStackIndex}");

			instructions.Add(
				$"OP_PUSH_STK");

			currentStackDepth++;

			VisitAction(context.action());

			flush = true;

			return null;
		}

		public override object VisitPluralSubjectsExpression([NotNull] StanleyParser.PluralSubjectsExpressionContext context)
		{
			int stackDepthAtContextStart = currentStackDepth;

			base.VisitPluralSubjectsExpression(context);

			var subjects = context.subject();

			var subjectsAmount = subjects.Length;

			instructions.Add(
				$"OP_PUSH_INT {subjectsAmount}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_CONCAT");

			currentStackDepth++;

			return null;
		}

		public override object VisitPluralObjectsExpression([NotNull] StanleyParser.PluralObjectsExpressionContext context)
		{
			int stackDepthAtContextStart = currentStackDepth;

			base.VisitPluralObjectsExpression(context);

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_SCAL");

			currentStackDepth++;

			return null;
		}

		//The signature should look like this:
		//select SELECTOR FROM_WHAT ARGUMENT ASSERT_AMOUNT ASSERT? ASSERT?
		public override object VisitObjectSelectedByQuality([NotNull] StanleyParser.ObjectSelectedByQualityContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			//base.VisitObjectSelectedByQuality(context);

			//First, let's push asserts
			var assertAdjectives = context.assertAdjective();

			int assertAdjectivesAmount = assertAdjectives.Length;

			for (int i = 0; i < assertAdjectivesAmount; i++)
			{
				VisitAssertAdjective(assertAdjectives[i]);
			}

			//Then asserts amount
			instructions.Add(
				$"OP_PUSH_INT {assertAdjectivesAmount}");

			currentStackDepth++;

			//Then the argument (in quality selection it's the target)
			instructions.Add(
				$"OP_PUSH_INT {actionTargetStackIndex}");

			instructions.Add(
				$"OP_PUSH_STK");

			currentStackDepth++;

			//Then the what (selection adjective)
			VisitObject(context.@object());

			//Then the selector
			VisitSelectionAdjective(context.selectionAdjective());

			//Finally, the opcode
			instructions.Add(
				$"OP_PUSH_STR {StanleyConsts.SELECTION_OPCODE}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_INVOKE");

			currentStackDepth++;

			return null;
        }

		//The signature should look like this:
		//select SELECTOR FROM_WHAT ARGUMENT ASSERT_AMOUNT ASSERT? ASSERT?
		public override object VisitObjectSelectedInRelation([NotNull] StanleyParser.ObjectSelectedInRelationContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			//base.VisitObjectSelectedInRelation(context);

			//First, let's push asserts amount
			instructions.Add(
				$"OP_PUSH_INT {0}");

			currentStackDepth++;

			//Then the argument (in relation selection it's the subject expression)
			VisitSubjectExpression(context.subjectExpression());

			//Then the what (selection adjective)
			VisitObject(context.@object());

			//Then the selector
			VisitRelativeSelectionAdjective(context.relativeSelectionAdjective());

			//Finally, the opcode
			instructions.Add(
				$"OP_PUSH_STR {StanleyConsts.SELECTION_OPCODE}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_INVOKE");

			currentStackDepth++;

			return null;
		}

        public override object VisitSelectionAdjective([NotNull] StanleyParser.SelectionAdjectiveContext context)
		{
			base.VisitSelectionAdjective(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_STR {text}");

			currentStackDepth++;

			return null;
		}

		public override object VisitRelativeSelectionAdjective([NotNull] StanleyParser.RelativeSelectionAdjectiveContext context)
		{
			base.VisitRelativeSelectionAdjective(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_STR {text}");

			currentStackDepth++;

			return null;
		}

		public override object VisitAssertAdjective([NotNull] StanleyParser.AssertAdjectiveContext context)
		{
			base.VisitAssertAdjective(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_STR {text}");

			currentStackDepth++;

			return null;
		}

		public override object VisitObject([NotNull] StanleyParser.ObjectContext context)
        {
			base.VisitObject(context);

			var text = context.GetText();

			instructions.Add($"OP_PUSH_STR {text}");

			if (objectCanBeRuntimeVariable)
			{
				instructions.Add($"OP_TCAST_RTM");
			}

			currentStackDepth++;

			return null;
        }

        public override object VisitAction([NotNull] StanleyParser.ActionContext context)
        {
			base.VisitAction(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_STR {text}");

			currentStackDepth++;

			return null;
        }

		public override object VisitImportVariableLiteral([NotNull] StanleyParser.ImportVariableLiteralContext context)
		{
			int stackDepthAtContextStart = currentStackDepth;

			base.VisitImportVariableLiteral(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_STR {text.Substring(1, text.Length - 1)}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_PUSH_IMP");

			currentStackDepth++;

			return null;
		}

		public override object VisitRuntimeVariableLiteral([NotNull] StanleyParser.RuntimeVariableLiteralContext context)
        {
			int stackDepthAtContextStart = currentStackDepth;

			base.VisitRuntimeVariableLiteral(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_STR {text}");

			currentStackDepth++;

			currentStackDepth = stackDepthAtContextStart;

			instructions.Add(
				$"OP_PUSH_RTM");

			currentStackDepth++;

			return null;
		}

        public override object VisitInteger([NotNull] StanleyParser.IntegerContext context)
        {
            base.VisitInteger(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_INT {text}");

			currentStackDepth++;

			return null;
        }

        public override object VisitFloat([NotNull] StanleyParser.FloatContext context)
        {
			base.VisitFloat(context);

			var text = context.GetText();

			instructions.Add(
				$"OP_PUSH_FLT {text}");

			currentStackDepth++;

			return null;
		}
    }
}
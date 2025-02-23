using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
    public class RValue
        : AStanleyInstructionHandler
    {
        public RValue(
            ILoggerResolver loggerResolver,
            ILogger logger)
            : base(
                loggerResolver,
                logger)
        {
        }

        #region IStanleyInstructionHandler

        #region Instruction

        public override string Instruction => StanleyOpcodes.OP_STACK_RVALUE;

        #endregion

        #region Arguments

        public override int ArgumentsCount => 1;

        public override Type[] ArgumentTypes => new Type[]
        {
            null
        };

        #endregion

        #region Return values

        public override int ReturnValuesCount => 1;

        public override Type[] ReturnValueTypes => new Type[]
        {
            null
        };

        #endregion

        public override bool Handle(
            IStanleyContextInternal context,
            string instruction,
            string[] instructionTokens)
        {
            var currentLogger = SelectLogger(
                context);

            var stack = context.StackMachine;

            if (!stack.Pop(
                    out var targetValue))
            {
                currentLogger?.LogError(
                    GetType(),
                    "STACK VARIABLE NOT FOUND");

                return false;
            }

            stack.Push(
                StanleyFactory.BuildValueVariable(
                    StanleyConsts.RVALUE_VARIABLE_NAME,
                    targetValue.VariableType,
                    targetValue.GetValue(),
                    loggerResolver));

            return true;
        }

        #endregion
    }
}
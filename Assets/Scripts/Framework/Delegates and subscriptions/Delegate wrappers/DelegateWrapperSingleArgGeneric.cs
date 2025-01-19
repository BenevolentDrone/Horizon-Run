using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Wrappers
{
    public class DelegateWrapperSingleArgGeneric<TValue>
        : IInvokableSingleArgGeneric<TValue>,
          IInvokableSingleArg
    {
        private readonly Action<TValue> @delegate;
        
        private readonly ILogger logger;

        public DelegateWrapperSingleArgGeneric(
            Action<TValue> @delegate,
            ILogger logger)
        {
            this.@delegate = @delegate;

            this.logger = logger;
        }

        #region IInvokableSingleArgGeneric
        
        public void Invoke(
            TValue argument)
        {
            @delegate?.Invoke(argument);
        }

        public void Invoke(
            object argument)
        {
            switch (argument)
            {
                case TValue tValue:

                    @delegate.Invoke(tValue);

                    break;

                default:

                    throw new ArgumentException(
                        logger.TryFormatException(
                            GetType(),
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{argument.GetType().Name}\""));
            }
        }

        #endregion

        #region IInvokableSingleArg

        public Type ValueType => typeof(TValue);

        public void Invoke<TArgument>(
            TArgument value)
        {
            switch (value)
            {
                case TValue tValue:

                    @delegate.Invoke(tValue);

                    break;

                default:

                    throw new ArgumentException(
                        logger.TryFormatException(
                            GetType(),
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{nameof(TArgument)}\""));
            }
        }

        public void Invoke(
            Type valueType,
            object value)
        {
            switch (value)
            {
                case TValue tValue:

                    @delegate.Invoke(tValue);

                    break;

                default:

                    throw new ArgumentException(
                        logger.TryFormatException(
                            GetType(),
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{nameof(TValue)}\" RECEIVED: \"{valueType.Name}\""));
            }
        }
        
        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq; //error CS1061: 'Delegate[]' does not contain a definition for 'Cast'

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Delegates.Broadcasting
{
    public class BroadcasterGeneric<TValue>
        : IPublisherSingleArgGeneric<TValue>,
          IPublisherSingleArg,
          ISubscribableSingleArgGeneric<TValue>,
          ISubscribableSingleArg,
          ICleanuppable,
          IDisposable
    {
        private readonly ILogger logger;

        private Action<TValue> multicastDelegate;

        public BroadcasterGeneric(
            ILogger logger = null)
        {
            this.logger = logger;

            multicastDelegate = null;
        }

        #region IPublisherSingleArgGeneric

        public void Publish(TValue value)
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invokation
            //That's why we'll copy the multicast delegate to a local variable and invoke it from there
            //multicastDelegate?.Invoke(value);

            var multicastDelegateCopy = multicastDelegate;

            multicastDelegateCopy?.Invoke(value);

            multicastDelegateCopy = null;
        }

        #endregion

        #region IPublisherSingleArg

        public void Publish<TArgument>(TArgument value)
        {
            switch (value)
            {
                case TValue tValue:

                    multicastDelegate?.Invoke(tValue);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<BroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Publish(Type valueType, object value)
        {
            switch (value)
            {
                case TValue tValue:

                    multicastDelegate?.Invoke(tValue);

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<BroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        #endregion

        #region ISubscribableSingleArgGeneric

        public void Subscribe(Action<TValue> @delegate)
        {
            multicastDelegate += @delegate;
        }

        public void Subscribe(object @delegate)
        {
            multicastDelegate += (Action<TValue>)@delegate;
        }

        public void Unsubscribe(Action<TValue> @delegate)
        {
            multicastDelegate -= @delegate;
        }

        public void Unsubscribe(object @delegate)
        {
            multicastDelegate -= (Action<TValue>)@delegate;
        }

        IEnumerable<Action<TValue>> ISubscribableSingleArgGeneric<TValue>.AllSubscriptions
        {
            get
            {
                //Kudos to Copilot for Cast() and the part after the ?? operator
                return multicastDelegate?
                    .GetInvocationList()
                    //.Cast<Action<TValue>>() //LINQ
                    .CastInvokationListToGenericActions<TValue>()
                    //?? Enumerable.Empty<Action<TValue>>(); //LINQ
                    ?? new Action<TValue>[0];
            }
        }

        #endregion

        #region ISubscribableSingleArg

        public void Subscribe<TArgument>(Action<TArgument> @delegate)
        {
            switch (@delegate)
            {
                case Action<TValue> tValue:

                    multicastDelegate += tValue;

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<BroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Subscribe(Type valueType, object @delegate)
        {
            switch (@delegate)
            {
                case Action<TValue> tValue:

                    multicastDelegate += tValue;

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<BroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        public void Unsubscribe<TArgument>(Action<TArgument> @delegate)
        {
            switch (@delegate)
            {
                case Action<TValue> tValue:

                    multicastDelegate -= tValue; //TODO: ensure works properly

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<BroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{typeof(TArgument).Name}\""));
            }
        }

        public void Unsubscribe(Type valueType, object @delegate)
        {
            switch (@delegate)
            {
                case Action<TValue> tValue:

                    multicastDelegate -= tValue;

                    break;

                default:

                    throw new Exception(
                        logger.TryFormatException<BroadcasterGeneric<TValue>>(
                            $"INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).Name}\" RECEIVED: \"{valueType.Name}\""));
            }
        }

        public IEnumerable<Action<TArgument>> GetAllSubscriptions<TArgument>()
        {
            //Kudos to Copilot for Cast() and the part after the ?? operator
            return multicastDelegate?
                .GetInvocationList()
                //.Cast<Action<TArgument>>() //LINQ
                .CastInvokationListToGenericActions<TArgument>()
                //?? Enumerable.Empty<Action<TArgument>>(); //LINQ
                ?? new Action<TArgument>[0];
        }

        public IEnumerable<object> GetAllSubscriptions(Type valueType)
        {
            //Kudos to Copilot for Cast() and the part after the ?? operator
            return multicastDelegate?
                .GetInvocationList()
                //.Cast<object>() //LINQ
                .CastInvokationListToObjects()
                //?? Enumerable.Empty<object>(); //LINQ
                ?? new object[0];
        }

        #endregion

        #region ISubscribable

        IEnumerable<object> ISubscribable.AllSubscriptions
        {
            get
            {
                //Kudos to Copilot for Cast() and the part after the ?? operator
                return multicastDelegate?
                    .GetInvocationList()
                    //.Cast<object>() //LINQ
                    .CastInvokationListToObjects()
                    //?? Enumerable.Empty<object>(); //LINQ
                    ?? new object[0];
            }
        }

        public void UnsubscribeAll()
        {
            multicastDelegate = null;
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            multicastDelegate = null;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            multicastDelegate = null;
        }

        #endregion
    }
}
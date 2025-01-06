using System;
using System.Collections.Generic;
//using System.Linq; //error CS1061: 'Delegate[]' does not contain a definition for 'Cast'

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Delegates.Pinging
{
    public class Pinger
        : IPublisherNoArgs,
          ISubscribableNoArgs,
          ICleanuppable,
          IDisposable
    {
        private Action multicastDelegate;

        #region IPublisherNoArgs

        /// <summary>
        /// Publishes the event to all subscribers.
        /// </summary>
        public void Publish()
        {
            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invokation
            //That's why we'll copy the multicast delegate to a local variable and invoke it from there
            //multicastDelegate?.Invoke();

            var multicastDelegateCopy = multicastDelegate;

            multicastDelegateCopy?.Invoke();

            multicastDelegateCopy = null;
        }

        #endregion

        #region ISubscribableNoArgs

        /// <summary>
        /// Subscribes to the event.
        /// </summary>
        /// <param name="delegate">The delegate to subscribe.</param>
        public void Subscribe(Action @delegate)
        {
            multicastDelegate += @delegate;
        }

        /// <summary>
        /// Unsubscribes from the event.
        /// </summary>
        /// <param name="delegate">The delegate to unsubscribe.</param>
        public void Unsubscribe(Action @delegate)
        {
            multicastDelegate -= @delegate;
        }

        IEnumerable<Action> ISubscribableNoArgs.AllSubscriptions
        {
            get
            {
                //Kudos to Copilot for Cast() and the part after the ?? operator
                return multicastDelegate?
                    .GetInvocationList()
                    //.Cast<Action>() //LINQ
                    .CastInvokationListToActions()
                    //?? Enumerable.Empty<Action>(); //LINQ
                    ?? new Action[0];
            }
        }

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
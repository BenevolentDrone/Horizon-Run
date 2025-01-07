using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Delegates
{
    public class Pinger
        : IPublisherNoArgs,
          ISubscribableNoArgs,
          ICleanuppable,
          IDisposable
    {
        private Action multicastDelegate;

        private IPool<PingerInvocationContext> contextPool;

        public Pinger(
            IPool<PingerInvocationContext> contextPool)
        {
            multicastDelegate = null;

            this.contextPool = contextPool;
        }

        #region IPublisherNoArgs

        public void Publish()
        {
            //TODO RIGHT NOW
            //EVERY TIME THE DELEGATE IS INVOKED, THE INVOCATION LIST IS COPIED INTO A NEW 'INVOCATION CONTEXT'
            //THIS INVOCATION CONTEXT ENSURES THE FOLLOWING:
            //1. IF ANY LITTLE SHIT UN/SUBSCRIBES DURING THE INVOCATION, IT DOES NOT AFFECT THE OPERATION OF CURRENT INVOCATION
            //2. IF ANY LITTLE SHIT IN INVOCATION LIST INVOKES THE PUBLISH METHOD DURING THE CURRENT INVOCATION, IT DOES NOT AFFECT THE OPERATION OF CURRENT INVOCATION
            //DO THIS WITH POOLS AND FOR BOTH DELEGATE AND NON ALLOC VERSIONS
            //MAYBE ADD HIDDEN 'DEPTH' VALUE FOR INVOCATIONS FOR SHITS AND GIGGLES AND TO PREVENT RECURSIVE INVOCATION HELL

            //If any delegate that is invoked attempts to unsubscribe itself, it would produce an error because the collection
            //should NOT be changed during the invocation
            //That's why we'll copy the multicast delegate to a local variable and invoke it from there
            //multicastDelegate?.Invoke();

            var context = contextPool.Pop();

            context.Delegate = multicastDelegate;

            context.Delegate?.Invoke();

            context.Delegate = null;

            contextPool.Push(context);
        }

        #endregion

        #region ISubscribableNoArgs

        public void Subscribe(Action @delegate)
        {
            multicastDelegate += @delegate;
        }

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
                    .CastInvocationListToActions()
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
                    .CastInvocationListToObjects()
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
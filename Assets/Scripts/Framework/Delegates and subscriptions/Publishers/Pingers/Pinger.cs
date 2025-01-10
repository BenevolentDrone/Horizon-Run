using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Delegates
{
    public class Pinger
        : IPublisherNoArgs,
          ISubscribableNoArgs,
          ICleanuppable,
          IDisposable
    {
        private readonly IPool<PingerInvocationContext> contextPool;

        private Action multicastDelegate;

        public Pinger(
            IPool<PingerInvocationContext> contextPool)
        {
            this.contextPool = contextPool;

            multicastDelegate = null;
        }

        #region ISubscribableNoArgs

        public void Subscribe(
            Action @delegate)
        {
            multicastDelegate += @delegate;
        }

        public void Unsubscribe(
            Action @delegate)
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

        #region IPublisherNoArgs

        public void Publish()
        {
            var context = contextPool.Pop();

            context.Delegate = multicastDelegate;

            context.Delegate?.Invoke();

            context.Delegate = null;

            contextPool.Push(context);
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
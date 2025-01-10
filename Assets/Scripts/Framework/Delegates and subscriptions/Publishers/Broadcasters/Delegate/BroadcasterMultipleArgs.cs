using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Delegates
{
    public class BroadcasterMultipleArgs
        : IPublisherMultipleArgs,
          ISubscribableMultipleArgs,
          ICleanuppable,
          IDisposable
    {
        private readonly BroadcasterGeneric<object[]> innerBroadcaster;

        public BroadcasterMultipleArgs(
            BroadcasterGeneric<object[]> innerBroadcaster)
        {
            this.innerBroadcaster = innerBroadcaster;
        }

        #region ISubscribableMultipleArgs
        
        public void Subscribe(
            Action<object[]> @delegate)
        {
            innerBroadcaster.Subscribe(@delegate);
        }

        public void Unsubscribe(
            Action<object[]> @delegate)
        {
            innerBroadcaster.Unsubscribe(@delegate);
        }

        IEnumerable<Action<object[]>> ISubscribableMultipleArgs.AllSubscriptions
        {
            get
            {
                return innerBroadcaster.GetAllSubscriptions<object[]>();
            }
        }

        #region ISubscribable

        IEnumerable<object> ISubscribable.AllSubscriptions
        {
            get
            {
                return ((ISubscribable)innerBroadcaster).AllSubscriptions;
            }
        }

        public void UnsubscribeAll()
        {
            innerBroadcaster.UnsubscribeAll();
        }

        #endregion

        #endregion

        #region IPublisherMultipleArgs

        public void Publish(
            object[] values)
        {
            innerBroadcaster.Publish(values);
        }

        #endregion

        #region ICleanUppable

        public void Cleanup()
        {
            if (innerBroadcaster is ICleanuppable)
                (innerBroadcaster as ICleanuppable).Cleanup();
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (innerBroadcaster is IDisposable)
                (innerBroadcaster as IDisposable).Dispose();
        }

        #endregion
    }
}
using System;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Messaging
{
    public interface INonAllocMessageSender
    {
        #region Pop

        INonAllocMessageSender PopMessage<TMessage>(
            out IPoolElementFacade<IMessage> messageElementFacade)
            where TMessage : IMessage;

        INonAllocMessageSender PopMessage(
            Type messageType,
            out IPoolElementFacade<IMessage> messageElementFacade);

        #endregion

        #region Write

        INonAllocMessageSender Write<TMessage>(
            IPoolElementFacade<IMessage> messageElementFacade,
            object[] args)
            where TMessage : IMessage;

        INonAllocMessageSender Write(
            IPoolElementFacade<IMessage> messageElementFacade,
            object[] args);

        #endregion

        #region Send

        void PutIntoMailbox<TMessage>(
            IPoolElementFacade<IMessage> messageElementFacade)
            where TMessage : IMessage;

        void PutIntoMailbox(
            IPoolElementFacade<IMessage> messageElementFacade);

        void SendImmediately<TMessage>(
            IPoolElementFacade<IMessage> messageElementFacade)
            where TMessage : IMessage;

        void SendImmediately(
            IPoolElementFacade<IMessage> messageElementFacade);

        #endregion

        #region Deliver

        void DeliverMessagesInMailbox();

        #endregion
    }
}
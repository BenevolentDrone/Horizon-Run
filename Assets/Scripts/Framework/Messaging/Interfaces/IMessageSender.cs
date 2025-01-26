
using System;

namespace HereticalSolutions.Messaging
{
    public interface IMessageSender
    {
        #region Pop

        IMessageSender PopMessage<TMessage>(
            out TMessage message)
            where TMessage : IMessage;

        IMessageSender PopMessage(
            Type messageType,
            out IMessage message);

        #endregion

        #region Write

        IMessageSender Write<TMessage>(
            TMessage message,
            object[] args)
            where TMessage : IMessage;

        IMessageSender Write(
            IMessage message,
            object[] args);

        #endregion

        #region Send

        void PutIntoMailbox<TMessage>(
            TMessage message)
            where TMessage : IMessage;

        void PutIntoMailbox(
            IMessage message);

        void SendImmediately<TMessage>(
            TMessage message)
            where TMessage : IMessage;

        void SendImmediately(
            IMessage message);

        #endregion

        #region Deliver

        void DeliverMessagesInMailbox();

        #endregion
    }
}

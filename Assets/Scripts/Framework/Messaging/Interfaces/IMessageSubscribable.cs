using System;

namespace HereticalSolutions.Messaging
{
	public interface IMessageSubscribable
	{
		void SubscribeTo<TMessage>(
            Action<TMessage> receiverDelegate) where TMessage : IMessage;
		
		void SubscribeTo(
            Type messageType,
            object receiverDelegate);

		void UnsubscribeFrom<TMessage>(
            Action<TMessage> receiverDelegate) where TMessage : IMessage;
		
		void UnsubscribeFrom(
            Type messageType,
            object receiverDelegate);
	}
}
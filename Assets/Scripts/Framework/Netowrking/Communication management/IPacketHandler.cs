using System;
using HereticalSolutions.Metadata;
using HereticalSolutions.StateMachines;

namespace HereticalSolutions.Networking
{
	public interface IPacketHandler
	{
		void RegisterPacketChainOfResponsibility<TPacket, THandler>(
			THandler handler);

		void BroadcastReceived();

		void ConnectionEstablished();

		//TODO: FILL THE REST FROM THE LITENETLIB AND/OR MIRROR
	}
}
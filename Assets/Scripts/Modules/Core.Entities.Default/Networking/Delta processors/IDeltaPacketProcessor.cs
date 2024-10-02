using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
	public interface IDeltaPacketProcessor
	{
		bool ProcessDeltasPacket(
			ClientReceivedPacketMessage message,
			NetworkTickSettings networkTickSettings,
			EntityManager entityManager,
			IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
			string relevantWorldID);
	}
}
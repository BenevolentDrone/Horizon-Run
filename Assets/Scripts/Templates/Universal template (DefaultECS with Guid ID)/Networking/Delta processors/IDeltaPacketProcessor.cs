using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

namespace HereticalSolutions.Templates.Universal.Networking
{
	public interface IDeltaPacketProcessor
	{
		bool ProcessDeltasPacket(
			ClientReceivedPacketMessage message,
			NetworkTickSettings networkTickSettings,
			UniversalTemplateEntityManager entityManager,
			IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
			string relevantWorldID);
	}
}
using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
	/*
	public class QuaternionDeltaProcessor
		: IDeltaPacketProcessor
	{
		public bool ProcessDeltasPacket(
			ClientReceivedPacketMessage message,
			NetworkTickSettings networkTickSettings,
			UniversalTemplateEntityManager entityManager,
			IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
			string relevantWorldID)
		{
			if (message.PacketType != typeof(EntityQuaternionDeltasPacket))
				return false;

			var packet = new EntityQuaternionDeltasPacket();

			packet.Deserialize(message.Reader);

			foreach (var idQuaternionPair in packet.Quaternions)
			{
				if (!networkIDToEntityIDMap.TryGetRight(
					idQuaternionPair.NetworkID,
					out var entityID))
					continue;

				var entity = entityManager.GetEntity(
					entityID,
					relevantWorldID);

				if (!entity.Has<ServerQuaternionComponent>())
					continue;

				ref var serverQuaternionComponent = ref entity.Get<ServerQuaternionComponent>();

				if (serverQuaternionComponent.ServerTick > packet.ServerTick
					&& (serverQuaternionComponent.ServerTick - packet.ServerTick) < networkTickSettings.MaxTickValue / 2)
					continue;

				serverQuaternionComponent.ServerQuaternion = idQuaternionPair.Quaternion;

				serverQuaternionComponent.ServerTick = packet.ServerTick;

				serverQuaternionComponent.Dirty = true;
			}

			return true;
		}
	}
	*/
}
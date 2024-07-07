using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

using HereticalSolutions.Templates.Universal.Networking;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
	public class Position3DDeltaProcessor
		: IDeltaPacketProcessor
	{
		public bool ProcessDeltasPacket(
			ClientReceivedPacketMessage message,
			NetworkTickSettings networkTickSettings,
			UniversalTemplateEntityManager entityManager,
			IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
			string relevantWorldID)
		{
			if (message.PacketType != typeof(EntityPosition3DDeltasPacket))
				return false;

			var packet = new EntityPosition3DDeltasPacket();

			packet.Deserialize(message.Reader);


			foreach (var idPositionPair in packet.Positions)
			{
				if (!networkIDToEntityIDMap.TryGetRight(
					idPositionPair.NetworkID,
					out var entityID))
					continue;

				var entity = entityManager.GetEntity(
					entityID,
					relevantWorldID);

				if (!entity.Has<ServerPosition3DComponent>())
					continue;

				ref var serverPosition3DComponent = ref entity.Get<ServerPosition3DComponent>();

				if (serverPosition3DComponent.ServerTick > packet.ServerTick
					&& (serverPosition3DComponent.ServerTick - packet.ServerTick) < networkTickSettings.MaxTickValue / 2)
					continue;

				serverPosition3DComponent.ServerPosition = idPositionPair.Position;

				serverPosition3DComponent.ServerTick = packet.ServerTick;

				serverPosition3DComponent.Dirty = true;
			}

			return true;
		}
	}
}
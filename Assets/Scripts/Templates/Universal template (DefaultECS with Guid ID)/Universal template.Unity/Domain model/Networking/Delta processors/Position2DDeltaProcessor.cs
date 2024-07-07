using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

using HereticalSolutions.Templates.Universal.Networking;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
	public class Position2DDeltaProcessor
		: IDeltaPacketProcessor
	{
		public bool ProcessDeltasPacket(
			ClientReceivedPacketMessage message,
			NetworkTickSettings networkTickSettings,
			UniversalTemplateEntityManager entityManager,
			IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
			string relevantWorldID)
		{
			if (message.PacketType != typeof(EntityPosition2DDeltasPacket))
				return false;

			var packet = new EntityPosition2DDeltasPacket();

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

				if (!entity.Has<ServerPosition2DComponent>())
					continue;

				ref var serverPosition2DComponent = ref entity.Get<ServerPosition2DComponent>();

				if (serverPosition2DComponent.ServerTick > packet.ServerTick
					&& (serverPosition2DComponent.ServerTick - packet.ServerTick) < networkTickSettings.MaxTickValue / 2)
					continue;

				serverPosition2DComponent.ServerPosition = idPositionPair.Position;

				serverPosition2DComponent.ServerTick = packet.ServerTick;

				serverPosition2DComponent.Dirty = true;
			}

			return true;
		}
	}
}
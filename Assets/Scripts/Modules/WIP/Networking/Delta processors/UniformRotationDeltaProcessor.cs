using System;

using HereticalSolutions.Repositories;

using HereticalSolutions.Networking;
using HereticalSolutions.Networking.LiteNetLib;

using HereticalSolutions.Templates.Universal.Networking;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
	public class UniformRotationDeltaProcessor
		: IDeltaPacketProcessor
	{
		public bool ProcessDeltasPacket(
			ClientReceivedPacketMessage message,
			NetworkTickSettings networkTickSettings,
			UniversalTemplateEntityManager entityManager,
			IOneToOneMap<ushort, Guid> networkIDToEntityIDMap,
			string relevantWorldID)
		{
			if (message.PacketType != typeof(EntityUniformRotationDeltasPacket))
				return false;

			var packet = new EntityUniformRotationDeltasPacket();

			packet.Deserialize(message.Reader);


			foreach (var idRotationPair in packet.UniformRotations)
			{
				if (!networkIDToEntityIDMap.TryGetRight(
					idRotationPair.NetworkID,
					out var entityID))
					continue;

				var entity = entityManager.GetEntity(
					entityID,
					relevantWorldID);

				if (!entity.Has<ServerUniformRotationComponent>())
					continue;

				ref var serverRotationComponent = ref entity.Get<ServerUniformRotationComponent>();

				if (serverRotationComponent.ServerTick > packet.ServerTick
					&& (serverRotationComponent.ServerTick - packet.ServerTick) < networkTickSettings.MaxTickValue / 2)
					continue;

				serverRotationComponent.ServerRotation = idRotationPair.Rotation;

				serverRotationComponent.ServerTick = packet.ServerTick;

				serverRotationComponent.Dirty = true;
			}

			return true;
		}
	}
}
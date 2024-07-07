using DefaultEcs;

using HereticalSolutions.Templates.Universal.Networking;

using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
	public class Position3DSerializer
		: IComponentSerializer
	{
		public Vector3 Position;

		public ESerializedEntityType SerializedEntityType
		{
			get => ESerializedEntityType.SIMULATION;
		}

		public void Serialize(NetDataWriter writer)
		{
			writer.Put(Position);
		}

		public void Deserialize(NetDataReader reader)
		{
			Position = reader.GetVector3();
		}

		public bool ReadFrom(
			Entity entity,
			ComponentSerializationContext context)
		{
			if (!entity.Has<Position3DComponent>())
				return false;

			var position3DComponent = entity.Get<Position3DComponent>();

			Position = position3DComponent.Position;

			return true;
		}

		public bool WriteTo(
			Entity entity,
			ComponentSerializationContext context)
		{
			if (entity.Has<Position3DComponent>())
			{
				ref var position3DComponent = ref entity.Get<Position3DComponent>();

				position3DComponent.Position = Position;
			}
			else
			{
				entity.Set<Position3DComponent>(
					new Position3DComponent
					{
						Position = this.Position
					});
			}

			entity.Set<ServerPosition3DComponent>(
				new ServerPosition3DComponent
				{
					ServerPosition = this.Position
				});

			return true;
		}

		public void Clear()
		{
			Position = default;
		}
	}
}
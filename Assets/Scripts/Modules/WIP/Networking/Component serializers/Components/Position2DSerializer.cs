using DefaultEcs;

using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
	/*
	public class Position2DSerializer
		: IComponentSerializer
	{
		public Vector2 Position;

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
			Position = reader.GetVector2();
		}

		public bool ReadFrom(
			Entity entity,
			ComponentSerializationContext context)
		{
			if (!entity.Has<Position2DComponent>())
				return false;

			var position2DComponent = entity.Get<Position2DComponent>();

			Position = position2DComponent.Position;

			return true;
		}

		public bool WriteTo(
			Entity entity,
			ComponentSerializationContext context)
		{
			if (entity.Has<Position2DComponent>())
			{
				ref var position2DComponent = ref entity.Get<Position2DComponent>();

				position2DComponent.Position = Position;
			}
			else
			{
				entity.Set<Position2DComponent>(
					new Position2DComponent
					{
						Position = this.Position
					});
			}

			entity.Set<ServerPosition2DComponent>(
				new ServerPosition2DComponent
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
	*/
}
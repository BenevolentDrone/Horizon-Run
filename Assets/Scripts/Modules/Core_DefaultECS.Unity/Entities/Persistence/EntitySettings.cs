using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	/// <summary>
	/// Represents the settings for an entity.
	/// </summary>
	[CreateAssetMenu(fileName = "Entity settings", menuName = "Settings/Entities/Entity settings", order = 0)]
	public class EntitySettings : ScriptableObject
	{
		private ISerializer serializer;

		/// <summary>
		/// The JSON string representation of the entity.
		/// </summary>
		public string EntityJson;

		/// <summary>
		/// The EntityPrototypeDTO object representing the entity's prototype.
		/// </summary>
		public EntityPrototypeDTO GetPrototypeDTO(
			ILoggerResolver loggerResolver)
		{
			if (serializer == null)
			{
				var serializerBuilder = PersistenceFactory.BuildSerializerBuilder(
					loggerResolver);

				serializer = serializerBuilder
					.NewSerializer()
					.ToJSON()
					.AsString()
					.BuildSerializer();
			}

			var stringStrategy = serializer.Context.SerializationStrategy as StringStrategy;

			stringStrategy.Value = EntityJson;

			bool success = serializer.Deserialize(
				typeof(EntityPrototypeDTO),
				out object newEntityDTO);

			return (EntityPrototypeDTO)newEntityDTO;
		}
	}
}
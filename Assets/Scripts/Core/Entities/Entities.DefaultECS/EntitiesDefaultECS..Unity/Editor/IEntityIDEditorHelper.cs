using System;
using System.Collections.Generic;

using DefaultEcs;

namespace HereticalSolutions.Entities.Editor
{
	public interface IEntityIDEditorHelper
	{
		bool TryGetEntityManager(out object entityManager);

		bool TryGetEntityID(
			Entity entity,
			out object entityIDObject);

		Entity GetRegistryEntity(
			object entityManagerObject,
			object entityIDObject);

		Entity GetEntity(
			object entityManagerObject,
			object entityIDObject,
			string worldID);

		IEnumerable<Guid> GetAllRegistryEntityIDs(object entityManagerObject);
	}
}
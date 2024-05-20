using System;
using System.Collections.Generic;

using DefaultEcs;
using HereticalSolutions.Samples.ECSCharacterControllerSample;
using Zenject;

namespace HereticalSolutions.Entities.Editor
{
	public class SampleEditorWithGUIDHelper
		: IEntityIDEditorHelper
	{
		public bool TryGetEntityManager(out object entityManager)
		{
			var contextRegistry = ProjectContext.Instance.Container.Resolve<SceneContextRegistry>();

			foreach (var sceneContext in contextRegistry.SceneContexts)
			{
				entityManager = sceneContext
					.Container
					.TryResolve<SampleEntityManager>();

				if (entityManager != null)
					return true;
			}

			entityManager = null;

			return false;
		}

		public bool TryGetEntityID(
			Entity entity,
			out object entityIDObject)
		{
			if (!entity.Has<GUIDComponent>())
			{
				entityIDObject = null;

				return false;
			}

			entityIDObject = entity.Get<GUIDComponent>().GUID;

			return true;
		}

		public Entity GetRegistryEntity(
			object entityManagerObject,
			object entityIDObject)
		{
			var entityManager = entityManagerObject as DefaultECSEntityManager<Guid>;

			var entityID = (Guid)entityIDObject;

			return entityManager.GetRegistryEntity(
				entityID);
		}

		public Entity GetEntity(
			object entityManagerObject,
			object entityIDObject,
			string worldID)
		{
			var entityManager = entityManagerObject as DefaultECSEntityManager<Guid>;

			var entityID = (Guid)entityIDObject;

			return entityManager.GetEntity(
				entityID,
				worldID);
		}

		public IEnumerable<Guid> GetAllRegistryEntityIDs(object entityManagerObject)
		{
			var entityManager = entityManagerObject as DefaultECSEntityManager<Guid>;

			if (entityManager == null)
				return null;
			
			return entityManager.AllAllocatedIDs;
		}
	}
}
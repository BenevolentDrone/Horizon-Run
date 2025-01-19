using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class ResolveEntitiesOnSceneInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private EntityAuthoringSettings entityAuthoringSettings;

		[Inject]
		private EntityManager entityManager;

		public override void InstallBindings()
		{
			var logger = loggerResolver?.GetLogger(
				GetType());

			var gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Entity");

			foreach (var gameObjectWithTag in gameObjectsWithTag)
			{
				if (gameObjectWithTag.activeInHierarchy == false)
					continue;

				var sceneEntity = gameObjectWithTag.GetComponent<SceneEntity>();

				var worldLocalSceneEntity = gameObjectWithTag.GetComponent<WorldLocalSceneEntity>();

				if (sceneEntity == null
					&& worldLocalSceneEntity == null)
				{
					logger?.LogError(
						GetType(),
						$"GAME OBJECT {gameObjectWithTag.name} HAS AN ENTITY TAG BUT NEITHER SceneEntity NOR WorldLocalSceneEntity COMPONENT",
						new object[]
						{
							gameObjectWithTag
						});

					continue;
				}

				logger?.Log(
					GetType(),
					$"RESOLVING GAME OBJECT {gameObjectWithTag.name}",
					new object[]
					{
						gameObjectWithTag
					});

				if (sceneEntity != null)
				{
					ResolveChildren(
						sceneEntity,
						logger);

					entityManager.ResolveEntity(
						sceneEntity.EntityID,
						gameObjectWithTag,
						sceneEntity.PrototypeID,
						entityAuthoringSettings.AuthoringPreset);
				}

				if (worldLocalSceneEntity != null)
				{
					ResolveChildren(
						worldLocalSceneEntity,
						logger);

					entityManager.ResolveWorldLocalEntity(
						out var _,
						worldLocalSceneEntity.PrototypeID,
						gameObjectWithTag,
						worldLocalSceneEntity.WorldID);
				}
			}
		}

		private void ResolveChildren(
			ASceneEntity parentSceneEntity,
			ILogger logger)
		{
			if (parentSceneEntity.ChildEntities == null)
				return;

			foreach (var childSceneEntityDescriptor in parentSceneEntity.ChildEntities)
			{
				var childSceneEntity = childSceneEntityDescriptor.SceneEntity;

				if (childSceneEntity.gameObject.activeInHierarchy == false)
					continue;

				logger?.Log(
					GetType(),
					$"RESOLVING GAME OBJECT {childSceneEntity.gameObject.name}",
					new object[]
					{
						childSceneEntity.gameObject
					});

				var concreteSceneEntity = childSceneEntity as SceneEntity;

				if (concreteSceneEntity != null)
				{
					ResolveChildren(
						concreteSceneEntity,
						logger);

					entityManager.ResolveEntity(
						concreteSceneEntity.EntityID,
						concreteSceneEntity.gameObject,
						concreteSceneEntity.PrototypeID,
						entityAuthoringSettings.AuthoringPreset);
				}

				var childWorldLocalSceneEntity = childSceneEntity as WorldLocalSceneEntity;

				if (childWorldLocalSceneEntity != null)
				{
					ResolveChildren(
						childWorldLocalSceneEntity,
						logger);

					entityManager.ResolveWorldLocalEntity(
						out var _,
						childWorldLocalSceneEntity.PrototypeID,
						childWorldLocalSceneEntity.gameObject,
						childWorldLocalSceneEntity.WorldID);
				}
			}

		}
	}
}
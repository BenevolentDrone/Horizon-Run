using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleResolveEntitiesOnSceneInstaller : MonoInstaller
	{
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private SampleEntityManager entityManager;

		public override void InstallBindings()
		{
			var logger = loggerResolver?.GetLogger<SampleResolveEntitiesOnSceneInstaller>();

			var gameObjectsWithTag = GameObject.FindGameObjectsWithTag("Entity");

			foreach (var gameObjectWithTag in gameObjectsWithTag)
			{
				var sceneEntity = gameObjectWithTag.GetComponent<SampleSceneEntity>();

				var worldLocalSceneEntity = gameObjectWithTag.GetComponent<WorldLocalSceneEntity>();

				if (sceneEntity == null
					&& worldLocalSceneEntity == null)
				{
					logger?.LogError<SampleResolveEntitiesOnSceneInstaller>(
						$"GAME OBJECT {gameObjectWithTag.name} HAS AN ENTITY TAG BUT NEITHER SampleSceneEntity NOR WorldLocalSceneEntity COMPONENT",
						new object[]
						{
							gameObjectWithTag
						});

					continue;
				}

				logger?.Log<SampleResolveEntitiesOnSceneInstaller>(
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
						sceneEntity.PrototypeID);
				}

				if (worldLocalSceneEntity != null)
				{
					ResolveChildren(
						worldLocalSceneEntity,
						logger);

					entityManager.ResolveWorldLocalEntity(
						worldLocalSceneEntity.PrototypeID,
						gameObjectWithTag,
						worldLocalSceneEntity.WorldID);
				}
			}
		}

		private void ResolveChildren(
			ASceneEntity parentSceneEntity,
			ILogger logger = null)
		{
			if (parentSceneEntity.ChildEntities == null)
				return;

			foreach (var childSceneEntityDescriptor in parentSceneEntity.ChildEntities)
			{
				var childSceneEntity = childSceneEntityDescriptor.SceneEntity;

				logger?.Log<SampleResolveEntitiesOnSceneInstaller>(
					$"RESOLVING GAME OBJECT {childSceneEntity.gameObject.name}",
					new object[]
					{
						childSceneEntity.gameObject
					});

				var childSampleSceneEntity = childSceneEntity as SampleSceneEntity;

				if (childSampleSceneEntity != null)
				{
					ResolveChildren(
						childSampleSceneEntity,
						logger);

					entityManager.ResolveEntity(
						childSampleSceneEntity.EntityID,
						childSceneEntity.gameObject,
						childSampleSceneEntity.PrototypeID);
				}

				var childWorldLocalSceneEntity = childSceneEntity as WorldLocalSceneEntity;

				if (childWorldLocalSceneEntity != null)
				{
					ResolveChildren(
						childWorldLocalSceneEntity,
						logger);

					entityManager.ResolveWorldLocalEntity(
						childWorldLocalSceneEntity.PrototypeID,
						childWorldLocalSceneEntity.gameObject,
						childWorldLocalSceneEntity.WorldID);
				}
			}

		}
	}
}
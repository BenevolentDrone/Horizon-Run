using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
	/*
	public class LinkSubSpawnersResolveSystem<TSceneEntity>
		: IEntityInitializationSystem
		  where TSceneEntity : MonoBehaviour
	{
		private readonly UniversalTemplateEntityManager entityManager;

		private readonly DefaultECSEntityHierarchyManager entityHierarchyManager;

		private readonly ILogger logger;

		public LinkSubSpawnersResolveSystem(
			UniversalTemplateEntityManager entityManager,
			DefaultECSEntityHierarchyManager entityHierarchyManager,
			ILogger logger)
		{
			this.entityManager = entityManager;

			this.entityHierarchyManager = entityHierarchyManager;
			
			this.logger = logger;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<ResolveComponent>())
				return;

			if (!entity.Has<InvokeSubSpawnersComponent>())
				return;


			ref ResolveComponent ResolveComponent = ref entity.Get<ResolveComponent>();

			ref InvokeSubSpawnersComponent invokeSubSpawnersComponent = ref entity.Get<InvokeSubSpawnersComponent>();


			GameObject source = ResolveComponent.Source as GameObject;

			if (source == null)
				return;

			UniversalTemplateSceneEntity sceneEntity = source.GetComponentInChildren<UniversalTemplateSceneEntity>();

			if (sceneEntity != null)
			{
				var childrenDescriptors = sceneEntity.ChildEntities;

				invokeSubSpawnersComponent.SubSpawners = new Entity[childrenDescriptors.Count];

				for (int i = 0; i < childrenDescriptors.Count; i++)
				{
					var childDescriptor = childrenDescriptors[i];

					var childSceneEntity = childDescriptor.SceneEntity;

					if (childSceneEntity is not UniversalTemplateSceneEntity concreteChildSceneEntity)
						continue;

					var childID = concreteChildSceneEntity.EntityID;

					var subSpawner = entityManager.GetEntity(
						childID,
						WorldConstants.SIMULATION_WORLD_ID);

					invokeSubSpawnersComponent.SubSpawners[i] = subSpawner;
					
					EntityHierarchyHelpers.AddChild(
						entity,
						subSpawner,
						entityHierarchyManager,
						logger);
				}
			}
		}

		/// <summary>
		/// Disposes the system.
		/// </summary>
		public void Dispose()
		{
		}
	}
	*/
}
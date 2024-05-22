using HereticalSolutions.Entities;

using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

namespace HereticalSolutions.HorizonRun
{
	public class LinkSubSpawnersResolveSystem<TSceneEntity>
		: IDefaultECSEntityInitializationSystem
		  where TSceneEntity : MonoBehaviour
	{
		private readonly HorizonRunEntityManager entityManager;

		private readonly DefaultECSEntityListManager entityListManager;

		private readonly ILogger logger;

		public LinkSubSpawnersResolveSystem(
			HorizonRunEntityManager entityManager,
			DefaultECSEntityListManager entityListManager,
			ILogger logger = null)
		{
			this.entityManager = entityManager;

			this.entityListManager = entityListManager;
			
			this.logger = logger;
		}

		//Required by ISystem
		public bool IsEnabled { get; set; } = true;

		public void Update(Entity entity)
		{
			if (!IsEnabled)
				return;

			if (!entity.Has<ResolveSimulationComponent>())
				return;

			if (!entity.Has<InvokeSubSpawnersComponent>())
				return;


			ref ResolveSimulationComponent resolveSimulationComponent = ref entity.Get<ResolveSimulationComponent>();

			ref InvokeSubSpawnersComponent invokeSubSpawnersComponent = ref entity.Get<InvokeSubSpawnersComponent>();


			GameObject source = resolveSimulationComponent.Source as GameObject;

			if (source == null)
				return;

			HorizonRunSceneEntity sceneEntity = source.GetComponentInChildren<HorizonRunSceneEntity>();

			if (sceneEntity != null)
			{
				var childrenDescriptors = sceneEntity.ChildEntities;

				invokeSubSpawnersComponent.SubSpawners = new Entity[childrenDescriptors.Count];

				for (int i = 0; i < childrenDescriptors.Count; i++)
				{
					var childDescriptor = childrenDescriptors[i];

					var childSceneEntity = childDescriptor.SceneEntity;

					if (childSceneEntity is not HorizonRunSceneEntity concreteChildSceneEntity)
						continue;

					var childID = concreteChildSceneEntity.EntityID;

					var subSpawner = entityManager.GetEntity(
						childID,
						WorldConstants.SIMULATION_WORLD_ID);

					invokeSubSpawnersComponent.SubSpawners[i] = subSpawner;
					
					HierarchyHelpers.AddChild(
						entity,
						subSpawner,
						entityListManager,
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
}

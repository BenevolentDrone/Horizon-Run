using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;


using TWorldID = System.String;

using TWorld = DefaultEcs.World;

using TPrototypeID = System.String;

using TEntityID = System.Guid;

using TEntity = DefaultEcs.Entity;


namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class EntityManager
		: IMultiWorldEntityManager<
			TWorldID,
			TWorld,
			TPrototypeID,
			TEntityID,
			TEntity>
	{
		private readonly IRepository<TEntityID, TEntity> registryEntityRepository;

		private readonly IReadOnlyEntityWorldRepository<TWorldID, TWorld, TEntity> entityWorldRepository;

		private readonly TWorldID[] worldsToSpawnEntitiesIn;

		private readonly ILogger logger;

		public EntityManager(
			IRepository<TEntityID, TEntity> registryEntityRepository,
			IReadOnlyEntityWorldRepository<TWorldID, TWorld, TEntity> entityWorldRepository,
			TWorldID[] worldsToSpawnEntitiesIn,
			ILogger logger = null)
		{
			this.registryEntityRepository = registryEntityRepository;

			this.entityWorldRepository = entityWorldRepository;

			this.worldsToSpawnEntitiesIn = worldsToSpawnEntitiesIn;

			this.logger = logger;
		}

		#region IReadOnlyEntityManager

		public bool HasEntity(
			TEntityID entityID)
		{
			return registryEntityRepository.Has(entityID);
		}

		public bool TryGetEntity(
			TEntityID entityID,
			out TEntity entity)
		{
			return TryGetRegistryEntity(
				entityID,
				out entity);
		}

		public IEnumerable<TEntityID> AllAllocatedIDs => registryEntityRepository.Keys;

		#endregion

		#region IReadOnlyMultiWorldEntityManager

		public bool HasEntity(
			TEntityID entityID,
			TWorldID worldID)
		{
			return TryGetEntity(
				entityID,
				worldID,
				out _);
		}

		public bool TryGetRegistryEntity(
			TEntityID entityID,
			out TEntity registryEntity)
		{
			return registryEntityRepository.TryGet(
				entityID,
				out registryEntity);
		}

		public bool TryGetEntity(
			TEntityID entityID,
			TWorldID worldID,
			out TEntity entity)
		{
			if (!registryEntityRepository.TryGet(
				entityID,
				out var registryEntity))
			{
				entity = default(TEntity);

				return false;
			}

			if (!entityWorldRepository.TryGetEntityWorldController(
				worldID,
				out var worldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER FOUND FOR WORLD {worldID}");

				entity = default(TEntity);

				return false;
			}

			var worldControllerDowncasted = worldController
				as IEntityWorldControllerWithRegistry<TWorld, TPrototypeID, TEntity>;

			if (worldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"WORLD CONTROLLER FOR WORLD {worldID} CANNOT BE CAST TO IEntityWorldControllerWithRegistry");

				entity = default(TEntity);

				return false;
			}

			return worldControllerDowncasted.TryGetEntityFromRegistry(
				registryEntity,
				out entity);
		}

		public IEnumerable<TEntity> AllRegistryEntities => registryEntityRepository.Values;

		#endregion

		#region IEntityManager

		#region Spawn entity

		public bool SpawnEntity(
			out TEntityID entityID,
			TPrototypeID prototypeID,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			entityID = AllocateID();

			if (!SpawnEntityInAllRelevantWorlds(
					entityID,
					prototypeID,
					null,
					authoringPreset))
				{
					entityID = default;

					return false;
				}

			logger?.Log(
				GetType(),
				$"SPAWNED ENTITY WITH ID {entityID} AND PROTOTYPE ID {prototypeID}");

			return true;
		}

		public bool SpawnEntity(
			TEntityID entityID,
			TPrototypeID prototypeID,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			return SpawnEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				null,
				authoringPreset);
		}

		#endregion

		#region Resolve entity

		public bool ResolveEntity(
			out TEntityID entityID,
			object source,
			TPrototypeID prototypeID,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			entityID = AllocateID();

			if (!SpawnAndResolveEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				null,
				source,
				authoringPreset))
			{
				entityID = default;

				return false;
			}

			logger?.Log(
				GetType(),
				$"RESOLVED ENTITY WITH ID {entityID} AND PROTOTYPE ID {prototypeID} FROM SOURCE",
				new object[]
				{
					source
				});

			return true;
		}

		public bool ResolveEntity(
			TEntityID entityID,
			object source,
			TPrototypeID prototypeID,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			if (registryEntityRepository.Has(entityID))
			{
				logger?.LogError(
					GetType(),
					$"ENTITY WITH ID {entityID} ALREADY PRESENT");

				return false;
			}

			return SpawnAndResolveEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				null,
				source,
				authoringPreset);
		}

		#endregion

		#region Despawn entity

		public bool DespawnEntity(
			TEntityID entityID)
		{
			if (!registryEntityRepository.TryGet(
				entityID,
				out var registryEntity))
			{
				return false;
			}

			foreach (var entityWorldID in worldsToSpawnEntitiesIn)
			{
				if (!entityWorldRepository.TryGetEntityWorldController(
					entityWorldID,
					out var worldController))
				{
					logger?.LogError(
						GetType(),
						$"NO WORLD CONTROLLER FOUND FOR WORLD {entityWorldID}");

					return false;
				}

				var worldControllerDowncasted = worldController
					as IEntityWorldControllerWithRegistry<TWorld, TPrototypeID, TEntity>;

				if (worldControllerDowncasted == null)
				{
					logger?.LogError(
						GetType(),
						$"WORLD CONTROLLER FOR WORLD {entityWorldID} CANNOT BE CAST TO IEntityWorldControllerWithRegistry");

					return false;
				}

				worldControllerDowncasted.DespawnEntityAndUnlinkFromRegistry(
					registryEntity);
			}

			if (!entityWorldRepository.TryGetEntityWorldController(
				WorldConstants.REGISTRY_WORLD_ID,
				out var registryWorldController))
			{
				logger?.LogError(
					GetType(),
					$"NO REGISTRY WORLD CONTROLLER FOUND");

				return false;
			}

			registryWorldController.DespawnEntity(
				registryEntity);

			registryEntityRepository.Remove(
				entityID);

			logger?.Log(
				GetType(),
				$"DESPAWNED ENTITY {entityID}");

			return true;
		}

		#endregion

		#endregion

		#region IMultiWorldEntityManager

		#region Spawn entity

		public bool SpawnEntity(
			out TEntityID entityID,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			entityID = AllocateID();

			if (!SpawnEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				overrides,
				authoringPreset))
			{
				entityID = default;

				return false;
			}

			logger?.Log(
				GetType(),
				$"SPAWNED ENTITY WITH ID {entityID} AND PROTOTYPE ID {prototypeID}");

			return true;
		}

		public bool SpawnEntity(
			TEntityID entityID,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			return SpawnEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				overrides,
				authoringPreset);
		}

		public bool SpawnWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			TWorldID worldID)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				worldID,
				out var worldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER FOUND FOR WORLD {worldID}");

				entity = default(TEntity);

				return false;
			}

			var worldControllerDowncasted = worldController
				as IEntityWorldControllerWithPrototypes<TWorldID, TPrototypeID, TEntity>;

			if (worldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"WORLD CONTROLLER FOR WORLD {worldID} CANNOT BE CAST TO IEntityWorldControllerWithPrototypes");

				entity = default(TEntity);

				return false;
			}

			worldControllerDowncasted.TrySpawnEntityFromPrototype(
				prototypeID,
				out entity);

			logger?.Log(
				GetType(),
				$"SPAWNED WORLD LOCAL ENTITY {entity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID}");

			return true;
		}

		public bool SpawnWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			TEntity @override,
			TWorldID worldID)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				worldID,
				out var worldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER FOUND FOR WORLD {worldID}");

				entity = default(TEntity);

				return false;
			}

			var worldControllerDowncasted = worldController
				as IEntityWorldControllerWithPrototypes<TWorldID, TPrototypeID, TEntity>;

			if (worldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"WORLD CONTROLLER FOR WORLD {worldID} CANNOT BE CAST TO IEntityWorldControllerWithPrototypes");

				entity = default(TEntity);

				return false;
			}

			worldControllerDowncasted.TrySpawnEntityFromPrototype(
				prototypeID,
				@override,
				out entity);

			logger?.Log(
				GetType(),
				$"SPAWNED WORLD LOCAL ENTITY {entity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID}");

			return true;
		}

		#endregion

		#region Resolve entity

		public bool ResolveEntity(
			out TEntityID entityID,
			object source,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			entityID = AllocateID();

			if (!SpawnAndResolveEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				overrides,
				source,
				authoringPreset))
			{
				entityID = default;

				return false;
			}

			logger?.Log(
				GetType(),
				$"RESOLVED ENTITY WITH ID {entityID} AND PROTOTYPE ID {prototypeID} FROM SOURCE",
				new object[]
				{
					source
				});

			return true;
		}

		public bool ResolveEntity(
			TEntityID entityID,
			object source,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			if (registryEntityRepository.Has(entityID))
			{
				logger?.LogError(
					GetType(),
					$"ENTITY WITH ID {entityID} ALREADY PRESENT");

				return false;
			}

			return SpawnAndResolveEntityInAllRelevantWorlds(
				entityID,
				prototypeID,
				overrides,
				source,
				authoringPreset);
		}

		public bool ResolveWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			object source,
			TWorldID worldID)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				worldID,
				out var worldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER FOUND FOR WORLD {worldID}");

				entity = default(TEntity);

				return false;
			}

			var worldControllerDowncasted = worldController
				as IEntityWorldControllerWithPrototypes<TWorldID, TPrototypeID, TEntity>;

			if (worldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"WORLD CONTROLLER FOR WORLD {worldID} CANNOT BE CAST TO IEntityWorldControllerWithPrototypes");

				entity = default(TEntity);

				return false;
			}

			worldControllerDowncasted.TrySpawnAndResolveEntityFromPrototype(
				prototypeID,
				source,
				out entity);

			logger?.Log(
				GetType(),
				$"RESOLVED WORLD LOCAL ENTITY {entity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID} FROM SOURCE",
				new object[]
				{
					source
				});

			return true;
		}

		public bool ResolveWorldLocalEntity(
			out TEntity entity,
			TPrototypeID prototypeID,
			object source,
			TEntity @override,
			TWorldID worldID)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				worldID,
				out var worldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER FOUND FOR WORLD {worldID}");

				entity = default(TEntity);

				return false;
			}

			var worldControllerDowncasted = worldController
				as IEntityWorldControllerWithPrototypes<TWorldID, TPrototypeID, TEntity>;

			if (worldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"WORLD CONTROLLER FOR WORLD {worldID} CANNOT BE CAST TO IEntityWorldControllerWithPrototypes");

				entity = default(TEntity);

				return false;
			}

			worldControllerDowncasted.TrySpawnAndResolveEntityFromPrototype(
				prototypeID,
				@override,
				source,
				out entity);

			logger?.Log(
				GetType(),
				$"RESOLVED WORLD LOCAL ENTITY {entity} AT WORLD {worldID} WITH PROTOTYPE ID {prototypeID} FROM SOURCE",
				new object[]
				{
					source
				});

			return true;
		}

		#endregion

		#region Despawn entity

		public bool DespawnWorldLocalEntity(
			TEntity entity)
		{
			if (entity == default)
				return false;

			if (!entityWorldRepository.TryGetEntityWorldController(
				entity.World,
				out var worldController))
			{
				logger?.LogError(
					GetType(),
					$"NO WORLD CONTROLLER FOUND FOR WORLD {entity.World}");

				return false;
			}

			worldController.DespawnEntity(
				entity);

			logger?.Log(
				GetType(),
				$"DESPAWNED WORLD LOCAL ENTITY {entity}");

			return true;
		}

		#endregion

		#endregion

		private TEntityID AllocateID()
		{
            TEntityID newID;

            do
            {
                newID = IDAllocationsFactory.BuildGUID();
            }
            while (registryEntityRepository.Has(newID));

            return newID;
		}

		private bool SpawnEntityInAllRelevantWorlds(
			TEntityID entityID,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			EEntityAuthoringPresets authoringPreset = EEntityAuthoringPresets.DEFAULT)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				WorldConstants.REGISTRY_WORLD_ID,
				out var registryWorldController))
			{
				logger?.LogError(
					GetType(),
					$"NO REGISTRY WORLD CONTROLLER FOUND");

				return false;
			}

			var registryWorldControllerDowncasted = registryWorldController
				as IEntityWorldControllerWithPrototypesAndIDs<TWorld, TPrototypeID, TEntityID, TEntity>;

			if (registryWorldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"REGISTRY WORLD CONTROLLER CANNOT BE CAST TO IEntityWorldControllerWithPrototypesAndIDs");

				return false;
			}

			if (!registryWorldControllerDowncasted.TrySpawnEntityWithIDFromPrototype(
				prototypeID,
				entityID,
				out var registryEntity))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO SPAWN ENTITY WITH ID {entityID} AND PROTOTYPE ID {prototypeID}");

				return false;
			}

			registryEntityRepository.Add(
				entityID,
				registryEntity);

			switch (authoringPreset)
			{
				case EEntityAuthoringPresets.NONE:
					break;

				case EEntityAuthoringPresets.DEFAULT:
				case EEntityAuthoringPresets.NETWORKING_HOST: //TODO: change
				case EEntityAuthoringPresets.NETWORKING_CLIENT: //TODO: change
				case EEntityAuthoringPresets.NETWORKING_HOST_HEADLESS: //TODO: change
				{
					foreach (var entityWorldID in worldsToSpawnEntitiesIn)
					{
						if (!entityWorldRepository.TryGetEntityWorldController(
							entityWorldID,
							out var worldController))
						{
							logger?.LogError(
								GetType(),
								$"NO WORLD CONTROLLER FOUND FOR WORLD {entityWorldID}");

							return false;
						}

						var worldControllerDowncasted = worldController
							as IEntityWorldControllerWithRegistry<TWorld, TPrototypeID, TEntity>;

						if (worldControllerDowncasted == null)
						{
							logger?.LogError(
								GetType(),
								$"WORLD CONTROLLER FOR WORLD {entityWorldID} CANNOT BE CAST TO IEntityWorldControllerWithRegistry");

							return false;
						}

						bool spawnWithOverrideAttempted = false;

						if (overrides != null)
						{
							for (int i = 0; i < overrides.Length; i++)
							{
								if (string.IsNullOrEmpty(overrides[i].WorldID))
									continue;
	
								if (overrides[i].WorldID != entityWorldID)
									continue;
								
								spawnWithOverrideAttempted = true;
	
								worldControllerDowncasted.TrySpawnEntityFromRegistry(
									registryEntity,
									overrides[i].OverrideEntity,
									out var localEntity);
	
								break;
							}
						}

						if (!spawnWithOverrideAttempted)
						{
							worldControllerDowncasted.TrySpawnEntityFromRegistry(
								registryEntity,
								out var localEntity);
						}
					}

						break;
				}

				default:
					break;
			}

			return true;
		}

		private bool SpawnAndResolveEntityInAllRelevantWorlds(
			TEntityID entityID,
			TPrototypeID prototypeID,
			WorldOverrideDescriptor<TWorldID, TEntity>[] overrides,
			object source,
			EEntityAuthoringPresets authoring = EEntityAuthoringPresets.DEFAULT)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				WorldConstants.REGISTRY_WORLD_ID,
				out var registryWorldController))
			{
				logger?.LogError(
					GetType(),
					$"NO REGISTRY WORLD CONTROLLER FOUND");

				return false;
			}

			var registryWorldControllerDowncasted = registryWorldController
				as IEntityWorldControllerWithPrototypesAndIDs<TWorld, TPrototypeID, TEntityID, TEntity>;

			if (registryWorldControllerDowncasted == null)
			{
				logger?.LogError(
					GetType(),
					$"REGISTRY WORLD CONTROLLER CANNOT BE CAST TO IEntityWorldControllerWithPrototypesAndIDs");

				return false;
			}

			if (!registryWorldControllerDowncasted.TrySpawnEntityWithIDFromPrototype(
				prototypeID,
				entityID,
				out var registryEntity))
			{
				logger?.LogError(
					GetType(),
					$"FAILED TO SPAWN ENTITY WITH ID {entityID} AND PROTOTYPE ID {prototypeID}");

				return false;
			}

			registryEntityRepository.Add(
				entityID,
				registryEntity);

			switch (authoring)
			{
				case EEntityAuthoringPresets.NONE:
					break;

				case EEntityAuthoringPresets.DEFAULT:
				case EEntityAuthoringPresets.NETWORKING_HOST: //TODO: change
				case EEntityAuthoringPresets.NETWORKING_CLIENT: //TODO: change
				case EEntityAuthoringPresets.NETWORKING_HOST_HEADLESS: //TODO: change
				{
					foreach (var entityWorldID in worldsToSpawnEntitiesIn)
					{
						if (!entityWorldRepository.TryGetEntityWorldController(
							entityWorldID,
							out var worldController))
						{
							logger?.LogError(
								GetType(),
								$"NO WORLD CONTROLLER FOUND FOR WORLD {entityWorldID}");

							return false;
						}

						var worldControllerDowncasted = worldController
							as IEntityWorldControllerWithRegistry<TWorld, TPrototypeID, TEntity>;

						if (worldControllerDowncasted == null)
						{
							logger?.LogError(
								GetType(),
								$"WORLD CONTROLLER FOR WORLD {entityWorldID} CANNOT BE CAST TO IEntityWorldControllerWithRegistry");

							return false;
						}

						bool spawnWithOverrideAttempted = false;

						if (overrides != null)
						{
							for (int i = 0; i < overrides.Length; i++)
							{
								if (string.IsNullOrEmpty(overrides[i].WorldID))
									continue;

								if (overrides[i].WorldID != entityWorldID)
									continue;

								spawnWithOverrideAttempted = true;

								worldControllerDowncasted.TrySpawnAndResolveEntityFromRegistry(
									registryEntity,
									overrides[i].OverrideEntity,
									source,
									out var localEntity);

								break;
							}
						}

						if (!spawnWithOverrideAttempted)
						{
							worldControllerDowncasted.TrySpawnAndResolveEntityFromRegistry(
								registryEntity,
								source,
								out var localEntity);
						}
					}

					break;
				}

				default:
					break;
			}

			return true;
		}
	}
}
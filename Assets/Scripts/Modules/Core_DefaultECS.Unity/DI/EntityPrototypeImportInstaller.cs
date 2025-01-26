/*
using HereticalSolutions.AssetImport;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;


using UnityEngine;

using DefaultEcs;

using Zenject;


using TWorldID = System.String;

using TWorld = DefaultEcs.World;

using TPrototypeID = System.String;

using TEntity = DefaultEcs.Entity;
using HereticalSolutions.Modules.Core_DefaultECS.Factories;

namespace HereticalSolutions.Modules.Core_DefaultECS.DI
{
	public class EntityPrototypeImportInstaller : MonoInstaller
	{
		private const string REGISTRY_ENTITY_VARIANT = "Registry entity";

		private const string SIMULATION_ENTITY_VARIANT = "Simulation entity";

		private const string VIEW_ENTITY_VARIANT = "View entity";

		[Inject]
		private EntityAuthoringSettings entityAuthoringSettings;
		
		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private IAssetImportManager assetImportManager;

		[Inject]
		private IRuntimeResourceManager runtimeResourceManager;

		[Inject]
		private EntityWorldRepository entityWorldRepository;

		[Inject]
		private EntityManager entityManager;

		[SerializeField]
		private ResourcesSettings[] settingsPages;

		public override void InstallBindings()
		{
			var logger = loggerResolver.GetLogger<EntityPrototypeImportInstaller>();

			var authoringPreset = entityAuthoringSettings.AuthoringPreset;
            
			bool includeSimulationWorld = authoringPreset != EEntityAuthoringPresets.NONE;
			
			bool includeViewWorld =
				authoringPreset == EEntityAuthoringPresets.DEFAULT
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_HOST
				|| authoringPreset == EEntityAuthoringPresets.NETWORKING_CLIENT;

			foreach (var settingsPage in settingsPages)
			{
				TaskExtensions.RunSync(
					() => assetImportManager.Import<ResourceImporterFromScriptable>(
							(importer) =>
							{
								importer.Initialize(
									runtimeResourceManager,
									settingsPage);
							})
						.ContinueWith(
							targetTask =>
							{
								logger?.Log(
									GetType(),
									$"CREATING ENTITY PROTOTYPE VISITORS");

								if (!TryGetWorldPrototypeVisitor(
									    entityWorldRepository,
									    WorldConstants.REGISTRY_WORLD_ID,
									    out DefaultECSEntityPrototypeVisitor registryWorldPrototypeVisitor,
									    true,
									    logger))
								{
									logger?.LogError(
										GetType(),
										$"COULD NOT GET registryWorldPrototypeVisitor");

									return;
								}

								DefaultECSEntityPrototypeVisitor simulationWorldPrototypeVisitor = null;

								if (includeSimulationWorld)
								{
									TryGetWorldPrototypeVisitor(
										entityWorldRepository,
										WorldConstants.SIMULATION_WORLD_ID,
										out simulationWorldPrototypeVisitor,
										false,
										logger);
								}


								DefaultECSEntityPrototypeVisitor viewWorldPrototypeVisitor = null;

								if (includeViewWorld)
								{
									TryGetWorldPrototypeVisitor(
										entityWorldRepository,
										WorldConstants.VIEW_WORLD_ID,
										out viewWorldPrototypeVisitor,
										false,
										logger);
								}

								CreateEntityPrototypes(
									settingsPage,

									includeSimulationWorld,
									includeViewWorld,

									registryWorldPrototypeVisitor,
									simulationWorldPrototypeVisitor,
									viewWorldPrototypeVisitor,

									logger);
							}
						));
			}
		}

		private bool TryGetWorldPrototypeVisitor(
			IEntityWorldRepository<TWorldID, TWorld, TEntity> entityWorldRepository,
			TWorldID worldID,

			out DefaultECSEntityPrototypeVisitor worldPrototypeVisitor,

			bool logErrorIfWorldNotFound = true,
			ILogger logger)
		{
			if (!entityWorldRepository.TryGetEntityWorldController(
				worldID,
				out var worldController))
			{
				if (logErrorIfWorldNotFound)
				{
					logger?.LogError(
						GetType(),
						$"entityWorldRepository DOES NOT HAVE {worldID}");
				}

				worldPrototypeVisitor = null;

				return false;
			}

			var worldWithPrototype = worldController as IEntityWorldControllerWithPrototypes<TWorld, TPrototypeID, TEntity>;

			if (worldWithPrototype == null)
			{
				logger?.LogError(
					GetType(),
					$"worldController CANNOT BE CAST TO IEntityWorldControllerWithPrototypes");
				
				worldPrototypeVisitor = null;
				
				return false;
			}
			
			var worldPrototypeRepository = worldWithPrototype.PrototypeRepository;
			
			if (worldPrototypeRepository == null)
			{
				logger?.LogError(
					GetType(),
					$"worldWithPrototype DOES NOT HAVE PrototypeRepository");
				
				worldPrototypeVisitor = null;
				
				return false;
			}

			worldPrototypeVisitor = EntityPersistenceFactory.BuildDefaultECSEntityPrototypeVisitor(
				worldPrototypeRepository,
				loggerResolver);

			return true;
		}

		private void CreateEntityPrototypes(
			ResourcesSettings resourcePage,
			
			bool includeSimulationWorld,
			bool includeViewWorld,
			
			DefaultECSEntityPrototypeVisitor registryWorldPrototypeVisitor,
			DefaultECSEntityPrototypeVisitor simulationWorldPrototypeVisitor,
			DefaultECSEntityPrototypeVisitor viewWorldPrototypeVisitor,
			
			ILogger logger)
		{
			logger?.Log(
				GetType(),
				$"PARSING RESOURCE PAGE: {resourcePage.ResourcePageName}");
			
			foreach (var resource in resourcePage.Resources)
			{
				string entityName = resource.ResourceID;

				logger?.Log(
					GetType(),
					$"CREATING ENTITY PROTOTYPES FOR ENTITY: {entityName}");

				if (!runtimeResourceManager.TryGetResource(
					entityName.SplitAddressBySeparator(),
					out IReadOnlyResourceData entityResourceData))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT FIND RESOURCE FOR ENTITY: {entityName}");

					continue;
				}

				if (!CreateEntityPrototype(
					entityName,
					REGISTRY_ENTITY_VARIANT,
					entityResourceData,
					registryWorldPrototypeVisitor,
					out Entity registryEntityPrototype,
					logger))
				{
					continue;
				}
				
				if (includeSimulationWorld)
				{
					if (!CreateEntityPrototype(
						entityName,
						SIMULATION_ENTITY_VARIANT,
						entityResourceData,
						simulationWorldPrototypeVisitor,
						out Entity simulationEntityPrototype,
						logger))
					{
						continue;
					}
				}
				
				if (includeViewWorld)
				{
					if (!CreateEntityPrototype(
						entityName,
						VIEW_ENTITY_VARIANT,
						entityResourceData,
						viewWorldPrototypeVisitor,
						out Entity viewEntityPrototype,
						logger))
					{
						continue;
					}
				}
			}
		}

		private bool CreateEntityPrototype(
			string entityName,
			string variantID,

			IReadOnlyResourceData entityResourceData,
			DefaultECSEntityPrototypeVisitor registryWorldPrototypeVisitor,

			out TEntity entityPrototype,
			ILogger logger)
		{
			if (entityResourceData.TryGetVariant(
				variantID,
				out IResourceVariantData registryVariant))
			{
				if (!registryWorldPrototypeVisitor.VisitLoad(
					registryVariant
					    .StorageHandle
					    .GetResource<EntitySettings>()
					    .GetPrototypeDTO(loggerResolver),
					out entityPrototype))
				{
					logger?.LogError(
						GetType(),
						$"COULD NOT CREATE {variantID} PROTOTYPE FOR: {entityName}");

					return false;
				}

				logger?.Log(
					GetType(),
					$"CREATED {variantID} PROTOTYPE FOR: {entityName}");

				return true;
			}
			
			logger?.Log(
				GetType(),
				$"NO {variantID} FOUND FOR: {entityName}");
			
			entityPrototype = default;
			
			return true;
		}
	}
}
*/
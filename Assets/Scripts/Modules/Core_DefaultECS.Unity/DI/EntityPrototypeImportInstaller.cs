using HereticalSolutions.AssetImport;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Installers
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
		private UniversalTemplateEntityManager entityManager;

		[Inject]
		private ISubaddressManager subaddressManager;
		
		[SerializeField]
		private ResourcesSettings[] settingsPages;

		public override void InstallBindings()
		{
			var logger = loggerResolver.GetLogger<EntityPrototypeImportInstaller>();

			var authoringPreset = entityAuthoringSettings.AuthoringPreset;
            
			bool includeSimulationWorld = authoringPreset != EEntityAuthoringPresets.NONE;
			
			bool includeViewWorld = authoringPreset == EEntityAuthoringPresets.DEFAULT
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
								logger?.Log<EntityPrototypeImportInstaller>(
									$"CREATING ENTITY PROTOTYPE VISITORS");

								if (!TryGetEntityWorldsRepository(
									    out IReadOnlyEntityWorldsRepository<World, IDefaultECSEntityWorldController>
										    entityWorldsRepository,
									    logger))
								{
									logger?.LogError<EntityPrototypeImportInstaller>(
										$"COULD NOT GET entityWorldsRepository");

									return;
								}

								if (!TryGetWorldPrototypeVisitor(
									    entityWorldsRepository,
									    WorldConstants.REGISTRY_WORLD_ID,
									    out DefaultECSEntityPrototypeVisitor registryWorldPrototypeVisitor,
									    true,
									    logger))
								{
									logger?.LogError<EntityPrototypeImportInstaller>(
										$"COULD NOT GET registryWorldPrototypeVisitor");

									return;
								}

								DefaultECSEntityPrototypeVisitor simulationWorldPrototypeVisitor = null;

								if (includeSimulationWorld)
								{
									TryGetWorldPrototypeVisitor(
										entityWorldsRepository,
										WorldConstants.SIMULATION_WORLD_ID,
										out simulationWorldPrototypeVisitor,
										false,
										logger);
								}


								DefaultECSEntityPrototypeVisitor viewWorldPrototypeVisitor = null;

								if (includeViewWorld)
								{
									TryGetWorldPrototypeVisitor(
										entityWorldsRepository,
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

		private bool TryGetEntityWorldsRepository(
			out IReadOnlyEntityWorldRepository<World,IDefaultECSEntityWorldController> entityWorldsRepository,
			ILogger logger = null)
		{
			var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;
			
			if (worldContainer == null)
			{
				logger?.LogError<EntityPrototypeImportInstaller>(
					$"entityManager CANNOT BE CAST TO IContainsEntityWorlds<World, IDefaultECSEntityWorldController>");
				
				entityWorldsRepository = null;
				
				return false;
			}

			entityWorldsRepository = worldContainer.EntityWorldsRepository;
			
			if (entityWorldsRepository == null)
			{
				logger?.LogError<EntityPrototypeImportInstaller>(
					$"worldContainer DOES NOT HAVE EntityWorldsRepository");
				
				entityWorldsRepository = null;
				
				return false;
			}

			return true;
		}

		private bool TryGetWorldPrototypeVisitor(
			IReadOnlyEntityWorldRepository<World,IDefaultECSEntityWorldController> entityWorldsRepository,
			string worldID,
			out DefaultECSEntityPrototypeVisitor worldPrototypeVisitor,
			bool logErrorIfWorldNotFound = true,
			ILogger logger = null)
		{
			if (!entityWorldsRepository.HasWorld(
				worldID))
			{
				if (logErrorIfWorldNotFound)
				{
					logger?.LogError<EntityPrototypeImportInstaller>(
						$"entityWorldsRepository DOES NOT HAVE {worldID}");
				}

				worldPrototypeVisitor = null;
				
				return false;
			}
			
			var worldController = entityWorldsRepository.GeTEntityWorldController(
				worldID);

			var worldWithPrototype = worldController as IEntityWorldControllerWithPrototypes<World, Entity>;

			if (worldWithPrototype == null)
			{
				logger?.LogError<EntityPrototypeImportInstaller>(
					$"worldController CANNOT BE CAST TO IPrototypeComplianTEntityWorldController<World, Entity>");
				
				worldPrototypeVisitor = null;
				
				return false;
			}
			
			var worldPrototypeRepository = worldWithPrototype.PrototypeRepository;
			
			if (worldPrototypeRepository == null)
			{
				logger?.LogError<EntityPrototypeImportInstaller>(
					$"worldWithPrototype DOES NOT HAVE PrototypeRepository");
				
				worldPrototypeVisitor = null;
				
				return false;
			}

			worldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
				worldPrototypeRepository,
				loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());

			return true;
		}

		private void CreateEntityPrototypes(
			ResourcesSettings resourcePage,
			
			bool includeSimulationWorld,
			bool includeViewWorld,
			
			DefaultECSEntityPrototypeVisitor registryWorldPrototypeVisitor,
			DefaultECSEntityPrototypeVisitor simulationWorldPrototypeVisitor,
			DefaultECSEntityPrototypeVisitor viewWorldPrototypeVisitor,
			
			ILogger logger = null)
		{
			logger?.Log<EntityPrototypeImportInstaller>(
				$"PARSING RESOURCE PAGE: {resourcePage.ResourcePageName}");
			
			foreach (var resource in resourcePage.Resources)
			{
				string entityName = resource.ResourceID;

				logger?.Log<EntityPrototypeImportInstaller>(
					$"CREATING ENTITY PROTOTYPES FOR ENTITY: {entityName}");

				if (!runtimeResourceManager.TryGetResource(
					entityName.SplitAddressBySeparator(),
					out IReadOnlyResourceData entityResourceData))
				{
					logger?.LogError<EntityPrototypeImportInstaller>(
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

					if (simulationEntityPrototype.Has<SubaddressComponent>())
					{
						subaddressManager.MemorizeSubaddressPart(
							simulationEntityPrototype.Get<SubaddressComponent>().Subaddress);
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
			out Entity entityPrototype,
			ILogger logger = null)
		{
			if (entityResourceData.TryGetVariant(
				variantID,
				out IResourceVariantData registryVariant))
			{
				if (!registryWorldPrototypeVisitor.Load(
					registryVariant
					    .StorageHandle
					    .GetResource<EntitySettings>()
					    .GetPrototypeDTO(loggerResolver),
					out entityPrototype))
				{
					logger?.LogError<EntityPrototypeImportInstaller>(
						$"COULD NOT CREATE {variantID} PROTOTYPE FOR: {entityName}");

					return false;
				}

				logger?.Log<EntityPrototypeImportInstaller>(
					$"CREATED {variantID} PROTOTYPE FOR: {entityName}");

				return true;
			}
			
			logger?.Log<EntityPrototypeImportInstaller>(
				$"NO {variantID} FOUND FOR: {entityName}");
			
			entityPrototype = default;
			
			return true;
		}
	}
}
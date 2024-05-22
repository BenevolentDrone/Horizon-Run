using HereticalSolutions.AssetImport;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using UnityEngine;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.HorizonRun.DI
{
	public class EntityPrototypeImportInstaller : MonoInstaller
	{
		private const string REGISTRY_ENTITY_VARIANT = "Registry entity";

		private const string SIMULATION_ENTITY_VARIANT = "Simulation entity";

		private const string VIEW_ENTITY_VARIANT = "View entity";

		[Inject]
		private ILoggerResolver loggerResolver;

		[Inject]
		private IAssetImportManager assetImportManager;

		[Inject]
		private IRuntimeResourceManager runtimeResourceManager;

		[Inject]
		private EEntityAuthoringPresets authoringPreset;

		[Inject]
		private HorizonRunEntityManager entityManager;

		[SerializeField]
		private ResourcesSettings settings;

		public override void InstallBindings()
		{
			var logger = loggerResolver.GetLogger<EntityPrototypeImportInstaller>();

			bool includeSimulationWorld = authoringPreset != EEntityAuthoringPresets.NONE;
			
			bool includeViewWorld = authoringPreset == EEntityAuthoringPresets.DEFAULT
			                        || authoringPreset == EEntityAuthoringPresets.NETWORKING_HOST
			                        || authoringPreset == EEntityAuthoringPresets.NETWORKING_CLIENT;
			
			TaskExtensions.RunSync(
				() => assetImportManager.Import<ResourceImporterFromScriptable>(
					(importer) =>
					{
						importer.Initialize(
							runtimeResourceManager,
							settings);
					})
					.ContinueWith(
						targetTask =>
						{
							logger?.Log<EntityPrototypeImportInstaller>(
								$"CREATING ENTITY PROTOTYPE VISITORS");

							var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

							var entityWorldsRepository = worldContainer.EntityWorldsRepository;


							var registryWorldController = entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);

							var registryWorldWithPrototype = registryWorldController as IPrototypeCompliantWorldController<World, Entity>;

							var registryWorldPrototypeRepository = registryWorldWithPrototype.PrototypeRepository;

							var registryWorldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
								registryWorldPrototypeRepository,
								loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());

							
							DefaultECSEntityPrototypeVisitor simulationWorldPrototypeVisitor = null;
							
							if (includeSimulationWorld)
							{
								var simulationWorldController =
									entityWorldsRepository.GetWorldController(WorldConstants.SIMULATION_WORLD_ID);

								var simulationWorldWithPrototype =
									simulationWorldController as IPrototypeCompliantWorldController<World, Entity>;

								var simulationWorldPrototypeRepository =
									simulationWorldWithPrototype.PrototypeRepository;

								simulationWorldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
									simulationWorldPrototypeRepository,
									loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());
							}

							
							DefaultECSEntityPrototypeVisitor viewWorldPrototypeVisitor = null;
							
							if (includeViewWorld)
							{
								var viewWorldController =
									entityWorldsRepository.GetWorldController(WorldConstants.VIEW_WORLD_ID);

								var viewWorldWithPrototype =
									viewWorldController as IPrototypeCompliantWorldController<World, Entity>;

								var viewWorldPrototypeRepository = viewWorldWithPrototype.PrototypeRepository;

								viewWorldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
									viewWorldPrototypeRepository,
									loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());
							}


							logger?.Log<EntityPrototypeImportInstaller>(
								$"PARSING ENTITY PROTOTYPES");

							foreach (var resource in settings.Resources)
							{
								string entityName = resource.ResourceID;

								logger?.Log<EntityPrototypeImportInstaller>(
									$"PARSING {entityName}");

								if (!runtimeResourceManager.TryGetResource(
									entityName.SplitAddressBySeparator(),
									out IReadOnlyResourceData resourceData))
								{
									logger?.LogError<EntityPrototypeImportInstaller>(
										$"COULD NOT FIND RESOURCE FOR ENTITY: {entityName}");

									continue;
								}

								if (resourceData.TryGetVariant(
									REGISTRY_ENTITY_VARIANT,
									out IResourceVariantData registryVariant))
								{
									if (!registryWorldPrototypeVisitor.Load(
										registryVariant.StorageHandle.GetResource<EntitySettings>().GetPrototypeDTO(loggerResolver),
										out Entity registryEntity))
									{
										logger?.LogError<EntityPrototypeImportInstaller>(
											$"COULD NOT LOAD REGISTRY ENTITY FOR: {entityName}");

										continue;
									}

									logger?.Log<EntityPrototypeImportInstaller>(
										$"LOADED REGISTRY ENTITY FOR: {entityName}");
								}
								else
								{
									logger?.Log<EntityPrototypeImportInstaller>(
										$"NO REGISTRY ENTITY FOUND FOR: {entityName}");
								}

								if (includeSimulationWorld
									&& resourceData.TryGetVariant(
										SIMULATION_ENTITY_VARIANT,
										out IResourceVariantData simulationVariant))
								{
									if (!simulationWorldPrototypeVisitor.Load(
										simulationVariant.StorageHandle.GetResource<EntitySettings>().GetPrototypeDTO(loggerResolver),
										out Entity simulationEntity))
									{
										logger?.LogError<EntityPrototypeImportInstaller>(
											$"COULD NOT LOAD SIMULATION ENTITY FOR: {entityName}");

										continue;
									}

									logger?.Log<EntityPrototypeImportInstaller>(
										$"LOADED SIMULATION ENTITY FOR: {entityName}");
								}
								else
								{
									logger?.Log<EntityPrototypeImportInstaller>(
										$"NO SIMULATION ENTITY FOUND FOR: {entityName}");
								}

								if (includeViewWorld
									&& resourceData.TryGetVariant(
										VIEW_ENTITY_VARIANT,
										out IResourceVariantData viewVariant))
								{
									if (!viewWorldPrototypeVisitor.Load(
										viewVariant.StorageHandle.GetResource<EntitySettings>().GetPrototypeDTO(loggerResolver),
										out Entity viewEntity))
									{
										logger?.LogError<EntityPrototypeImportInstaller>(
											$"COULD NOT LOAD VIEW ENTITY FOR: {entityName}");

										continue;
									}

									logger?.Log<EntityPrototypeImportInstaller>(
										$"LOADED VIEW ENTITY FOR: {entityName}");
								}
								else
								{
									logger?.Log<EntityPrototypeImportInstaller>(
										$"NO VIEW ENTITY FOUND FOR: {entityName}");
								}
							}
						}
					));
		}
	}
}
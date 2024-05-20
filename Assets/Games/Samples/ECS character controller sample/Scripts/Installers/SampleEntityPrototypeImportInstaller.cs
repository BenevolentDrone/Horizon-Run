using System;

using HereticalSolutions.AssetImport;

using HereticalSolutions.ResourceManagement;

using HereticalSolutions.Entities;

using HereticalSolutions.Logging;

using UnityEngine;

using DefaultEcs;

using Zenject;

namespace HereticalSolutions.Samples.ECSCharacterControllerSample.Installers
{
	public class SampleEntityPrototypeImportInstaller : MonoInstaller
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
		private SampleEntityManager entityManager;

		[SerializeField]
		private ResourcesSettings settings;

		public override void InstallBindings()
		{
			var logger = loggerResolver.GetLogger<SampleEntityPrototypeImportInstaller>();

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
							logger?.Log<SampleEntityPrototypeImportInstaller>(
								$"CREATING ENTITY PROTOTYPE VISITORS");

							var worldContainer = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

							var entityWorldsRepository = worldContainer.EntityWorldsRepository;


							var registryWorldController = entityWorldsRepository.GetWorldController(WorldConstants.REGISTRY_WORLD_ID);

							var registryWorldWithPrototype = registryWorldController as IPrototypeCompliantWorldController<World, Entity>;

							var registryWorldPrototypeRepository = registryWorldWithPrototype.PrototypeRepository;

							var registryWorldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
								registryWorldPrototypeRepository,
								loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());


							var simulationWorldController = entityWorldsRepository.GetWorldController(WorldConstants.SIMULATION_WORLD_ID);

							var simulationWorldWithPrototype = simulationWorldController as IPrototypeCompliantWorldController<World, Entity>;

							var simulationWorldPrototypeRepository = simulationWorldWithPrototype.PrototypeRepository;

							var simulationWorldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
								simulationWorldPrototypeRepository,
								loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());


							var viewWorldController = entityWorldsRepository.GetWorldController(WorldConstants.VIEW_WORLD_ID);

							var viewWorldWithPrototype = viewWorldController as IPrototypeCompliantWorldController<World, Entity>;

							var viewWorldPrototypeRepository = viewWorldWithPrototype.PrototypeRepository;

							var viewWorldPrototypeVisitor = new DefaultECSEntityPrototypeVisitor(
								viewWorldPrototypeRepository,
								loggerResolver?.GetLogger<DefaultECSEntityPrototypeVisitor>());


							logger?.Log<SampleEntityPrototypeImportInstaller>(
								$"PARSING ENTITY PROTOTYPES");

							foreach (var resource in settings.Resources)
							{
								string entityName = resource.ResourceID;

								logger?.Log<SampleEntityPrototypeImportInstaller>(
									$"PARSING {entityName}");

								if (!runtimeResourceManager.TryGetResource(
									entityName.SplitAddressBySeparator(),
									out IReadOnlyResourceData resourceData))
								{
									logger?.LogError<SampleEntityPrototypeImportInstaller>(
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
										logger?.LogError<SampleEntityPrototypeImportInstaller>(
											$"COULD NOT LOAD REGISTRY ENTITY FOR: {entityName}");

										continue;
									}

									logger?.Log<SampleEntityPrototypeImportInstaller>(
										$"LOADED REGISTRY ENTITY FOR: {entityName}");
								}
								else
								{
									logger?.Log<SampleEntityPrototypeImportInstaller>(
										$"NO REGISTRY ENTITY FOUND FOR: {entityName}");
								}

								if (resourceData.TryGetVariant(
									SIMULATION_ENTITY_VARIANT,
									out IResourceVariantData simulationVariant))
								{
									if (!simulationWorldPrototypeVisitor.Load(
										simulationVariant.StorageHandle.GetResource<EntitySettings>().GetPrototypeDTO(loggerResolver),
										out Entity simulationEntity))
									{
										logger?.LogError<SampleEntityPrototypeImportInstaller>(
											$"COULD NOT LOAD SIMULATION ENTITY FOR: {entityName}");

										continue;
									}

									logger?.Log<SampleEntityPrototypeImportInstaller>(
										$"LOADED SIMULATION ENTITY FOR: {entityName}");
								}
								else
								{
									logger?.Log<SampleEntityPrototypeImportInstaller>(
										$"NO SIMULATION ENTITY FOUND FOR: {entityName}");
								}

								if (resourceData.TryGetVariant(
									VIEW_ENTITY_VARIANT,
									out IResourceVariantData viewVariant))
								{
									if (!viewWorldPrototypeVisitor.Load(
										viewVariant.StorageHandle.GetResource<EntitySettings>().GetPrototypeDTO(loggerResolver),
										out Entity viewEntity))
									{
										logger?.LogError<SampleEntityPrototypeImportInstaller>(
											$"COULD NOT LOAD VIEW ENTITY FOR: {entityName}");

										continue;
									}

									logger?.Log<SampleEntityPrototypeImportInstaller>(
										$"LOADED VIEW ENTITY FOR: {entityName}");
								}
								else
								{
									logger?.Log<SampleEntityPrototypeImportInstaller>(
										$"NO VIEW ENTITY FOUND FOR: {entityName}");
								}
							}
						}
					));
		}
	}
}
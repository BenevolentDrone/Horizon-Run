using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Logging;

using DefaultEcs;

namespace HereticalSolutions.Entities.Factories
{
    /// <summary>
    /// Class containing methods to build entities and their components.
    /// </summary>
    public static partial class DefaultECSEntityFactory
    {
        /*
        public delegate void ComponentReaderDelegate<TComponent>(
            Entity entity,
            IReadOnlyRepository<Type, int> typeToHash,
            List<ECSComponentDTO> componentDTOs);
        */

        #region Entity manager

        public static DefaultECSEntityManager<TEntityID> BuildDefaultECSSimpleEntityManager<TEntityID, TEntityIDComponent>(
            Func<TEntityID> allocateIDDelegate,

            Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
            Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

            ILoggerResolver loggerResolver = null)
        {
            var registryEntityRepository = RepositoriesFactory.BuildDictionaryRepository<TEntityID, Entity>();

            var entityWorldsRepository = BuildDefaultECSEntityWorldsRepository(loggerResolver);


            entityWorldsRepository.AddWorld(
                WorldConstants.REGISTRY_WORLD_ID,
                BuildDefaultECSRegistryWorldController(
                    createIDComponentDelegate,
                    BuildDefaultECSPrototypesRepository(),
                    loggerResolver));

            entityWorldsRepository.AddWorld(
                WorldConstants.EVENT_WORLD_ID,
                BuildDefaultECSEventWorldController(
                    loggerResolver));

            entityWorldsRepository.AddWorld(
                WorldConstants.SIMULATION_WORLD_ID,
                BuildDefaultECSWorldController
                    <TEntityID,
                    TEntityIDComponent,
                    SimulationEntityComponent,
                    ResolveSimulationComponent>(
                        getEntityIDFromIDComponentDelegate,
                        createIDComponentDelegate,

                        (component) => { return component.SimulationEntity; },
                        (component) => { return component.PrototypeID; },
                        (prototypeID, entity) => 
                        {
                            return new SimulationEntityComponent
                            {
                                PrototypeID = prototypeID,

                                SimulationEntity = entity
                            };
                        },

                        (source) => { return new ResolveSimulationComponent { Source = source }; },

                        loggerResolver));

            entityWorldsRepository.AddWorld(
                WorldConstants.VIEW_WORLD_ID,
                BuildDefaultECSWorldController
                    <TEntityID,
                    TEntityIDComponent,
                    ViewEntityComponent,
                    ResolveViewComponent>(
                        getEntityIDFromIDComponentDelegate,
                        createIDComponentDelegate,

                        (component) => { return component.ViewEntity; },
                        (component) => { return component.PrototypeID; },
                        (prototypeID, entity) =>
                        {
                            return new ViewEntityComponent
                            {
                                PrototypeID = prototypeID,

                                ViewEntity = entity
                            };
                        },

                        (source) => { return new ResolveViewComponent { Source = source }; },

                        loggerResolver));

            List<World> childEntityWorlds = new List<World>();

            childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.SIMULATION_WORLD_ID));
            childEntityWorlds.Add(entityWorldsRepository.GetWorld(WorldConstants.VIEW_WORLD_ID));

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSEntityManager<TEntityID>>()
                ?? null;

            return new DefaultECSEntityManager<TEntityID>(
                allocateIDDelegate,
                registryEntityRepository,
                entityWorldsRepository,
                childEntityWorlds,
                logger);
        }

        public static DefaultECSEventWorldController BuildDefaultECSEventWorldController(
            ILoggerResolver loggerResolver = null)
        {
            World world = new World();

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSEventWorldController>()
                ?? null;

            return new DefaultECSEventWorldController(
                world,
                logger);
        }

        public static DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>
            BuildDefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>(
                Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,
                IPrototypesRepository<World, Entity> prototypeRepository,
                ILoggerResolver loggerResolver = null)
        {
            World world = new World();

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>>()
                ?? null;

            return new DefaultECSRegistryWorldController<TEntityID, TEntityIDComponent>(
                world,

                createIDComponentDelegate,

                prototypeRepository,
                new ComponentCloner(),
                logger);
        }

        public static DefaultECSWorldController
            <TEntityID,
            TEntityIDComponent,
            TWorldIdentityComponent,
            TResolveWorldIdentityComponent>
            BuildDefaultECSWorldController
                <TEntityID,
                TEntityIDComponent,
                TWorldIdentityComponent,
                TResolveWorldIdentityComponent>(
                    Func<TEntityIDComponent, TEntityID> getEntityIDFromIDComponentDelegate,
                    Func<TEntityID, TEntityIDComponent> createIDComponentDelegate,

                    Func<TWorldIdentityComponent, Entity> getEntityFromWorldIdentityComponentDelegate,
                    Func<TWorldIdentityComponent, string> getPrototypeIDFromWorldIdentityComponentDelegate,
                    Func<string, Entity, TWorldIdentityComponent> createWorldIdentityComponentDelegate,

                    Func<object, TResolveWorldIdentityComponent> createResolveWorldIdentityComponentDelegate,
                    ILoggerResolver loggerResolver = null)
        {
            World world = new World();

            ILogger logger =
                loggerResolver?.GetLogger
                    <DefaultECSWorldController
                        <TEntityID,
                        TEntityIDComponent,
                        TWorldIdentityComponent,
                        TResolveWorldIdentityComponent>>()
                ?? null;

            return new DefaultECSWorldController
                <TEntityID,
                TEntityIDComponent,
                TWorldIdentityComponent,
                TResolveWorldIdentityComponent>(
                    world,

                    getEntityIDFromIDComponentDelegate,
                    createIDComponentDelegate,

                    getEntityFromWorldIdentityComponentDelegate,
                    getPrototypeIDFromWorldIdentityComponentDelegate,
                    createWorldIdentityComponentDelegate,

                    createResolveWorldIdentityComponentDelegate,

                    BuildDefaultECSPrototypesRepository(),
                    new ComponentCloner(),
                    logger);
        }

        #endregion

        #region Prototypes repository

        public static DefaultECSPrototypesRepository BuildDefaultECSPrototypesRepository()
        {
            return new DefaultECSPrototypesRepository(
                new World(),
                RepositoriesFactory.BuildDictionaryRepository<string, Entity>());
        }

        #endregion

        #region Entity worlds repository

        public static IDefaultECSEntityWorldsRepository BuildDefaultECSEntityWorldsRepository(
            ILoggerResolver loggerResolver = null)
        {
            var worldsRepository = RepositoriesFactory.BuildDictionaryRepository<string, World>();

            var worldControllersRepository = RepositoriesFactory.BuildDictionaryRepository<World, IDefaultECSEntityWorldController>();

            ILogger logger =
                loggerResolver?.GetLogger<DefaultECSEntityWorldsRepository>()
                ?? null;

            return new DefaultECSEntityWorldsRepository(
                worldsRepository,
                worldControllersRepository,
                logger);
        }

        #endregion

        #region Event entity builder

        public static DefaultECSEventEntityBuilder<TEntityID> BuildDefaultECSEventEntityBuilder<TEntityID>(
            World world)
        {
            return new DefaultECSEventEntityBuilder<TEntityID>(world);
        }

        #endregion

        /*
        #region Component readers and writers

        public static ReadComponentToDTOsListDelegate[] BuildComponentReaders(
            MethodInfo readComponentMethodInfo,
            Type[] componentTypes)
        {
            var result = new ReadComponentToDTOsListDelegate[componentTypes.Length];

            for (int i = 0; i < result.Length; i++)
            {
                MethodInfo readComponentGeneric = readComponentMethodInfo.MakeGenericMethod(componentTypes[i]);
                
                ReadComponentToDTOsListDelegate readComponentGenericDelegate =
                    (ReadComponentToDTOsListDelegate)readComponentGeneric.CreateDelegate(
                        typeof(ReadComponentToDTOsListDelegate),
                        null);

                result[i] = readComponentGenericDelegate;
            }

            return result;
        }
        
        public static IReadOnlyRepository<Type, WriteComponentFromDTODelegate> BuildComponentWriters(
            MethodInfo writeComponentMethodInfo,
            Type[] componentTypes)
        {
            IReadOnlyRepository<Type, WriteComponentFromDTODelegate> result = RepositoriesFactory.BuildDictionaryRepository<Type, WriteComponentFromDTODelegate>();

            for (int i = 0; i < componentTypes.Length; i++)
            {
                MethodInfo writeComponentGeneric = writeComponentMethodInfo.MakeGenericMethod(componentTypes[i]);
                
                WriteComponentFromDTODelegate writeComponentGenericDelegate =
                    (WriteComponentFromDTODelegate)writeComponentGeneric.CreateDelegate(
                        typeof(WriteComponentFromDTODelegate),
                        null);

                ((IRepository<Type, WriteComponentFromDTODelegate>)result).Add(
                    componentTypes[i],
                    writeComponentGenericDelegate);
            }

            return result;
        }
        
        public static void ReadComponentToDTOsList<TComponent>(
            Entity entity,
            IReadOnlyRepository<Type, int> typeToHash,
            List<ECSComponentDTO> componentDTOs)
        {
            //Early return for AoT compilation calls
            if (componentDTOs == null)
                return;
            
            if (!entity.Has<TComponent>())
            {
                return;
            }

            var dto = new ECSComponentDTO();
            
            dto.TypeHash = typeToHash.Get(typeof(TComponent));
            
            dto.Data = ToBytes(entity.Get<TComponent>());
            
            componentDTOs.Add(dto);
        }
        
        public static void WriteComponentFromDTO<TComponent>(
            Entity entity,
            ECSComponentDTO componentDTO)
        {
            //Early return for AoT compilation calls
            if (componentDTO.Data == null)
                return;
            

            var component = (TComponent)FromBytes(componentDTO.Data, typeof(TComponent));
            
            entity.Set<TComponent>(component);
        }

        public static byte[] ToBytes(object component)
        {
            int componentSize = Marshal.SizeOf(component);
            
            byte[] result = new byte[componentSize];
            
            IntPtr ptr = IntPtr.Zero;
            
            try
            {
                ptr = Marshal.AllocHGlobal(componentSize);
                
                Marshal.StructureToPtr(
                    component,
                    ptr,
                    true);
                
                Marshal.Copy(
                    ptr,
                    result,
                    0,
                    componentSize);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            
            return result;
        }

        public static object FromBytes(
            byte[] data,
            Type componentType)
        {
            object result;
            
            int size = Marshal.SizeOf(componentType);
            
            IntPtr ptr = IntPtr.Zero;
            
            try
            {
                ptr = Marshal.AllocHGlobal(size);
                
                Marshal.Copy(
                    data,
                    0,
                    ptr,
                    size);
                
                result = Marshal.PtrToStructure(
                    ptr,
                    componentType);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            
            return result;
        }

        #endregion
        */
    }
}
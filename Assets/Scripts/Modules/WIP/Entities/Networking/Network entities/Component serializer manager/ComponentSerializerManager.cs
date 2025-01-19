using System.Collections.Generic;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

using LiteNetLib.Utils;

using DefaultEcs;

namespace HereticalSolutions.Modules.Core_DefaultECS.Networking
{
    public class ComponentSerializerManager
        : IComponentSerializerManager
    {
        private readonly IPool<IComponentSerializer>[] componentSerializersPool;
        
        private readonly ESerializedEntityType serializedEntityType;
        
        private readonly ComponentSerializationContext componentSerializationContext;
        
        private readonly List<ushort> serializerIDsCache;
        
        private readonly List<IComponentSerializer> serializersCache;

        private readonly ILogger logger;
        
        public ComponentSerializerManager(
            IPool<IComponentSerializer>[] componentSerializersPool,
            ESerializedEntityType serializedEntityType,
            ComponentSerializationContext componentSerializationContext,
            List<ushort> serializerIDsCache,
            List<IComponentSerializer> serializersCache,
            ILogger logger)
        {
            this.componentSerializersPool = componentSerializersPool;
            
            this.serializedEntityType = serializedEntityType;
            
            this.componentSerializationContext = componentSerializationContext;
            
            this.serializerIDsCache = serializerIDsCache;
            
            this.serializersCache = serializersCache;
            
            this.logger = logger;
        }

        public ESerializedEntityType SerializedEntityType => serializedEntityType;
        
        public ComponentSerializationContext Context => componentSerializationContext;

        public void Serialize(
            IContainsComponentData packet,
            NetDataWriter writer)
        {
            packet.ComponentSerializerManager = this;
            
            packet.Serialize(writer);

            packet.ComponentSerializerManager = null;
        }

        public void InvokeComponentSerializersSerialization(
            IComponentSerializer[] serializers,
            ushort[] serializerIDs,
            NetDataWriter writer)
        {
            for (int i = 0; i < serializerIDs.Length; i++)
            {
                var serializer = serializers[i];

                if (serializer.SerializedEntityType != serializedEntityType)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER {serializer.GetType().Name} HAS INVALID ENTITY TYPE. EXPECTED: {serializedEntityType} ACTUAL: {serializer.SerializedEntityType}");
                }

                serializer.Serialize(writer);
                
                serializer.Clear();
                

                var serializerIndex = serializerIDs[i];
                
                if (serializerIndex < 0 || serializerIndex >= componentSerializersPool.Length)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER INDEX {serializerIndex} OUT OF RANGE");
                }
                
                componentSerializersPool[serializerIndex].Push(serializer);
            }
        }
        
        public void Deserialize(
            IContainsComponentData packet,
            NetDataReader reader)
        {
            packet.ComponentSerializerManager = this;
            
            packet.Deserialize(reader);

            packet.ComponentSerializerManager = null;
        }

        public void InvokeComponentSerializersDeserialization(
            IComponentSerializer[] serializers,
            ushort[] serializerIDs,
            NetDataReader reader)
        {
            for (int i = 0; i < serializerIDs.Length; i++)
            {
                var serializerIndex = serializerIDs[i];

                if (serializerIndex < 0 || serializerIndex >= componentSerializersPool.Length)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"INDEX OUT OF RANGE");
                }

                var serializer = componentSerializersPool[serializerIndex].Pop();

                if (serializer.SerializedEntityType != serializedEntityType)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER {serializer.GetType().Name} HAS INVALID ENTITY TYPE. EXPECTED: {serializedEntityType} ACTUAL: {serializer.SerializedEntityType}");
                }
                
                serializers[i] = serializer;
                
                serializer.Deserialize(reader);
            }
        }

        public ushort ParseEntity(
            Entity entity,
            out ushort[] serializerIDs,
            out IComponentSerializer[] serializers)
        {
            serializerIDsCache.Clear();

            serializersCache.Clear();
            
            for (ushort i = 0; i < componentSerializersPool.Length; i++)
            {
                var serializer = componentSerializersPool[i].Pop();
                
                if (serializer.SerializedEntityType != serializedEntityType)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER {serializer.GetType().Name} HAS INVALID ENTITY TYPE. EXPECTED: {serializedEntityType} ACTUAL: {serializer.SerializedEntityType}");
                }

                if (serializer.ReadFrom(
                    entity,
                    componentSerializationContext))
                {
                    serializerIDsCache.Add(i);
                    
                    serializersCache.Add(serializer);
                }
                else
                {
                    serializer.Clear();
                    
                    componentSerializersPool[i].Push(serializer);
                }
            }

            ushort count = (ushort)serializerIDsCache.Count;

            serializerIDs = serializerIDsCache.ToArray();

            serializers = serializersCache.ToArray();
            
            
            serializerIDsCache.Clear();

            serializersCache.Clear();

            
            foreach (var serializerID in serializerIDs)
            {
                if (serializerID < 0 || serializerID >= componentSerializersPool.Length)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER INDEX {serializerID} OUT OF RANGE");
                }
            }
            
            return count;
        }
        
        public void PopulateEntity(
            Entity entity,
            IComponentSerializer[] serializers,
            ushort[] serializerIDs)
        {
            for (int i = 0; i < serializers.Length; i++)
            {
                var serializer = serializers[i];
                
                if (serializer.SerializedEntityType != serializedEntityType)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER {serializer.GetType().Name} HAS INVALID ENTITY TYPE. EXPECTED: {serializedEntityType} ACTUAL: {serializer.SerializedEntityType}");
                }

                if (!serializer.WriteTo(
                    entity,
                    componentSerializationContext))
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER {serializer.GetType().Name} FAILED TO WRITE TO ENTITY");
                }
                
                serializer.Clear();

                var serializerIndex = serializerIDs[i];
                
                if (serializerIndex < 0 || serializerIndex >= componentSerializersPool.Length)
                {
                    logger?.LogError<ComponentSerializerManager>(
                        $"SERIALIZER INDEX {serializerIndex} OUT OF RANGE");
                }
                
                componentSerializersPool[serializerIndex].Push(serializer);
            }
        }
    }
}
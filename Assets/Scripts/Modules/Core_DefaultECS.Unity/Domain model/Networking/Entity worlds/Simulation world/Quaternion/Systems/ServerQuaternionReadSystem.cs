using System.Collections.Generic;

using HereticalSolutions.Time;

using HereticalSolutions.Messaging;

using HereticalSolutions.Networking;

using HereticalSolutions.Templates.Universal.Networking;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerQuaternionReadSystem : AServerReadSystem<
        QuaternionComponent,
        ServerQuaternionComponent,
        IDQuaternionPair,
        EntityQuaternionDeltasPacket>
    {
        public ServerQuaternionReadSystem(
            World world,
            NetworkDeltaReplicationSettings networkDeltaReplicationSettings,
            ITimerManager simulationTimerManager,
            INetworkHost host,
            UniversalTemplateEntityManager entityManager,
            INonAllocMessageSender networkBusAsSender,
            List<IDQuaternionPair> dirtyEntities,
            ILoggerResolver loggerResolver,
            ILogger logger = null,
            float controlPacketFrequencyChance = DEFAULT_CONTROL_PACKET_FREQUENCY_CHANCE)
            : base(
                world,
                networkDeltaReplicationSettings,
                simulationTimerManager,
                host,
                entityManager,
                networkBusAsSender,
                dirtyEntities,
                loggerResolver,
                logger,
                controlPacketFrequencyChance)
        {
        }

        protected override bool ComponentChanged(
            QuaternionComponent quaternionComponent,
            ServerQuaternionComponent serverQuaternionComponent)
        {
            return Quaternion.Angle(
                       quaternionComponent.Quaternion,
                       serverQuaternionComponent.ServerQuaternion)
                   > Quaternion.kEpsilon;
        }

        protected override void ReadComponentToServerComponent(
            QuaternionComponent quaternionComponent,
            ref ServerQuaternionComponent serverQuaternionComponent,
            ushort tick)
        {
            serverQuaternionComponent.ServerQuaternion = quaternionComponent.Quaternion;

            serverQuaternionComponent.ServerTick = host.Tick;
        }

        protected override IDQuaternionPair CreateIDValuePair(
            ushort networkID,
            QuaternionComponent quaternionComponent)
        {
            return new IDQuaternionPair
            {
                NetworkID =  networkID,
                            
                Quaternion = quaternionComponent.Quaternion
            };
        }

        protected override int GetMaxCapacityPerPacket()
        {
            return EntityQuaternionDeltasPacket.MAX_CAPACITY;
        }

        protected override EntityQuaternionDeltasPacket CreateDeltaPacket(
            ushort tick,
            ushort count,
            IDQuaternionPair[] payload)
        {
            return new EntityQuaternionDeltasPacket
            {
                ServerTick = host.Tick,
                                
                Count = (ushort)payload.Length,
                                
                Quaternions = payload
            };
        }
    }
}
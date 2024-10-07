using System.Collections.Generic;

using HereticalSolutions.Time;

using HereticalSolutions.Messaging;

using HereticalSolutions.Templates.Universal.Networking;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;
using HereticalSolutions.Networking;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public class ServerUniformRotationReadSystem : AServerReadSystem<
        UniformRotationComponent,
        ServerUniformRotationComponent,
        IDUniformRotationPair,
        EntityUniformRotationDeltasPacket>
    {
        public ServerUniformRotationReadSystem(
            World world,
            NetworkDeltaReplicationSettings networkDeltaReplicationSettings,
            ITimerManager simulationTimerManager,
            INetworkHost host,
            UniversalTemplateEntityManager entityManager,
            INonAllocMessageSender networkBusAsSender,
            List<IDUniformRotationPair> dirtyEntities,
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
            UniformRotationComponent rotationComponent,
            ServerUniformRotationComponent serverRotationComponent)
        {
            return Mathf.Abs(
                       rotationComponent.Angle.SanitizeAngle() - serverRotationComponent.ServerRotation.SanitizeAngle())
                   > MathHelpers.EPSILON;
        }

        protected override void ReadComponentToServerComponent(
            UniformRotationComponent rotationComponent,
            ref ServerUniformRotationComponent serverRotationComponent,
            ushort tick)
        {
            serverRotationComponent.ServerRotation = rotationComponent.Angle.SanitizeAngle();

            serverRotationComponent.ServerTick = host.Tick;
        }

        protected override IDUniformRotationPair CreateIDValuePair(
            ushort networkID,
            UniformRotationComponent rotationComponent)
        {
            return new IDUniformRotationPair
            {
                NetworkID =  networkID,
                            
                Rotation = rotationComponent.Angle.SanitizeAngle()
            };
        }

        protected override int GetMaxCapacityPerPacket()
        {
            return EntityUniformRotationDeltasPacket.MAX_CAPACITY;
        }

        protected override EntityUniformRotationDeltasPacket CreateDeltaPacket(
            ushort tick,
            ushort count,
            IDUniformRotationPair[] payload)
        {
            return new EntityUniformRotationDeltasPacket
            {
                ServerTick = host.Tick,
                                
                Count = (ushort)payload.Length,
                                
                UniformRotations = payload
            };
        }
    }
}
using System.Collections.Generic;

using HereticalSolutions.Time;

using HereticalSolutions.Messaging;

using HereticalSolutions.Networking;

using HereticalSolutions.Templates.Universal.Networking;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using DefaultEcs;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public class ServerPosition3DReadSystem : AServerReadSystem<
        Position3DComponent,
        ServerPosition3DComponent,
        IDPosition2DPair,
        EntityPosition2DDeltasPacket>
    {
        public ServerPosition3DReadSystem(
            World world,
            NetworkDeltaReplicationSettings networkDeltaReplicationSettings,
            ITimerManager simulationTimerManager,
            INetworkHost host,
            UniversalTemplateEntityManager entityManager,
            INonAllocMessageSender networkBusAsSender,
            List<IDPosition2DPair> dirtyEntities,
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
            Position3DComponent position3DComponent,
            ServerPosition3DComponent serverPosition2DComponent)
        {
            return (position3DComponent.Position - serverPosition2DComponent.ServerPosition).sqrMagnitude
               > MathHelpers.EPSILON;
        }

        protected override void ReadComponentToServerComponent(
            Position3DComponent position3DComponent,
            ref ServerPosition3DComponent serverPosition3DComponent,
            ushort tick)
        {
            serverPosition3DComponent.ServerPosition = position3DComponent.Position;

            serverPosition3DComponent.ServerTick = tick;
        }

        protected override IDPosition2DPair CreateIDValuePair(
            ushort networkID,
            Position3DComponent position3DComponent)
        {
            return new IDPosition2DPair
            {
                NetworkID = networkID,

                Position = position3DComponent.Position 
            };
        }

        protected override int GetMaxCapacityPerPacket()
        {
            return EntityPosition2DDeltasPacket.MAX_CAPACITY;
        }

        protected override EntityPosition2DDeltasPacket CreateDeltaPacket(
            ushort tick,
            ushort count,
            IDPosition2DPair[] payload)
        {
            return new EntityPosition2DDeltasPacket
            {
                ServerTick = host.Tick,

                Count = (ushort)payload.Length,

                Positions = payload
            };
        }
    }
}
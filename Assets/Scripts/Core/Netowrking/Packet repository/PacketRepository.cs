using System;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Networking
{
    public class PacketRepository
        : IPacketRepository
    {
        private IReadOnlyRepository<byte, Type> packetIDToType;
        
        private IReadOnlyRepository<Type, byte> typeToPacketID;
        
        public PacketRepository(
            IReadOnlyRepository<byte, Type> packetIDToType,
            IReadOnlyRepository<Type, byte> typeToPacketID)
        {
            this.packetIDToType = packetIDToType;
            
            this.typeToPacketID = typeToPacketID;
        }
        
        #region IPacketRepository

        public bool TryGetPacketType(
            byte packetID,
            out Type type)
        {
            return packetIDToType.TryGet(
                packetID,
                out type);
        }

        public bool TryGetPacketID(
            Type type,
            out byte packetID)
        {
            return typeToPacketID.TryGet(
                type,
                out packetID);
        }

        #endregion
    }
}
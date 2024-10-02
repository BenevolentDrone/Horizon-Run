using System;

namespace HereticalSolutions.Networking
{
    public interface IPacketRepository
    {
        bool TryGetPacketType(
            byte packetID,
            out Type type);
        
        bool TryGetPacketID(
            Type type,
            out byte packetID);
    }
}
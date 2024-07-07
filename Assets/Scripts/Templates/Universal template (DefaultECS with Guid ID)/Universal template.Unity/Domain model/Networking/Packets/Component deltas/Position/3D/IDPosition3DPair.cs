using HereticalSolutions.Templates.Universal.Networking;

using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    //14 bytes
    public struct IDPosition3DPair : INetSerializable
    {
        //2 bytes
        public ushort NetworkID;
        
        //12 bytes
        public Vector3 Position;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NetworkID);
            
            writer.Put(Position);
        }

        public void Deserialize(NetDataReader reader)
        {
            NetworkID = reader.GetUShort();

            Position = reader.GetVector3();
        }
    }
}
using HereticalSolutions.Templates.Universal.Networking;

using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    //18 bytes
    public struct IDQuaternionPair : INetSerializable
    {
        //2 bytes
        public ushort NetworkID;
        
        //16 bytes
        public Quaternion Quaternion;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NetworkID);
            
            writer.Put(Quaternion);
        }

        public void Deserialize(NetDataReader reader)
        {
            NetworkID = reader.GetUShort();

            Quaternion = reader.GetQuaternion();
        }
    }
}
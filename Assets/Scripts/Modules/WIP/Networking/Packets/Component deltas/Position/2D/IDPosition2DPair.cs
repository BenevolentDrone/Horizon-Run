using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    /*
    //10 bytes
    public struct IDPosition2DPair : INetSerializable
    {
        //2 bytes
        public ushort NetworkID;
        
        //8 bytes
        public Vector2 Position;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NetworkID);
            
            writer.Put(Position);
        }

        public void Deserialize(NetDataReader reader)
        {
            NetworkID = reader.GetUShort();

            Position = reader.GetVector2();
        }
    }
    */
}
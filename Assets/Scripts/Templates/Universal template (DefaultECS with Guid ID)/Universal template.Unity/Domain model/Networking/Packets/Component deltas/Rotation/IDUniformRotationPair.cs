using HereticalSolutions.Networking;

using LiteNetLib.Utils;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    //6 bytes
    public struct IDUniformRotationPair : INetSerializable
    {
        //2 bytes
        public ushort NetworkID;
        
        //4 bytes
        public float Rotation;
        
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(NetworkID);
            
            writer.Put(Rotation);
        }

        public void Deserialize(NetDataReader reader)
        {
            NetworkID = reader.GetUShort();

            Rotation = reader.GetFloat();
        }
    }
}
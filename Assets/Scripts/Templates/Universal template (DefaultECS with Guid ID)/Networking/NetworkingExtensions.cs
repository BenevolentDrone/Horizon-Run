using LiteNetLib.Utils;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Networking
{
    public static class NetworkingExtensions
    {
        #region Vector2
        
        public static void Put(
            this NetDataWriter writer,
            Vector2 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
        }

        public static Vector2 GetVector2(
            this NetDataReader reader)
        {
            Vector2 v;
            
            v.x = reader.GetFloat();
            v.y = reader.GetFloat();
            
            return v;
        }
        
        #endregion
        
        #region Vector3
        
        public static void Put(
            this NetDataWriter writer,
            Vector3 vector)
        {
            writer.Put(vector.x);
            writer.Put(vector.y);
            writer.Put(vector.z);
        }

        public static Vector3 GetVector3(
            this NetDataReader reader)
        {
            Vector3 v;
            
            v.x = reader.GetFloat();
            v.y = reader.GetFloat();
            v.z = reader.GetFloat();
            
            return v;
        }
        
        #endregion

        #region Quaternion

        public static void Put(
            this NetDataWriter writer,
            Quaternion quaternion)
        {
            writer.Put(quaternion.x);
            writer.Put(quaternion.y);
            writer.Put(quaternion.z);
            writer.Put(quaternion.w);
        }

        public static Quaternion GetQuaternion(
            this NetDataReader reader)
        {
            Quaternion q;
            
            q.x = reader.GetFloat();
            q.y = reader.GetFloat();
            q.z = reader.GetFloat();
            q.w = reader.GetFloat();
            
            return q;
        }

        #endregion
    }
}
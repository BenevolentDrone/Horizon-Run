using UnityEngine;

namespace HereticalSolutions
{
    public static class MathHelpersUnity
    {
        public static float DegreesToRadians(float degrees)
        {
            return UnityEngine.Mathf.Deg2Rad * degrees;   
        }
        
        public static float TargetAngle(Vector2 direction)
        {
            return Mathf.Atan2(
                       -direction.y,
                       direction.x)
                   * Mathf.Rad2Deg
                   + 90f;
        }

        #region Axis vectors

        public static Vector3 AxisVector3X(float value)
        {
            return new Vector3(
                value,
                0f,
                0f);
        }
        
        public static Vector3 AxisVector3Y(float value)
        {
            return new Vector3(
                0f,
                value,
                0f);
        }
        
        public static Vector3 AxisVector3Z(float value)
        {
            return new Vector3(
                0f,
                0f,
                value);
        }

        #endregion

        #region Uniform vectors

        public static Vector3 UniformVector3(float value)
        {
            return new Vector3(
                value,
                value,
                value);
        }

        #endregion
        
        #region Vector 2 <- -> Vector 3

        public static Vector2 Vector3To2XY(Vector3 vector)
        {
            return (Vector2)vector;
        }
        
        public static Vector2 Vector3To2XZ(Vector3 vector)
        {
            return new Vector2(
                vector.x,
                vector.z);
        }
        
        public static Vector3 Vector2XYTo3(Vector2 vector)
        {
            return (Vector3)vector;
        }
        
        public static Vector3 Vector2XZTo3(Vector2 vector)
        {
            return new Vector3(
                vector.x,
                0f,
                vector.y);
        }
        
        #endregion

        #region Unity vector <- -> Numerics vector

        public static System.Numerics.Vector2 UnityVector2ToNumericsVector2(Vector2 vector)
        {
            return new System.Numerics.Vector2(
                vector.x,
                vector.y);
        }

        public static System.Numerics.Vector3 UnityVector3ToNumericsVector3(Vector3 vector)
        {
            return new System.Numerics.Vector3(
                vector.x,
                vector.y,
                vector.z);
        }

        public static Vector2 NumericsVector2ToUnityVector2(System.Numerics.Vector2 vector)
        {
            return new Vector2(
                vector.X,
                vector.Y);
        }
        
        public static Vector3 NumericsVector3ToUnityVector3(System.Numerics.Vector3 vector)
        {
            return new Vector3(
                vector.X,
                vector.Y,
                vector.Z);
        }
        
        #endregion
    }
}
namespace HereticalSolutions
{
    /// <summary>
    /// Provides mathematical helper functions.
    /// </summary>
    public static class MathHelpers
    {
        //TODO: add stuff from OpenTK

        /// <summary>
        /// Defines a small constant value used for floating-point comparisons.
        /// </summary>
        public const float EPSILON = 0.0001f;

        public static float SanitizeAngle(this float degrees)
        {
            while (degrees > 180f)
            {
                degrees -= 360f;
            }

            while (degrees < -180f)
            {
                degrees += 360f;
            }

            return degrees;
        }

        /*
        public static float DegreesToRadians(float degrees)
        {
#if (UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_SERVER || UNITY_WEBGL || UNITY_EDITOR)
            return UnityEngine.Mathf.Deg2Rad * degrees;
#else
            return MathF.PI / 180f * degrees;
#endif            
        }
        */
    }
}
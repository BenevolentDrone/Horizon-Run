using UnityEngine;

public static class Vector3Extensions
{
    public static string ToNormalString(this Vector3 vector)
    {
        return string.Format("[{0} {1} {2}]", vector.x, vector.y, vector.z);
    }
}
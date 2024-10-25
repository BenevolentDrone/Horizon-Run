using UnityEngine;

public static class QuaternionExtensions
{
    public static string ToNormalString(this Quaternion quaternion)
    {
        return string.Format("[{0} {1} {2} {3}]", quaternion.x, quaternion.y, quaternion.z, quaternion.w);
    }
}
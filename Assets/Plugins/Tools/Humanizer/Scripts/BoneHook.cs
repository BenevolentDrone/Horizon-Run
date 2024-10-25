using UnityEngine;

[ExecuteInEditMode]
public class BoneHook : MonoBehaviour
{
    public Bounds Bounds { get; set; }

    public Color Color { get; set; }

    void OnDrawGizmos()
    {
        var cachedColor = Gizmos.color;

        Gizmos.color = Color;

        var cachedMatrix = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Bounds.center, Bounds.size);

        Gizmos.matrix = cachedMatrix;

        Gizmos.color = cachedColor;
    }
}

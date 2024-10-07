using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    [ViewComponent]
    public class ServerPositionRotation3DViewComponent : AMonoViewComponent
    {
        public Transform PositionRotationTransform;
        
        public LineRenderer LineRenderer;
        
        public bool Visible = false;
        
        public float LineWidth = 0.1f;

        public float Padding = 0.2f;

        public Vector3 ServerPosition;

        public Quaternion ServerRotation;
        
        public Vector3 Position;

        public Quaternion Rotation;

        public bool Dirty;
    }
}
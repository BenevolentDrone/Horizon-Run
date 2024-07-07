using HereticalSolutions.Entities;

using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    [ViewComponent]
    public class ServerPositionRotation2DViewComponent : AMonoViewComponent
    {
        public Transform PositionRotationTransform;
        
        public LineRenderer LineRenderer;
        
        public bool Visible = false;
        
        public float LineWidth = 0.1f;

        public float Padding = 0.2f;

        public Vector2 ServerPosition;

        public float ServerRotation;
        
        public Vector2 Position;

        public float Rotation;

        public bool Dirty;
    }
}
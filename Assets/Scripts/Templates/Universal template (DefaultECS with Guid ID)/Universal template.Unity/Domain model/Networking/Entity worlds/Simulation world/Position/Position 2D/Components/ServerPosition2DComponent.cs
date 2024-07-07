using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public struct ServerPosition2DComponent
    {
        public Vector2 ServerPosition;

        public Vector2 Error;

        public ushort ServerTick;

        public bool Dirty;
    }
}
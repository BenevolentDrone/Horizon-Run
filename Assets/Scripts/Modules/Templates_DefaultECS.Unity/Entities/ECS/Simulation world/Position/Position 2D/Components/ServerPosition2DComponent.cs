using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public struct ServerPosition2DComponent
    {
        public Vector2 ServerPosition;

        public Vector2 Error;

        public ushort ServerTick;

        public bool Dirty;
    }
}
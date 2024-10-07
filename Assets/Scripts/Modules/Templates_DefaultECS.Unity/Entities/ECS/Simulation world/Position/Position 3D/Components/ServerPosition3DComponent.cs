using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public struct ServerPosition3DComponent
    {
        public Vector3 ServerPosition;

        public Vector3 Error;

        public ushort ServerTick;

        public bool Dirty;
    }
}
using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public struct ServerPosition3DComponent
    {
        public Vector3 ServerPosition;

        public Vector3 Error;

        public ushort ServerTick;

        public bool Dirty;
    }
}
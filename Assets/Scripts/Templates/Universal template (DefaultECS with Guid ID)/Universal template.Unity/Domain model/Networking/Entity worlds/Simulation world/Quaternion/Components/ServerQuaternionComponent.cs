using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity.Networking
{
    public struct ServerQuaternionComponent
    {
        public Quaternion ServerQuaternion;

        public ushort ServerTick;

        public bool Dirty;
    }
}
using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity.Networking
{
    public struct ServerQuaternionComponent
    {
        public Quaternion ServerQuaternion;

        public ushort ServerTick;

        public bool Dirty;
    }
}
using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS.Unity
{
    public interface IVFXManager
    {
        void PopVFX(
            string id,
            Vector3 position);
        
        void PopVFX(
            string id,
            Vector3 position,
            Quaternion rotation);
    }
}
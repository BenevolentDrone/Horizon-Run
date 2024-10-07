using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
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
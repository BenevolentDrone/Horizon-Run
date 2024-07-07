using UnityEngine;

namespace HereticalSolutions.Templates.Universal.Unity
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
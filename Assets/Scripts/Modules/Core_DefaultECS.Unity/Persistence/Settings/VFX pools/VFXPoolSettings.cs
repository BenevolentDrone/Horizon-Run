using UnityEngine;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
    [CreateAssetMenu(
        fileName = "VFX pool settings",
        menuName = "Settings/VFX pools/VFX pool settings",
        order = 0)]
    public class VFXPoolSettings : ScriptableObject
    {
        public VFXElementSettings[] Elements;
    }
}
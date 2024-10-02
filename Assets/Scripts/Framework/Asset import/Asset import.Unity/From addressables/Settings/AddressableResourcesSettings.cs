using UnityEngine;

namespace HereticalSolutions.AssetImport
{
    /// <summary>
    /// Settings for addressable resources.
    /// </summary>
    [CreateAssetMenu(fileName = "Addressable resources settings", menuName = "Settings/Resources/Addressable resources settings", order = 0)]
    public class AddressableResourcesSettings : ScriptableObject
    {
        /// <summary>
        /// Array of addressable resource data settings.
        /// </summary>
        public AddressableResourceDataSettings[] Resources;
    }
}
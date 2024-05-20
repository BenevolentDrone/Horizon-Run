using UnityEngine;

namespace HereticalSolutions.AssetImport
{
    /// <summary>
    /// Represents the settings for multiple resources.
    /// </summary>
    [CreateAssetMenu(fileName = "Resources settings", menuName = "Settings/Resources/Resources settings", order = 0)]
    public class ResourcesSettings : ScriptableObject
    {
        /// <summary>
        /// The settings for the individual resources.
        /// </summary>
        public ResourceDataSettings[] Resources;
    }
}
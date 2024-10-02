using System;

namespace HereticalSolutions.AssetImport
{
    /// <summary>
    /// Represents the settings for a resource variant.
    /// </summary>
    [Serializable]
    public class ResourceVariantDataSettings
    {
        /// <summary>
        /// The identifier of the resource variant.
        /// </summary>
        public string VariantID;

        /// <summary>
        /// The priority of the resource variant. Higher priority variants are preferred over lower priority variants.
        /// </summary>
        public int Priority = 0;
        
        /// <summary>
        /// The UnityEngine.Object representing the resource.
        /// </summary>
        public UnityEngine.Object Resource;
    }
}
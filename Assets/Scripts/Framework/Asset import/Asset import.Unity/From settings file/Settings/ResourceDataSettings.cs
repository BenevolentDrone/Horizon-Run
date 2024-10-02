using System;

namespace HereticalSolutions.AssetImport
{
    /// <summary>
    /// Represents the settings for a resource data.
    /// </summary>
    [Serializable]
    public class ResourceDataSettings
    {
        /// <summary>
        /// The ID of the resource.
        /// </summary>
        public string ResourceID;

        /// <summary>
        /// The settings for the resource variants.
        /// </summary>
        public ResourceVariantDataSettings[] Variants;
    }
}
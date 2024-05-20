using System;

using UnityEngine.AddressableAssets;

namespace HereticalSolutions.AssetImport
{
    /// <summary>
    /// Settings for addressable resource variant data.
    /// </summary>
    [Serializable]
    public class AddressableResourceVariantDataSettings
    {
        /// <summary>
        /// The ID of the variant.
        /// </summary>
        public string VariantID;

        /// <summary>
        /// The priority of the variant. Higher priority variants are preferred over lower priority variants.
        /// </summary>
        public int Priority = 0;
        
        /// <summary>
        /// The asset reference for the variant.
        /// </summary>
        public AssetReference AssetReference;
    }
}
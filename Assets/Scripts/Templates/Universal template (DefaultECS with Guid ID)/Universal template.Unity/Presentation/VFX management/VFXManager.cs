using UnityEngine;

using HereticalSolutions.Pools;

namespace HereticalSolutions.Templates.Universal.Unity
{
    public class VFXManager
        : IVFXManager
    {
        private readonly IManagedPool<GameObject> vfxPool;
        
        
        private readonly AddressArgument addressArgument;
		
        private readonly WorldPositionArgument worldPositionArgument;
        private readonly WorldRotationArgument worldRotationArgument;

        private readonly IPoolPopArgument[] argumentsCache;

        public VFXManager(
            IManagedPool<GameObject> vfxPool,
            AddressArgument addressArgument,
            WorldPositionArgument worldPositionArgument,
            WorldRotationArgument worldRotationArgument,
            IPoolPopArgument[] argumentsCache)
        {
            this.vfxPool = vfxPool;
            
            this.addressArgument = addressArgument;
            
            this.worldPositionArgument = worldPositionArgument;
            this.worldRotationArgument = worldRotationArgument;
            
            this.argumentsCache = argumentsCache;
        }

        #region IVFXManager

        public void PopVFX(
            string id,
            Vector3 position)
        {
            worldPositionArgument.Position = position;
            
            addressArgument.FullAddress = id;

            addressArgument.AddressHashes = id.AddressToHashes();
			
            vfxPool.Pop(
                argumentsCache);
        }

        public void PopVFX(string id, Vector3 position, Quaternion rotation)
        {
            worldPositionArgument.Position = position;
            worldRotationArgument.Rotation = rotation;
            
            addressArgument.FullAddress = id;

            addressArgument.AddressHashes = id.AddressToHashes();
            
            vfxPool.Pop(
                argumentsCache);
        }

        #endregion
    }
}
using UnityEngine;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Arguments;

namespace HereticalSolutions.Templates.Universal.Unity
{
    public class VFXManager
        : IVFXManager
    {
        private readonly INonAllocDecoratedPool<GameObject> vfxPool;
        
        
        private readonly AddressArgument addressArgument;
		
        private readonly WorldPositionArgument worldPositionArgument;
        private readonly WorldRotationArgument worldRotationArgument;

        private readonly IPoolDecoratorArgument[] argumentsCache;

        public VFXManager(
            INonAllocDecoratedPool<GameObject> vfxPool,
            AddressArgument addressArgument,
            WorldPositionArgument worldPositionArgument,
            WorldRotationArgument worldRotationArgument,
            IPoolDecoratorArgument[] argumentsCache)
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
using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Metadata.Allocations;

using UnityEngine;

using Zenject;

namespace HereticalSolutions.Pools.Factories
{
    public static class UnityZenjectDecoratorsPoolsFactory
    {
        #region Templates

        public static INonAllocDecoratedPool<GameObject> BuildSimpleGameObjectPool(
	        DiContainer container,
	        string id,
	        GameObject prefab,
	        Transform poolParent,
	        AllocationCommandDescriptor initialAllocation,
	        AllocationCommandDescriptor additionalAllocation)
        {
	        ResizablePoolBuilder<GameObject> resizablePoolBuilder = new ResizablePoolBuilder<GameObject>();

	        #region Value allocation delegate initialization

	        Func<GameObject> valueAllocationDelegate =
		        () => UnityZenjectAllocationsFactory.DIResolveOrInstantiateAllocationDelegate(
			        container,
			        prefab);

	        #endregion
	        
	        #region Metadata initialization

	        var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
	        {
		        PoolsFactory.BuildIndexedMetadataDescriptor
	        };

	        #endregion

	        #region Allocation callbacks initialization

	        RenameByStringAndIndexCallback renameCallback = UnityDecoratorsPoolsFactory.BuildRenameByStringAndIndexCallback(id);
	        
	        PushToDecoratedPoolCallback<GameObject> pushCallback =
		        PoolsFactory.BuildPushToDecoratedPoolCallback<GameObject>(
			        PoolsFactory.BuildDeferredCallbackQueue<GameObject>());

	        var callbacks = new IAllocationCallback<GameObject>[]
	        {
		        renameCallback,
		        pushCallback
	        };

	        #endregion

	        #region Resizable pool builder initialization

	        resizablePoolBuilder.Initialize(
		        valueAllocationDelegate,
				true,

		        metadataDescriptorBuilders,

		        initialAllocation,
		        additionalAllocation,
				
		        callbacks);
	        
	        #endregion

	        #region Decorator pools initialization

	        var decoratorChain = new NonAllocDecoratorPoolChain<GameObject>();

	        decoratorChain
		        .Add(resizablePoolBuilder.BuildResizablePool())
		        .Add(UnityDecoratorsPoolsFactory.BuildNonAllocGameObjectPool(decoratorChain.TopWrapper, poolParent))
		        .Add(UnityDecoratorsPoolsFactory.BuildNonAllocPrefabInstancePool(decoratorChain.TopWrapper, prefab))
		        .Add(IDDecoratorsPoolsFactory.BuildNonAllocPoolWithID<GameObject>(decoratorChain.TopWrapper, id));

	        var result = decoratorChain.TopWrapper;

	        #endregion

	        #region Dependency injection

	        pushCallback.Root = result;

	        #endregion

	        return result;
        }

        #endregion
    }
}
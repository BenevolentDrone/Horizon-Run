using System;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Metadata.Allocations;

using UnityEngine;

namespace HereticalSolutions.Pools.Factories
{
    public static partial class UnityDecoratorsPoolsFactory
    {
        #region Templates

	    public static INonAllocDecoratedPool<GameObject> BuildSimpleGameObjectPool(
	        string ID,
	        GameObject prefab,
	        Transform poolParent,
	        AllocationCommandDescriptor initialAllocation,
	        AllocationCommandDescriptor additionalAllocation)
        {
	        ResizablePoolBuilder<GameObject> resizablePoolBuilder = new ResizablePoolBuilder<GameObject>();
	        
	        #region Value allocation delegate initialization

	        Func<GameObject> valueAllocationDelegate =
		        () => UnityAllocationsFactory.InstantiatePrefabAllocationDelegate(prefab);

	        #endregion

	        #region Metadata initialization

	        var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
	        {
		        PoolsFactory.BuildIndexedMetadataDescriptor
	        };

	        #endregion

	        #region Allocation callbacks initialization

	        RenameByStringAndIndexCallback renameCallback = BuildRenameByStringAndIndexCallback(ID);
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
		        .Add(BuildNonAllocGameObjectPool(decoratorChain.TopWrapper, poolParent))
		        .Add(BuildNonAllocPrefabInstancePool(decoratorChain.TopWrapper, prefab))
		        .Add(IDDecoratorsPoolsFactory.BuildNonAllocPoolWithID<GameObject>(decoratorChain.TopWrapper, ID));

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
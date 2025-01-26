using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEngine;

namespace HereticalSolutions.Pools.Decorators
{
	public class GameObjectManagedPool
		: ADecoratorManagedPool<GameObject>
	{
		private readonly Transform poolParentTransform;

		public GameObjectManagedPool(
			IManagedPool<GameObject> innerPool,
			Transform parentTransform,
			ILogger logger)
			: base(
				innerPool,
				logger)
		{
			poolParentTransform = parentTransform;
		}

		protected override void OnAfterPop(
			IPoolElementFacade<GameObject> instance,
			IPoolPopArgument[] args)
		{
			var value = instance.Value;

			if (value == null)
				return;

			Transform newParentTransform = null;

			bool worldPositionStays = true;

			#region Parent transform
			
			if (args != null
			    && args.TryGetArgument<ParentTransformArgument>(out var arg1))
			{
				newParentTransform = arg1.Parent;

				worldPositionStays = arg1.WorldPositionStays;
				
				value.transform.SetParent(newParentTransform, worldPositionStays);
			}
			
			#endregion

			#region World position
			
			if (args != null
			    && args.TryGetArgument<WorldPositionArgument>(out var arg2))
			{
				value.transform.position = arg2.Position;
			}
			
			#endregion

			#region World rotation
			
			if (args != null
			    && args.TryGetArgument<WorldRotationArgument>(out var arg3))
			{
				value.transform.rotation = arg3.Rotation;
			}
			
			#endregion

			#region Local position
			
			if (args != null
			    && args.TryGetArgument<LocalPositionArgument>(out var arg4))
			{
				value.transform.localPosition = arg4.Position;
			}
			
			#endregion

			#region Local rotation
			
			if (args != null
			    && args.TryGetArgument<LocalRotationArgument>(out var arg5))
			{
				value.transform.localRotation = arg5.Rotation;
			}
			
			#endregion

			value.SetActive(true);
		}

		protected override void OnBeforePush(IPoolElementFacade<GameObject> instance)
		{
			var value = instance.Value;

			if (value == null)
				return;

			value.SetActive(false);

			if (poolParentTransform != null)
				value.transform.SetParent(poolParentTransform);
			else if (value.transform.parent != null)
				value.transform.SetParent(null);

			value.transform.localPosition = Vector3.zero;

			value.transform.localRotation = Quaternion.identity;
		}
	}
}
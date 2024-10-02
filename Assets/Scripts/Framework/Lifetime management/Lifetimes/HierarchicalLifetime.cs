using System;
using System.Collections.Generic;

using HereticalSolutions.Hierarchy;

using HereticalSolutions.Pools;

using HereticalSolutions.Logging;

namespace HereticalSolutions.LifetimeManagement
{
	public class HierarchicalLifetime
		: ILifetimeable,
		  ISetUppable,
		  IInitializable,
		  ICleanuppable,
		  ITearDownable,
		  ILifetimeTargetProvideable,
		  IHierarchySubject<ILifetimeable>,
		  IDisposable
	{
		private readonly object target;

		private readonly ILifetimeable parentLifetime;
		
		private readonly EInitializationFlags initializationFlags;
		
		private readonly IHierarchyNode<ILifetimeable> hierarchyNode;
		
		private readonly IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool;
		
		private readonly ILogger logger;
		
		
		private Action setUp;

		private Func<object[], bool> initialize;
		
		private Action cleanup;
		
		private Action tearDown;

		
		public HierarchicalLifetime(
			object target,
			IPool<List<IReadOnlyHierarchyNode<ILifetimeable>>> bufferPool,
			
			ILifetimeable parentLifetime = null,
			EInitializationFlags initializationFlags = 
				EInitializationFlags.NO_ARGS_ALLOWED
				| EInitializationFlags.INITIALIZE_ON_PARENT_INITIALIZE
				| EInitializationFlags.INITIALIZE_CHILDREN_ON_INITIALIZE
				| EInitializationFlags.INITIALIZE_WITH_PARENTS_ARGS,
			
			IHierarchyNode<ILifetimeable> hierarchyNode = null,
			
			Action setUp = null,
			Func<object[], bool> initialize = null,
			Action cleanup = null,
			Action tearDown = null,
			
			ILogger logger = null)
		{
			this.target = target;
			
			this.bufferPool = bufferPool;
			
			
			this.parentLifetime = parentLifetime;
			
			this.initializationFlags = initializationFlags;
			
			
			this.hierarchyNode = hierarchyNode;
            
			
			this.logger = logger;
			
			
			this.setUp = setUp;
			
			this.initialize = initialize;
			
			this.cleanup = cleanup;
			
			this.tearDown = tearDown;
			

			if (this.setUp == null && target is ISetUppable)
			{
				this.setUp = (target as ISetUppable).SetUp;
			}
			
			if (this.initialize == null && target is IInitializable)
			{
				this.initialize = (target as IInitializable).Initialize;
			}
			
			if (this.cleanup == null && target is ICleanuppable)
			{
				this.cleanup = (target as ICleanuppable).Cleanup;
			}
			
			if (this.tearDown == null && target is ITearDownable)
			{
				this.tearDown = (target as ITearDownable).TearDown;
			}
		}

		#region ILifetimeable

		public bool IsSetUp { get; private set; } = false;

		public bool IsInitialized { get; private set; } = false;

		public Action<ILifetimeable> OnSetUp { get; set; }
		
		public Action<ILifetimeable, object[]> OnInitialized { get; set; }
		
		public EInitializationFlags InitializationFlags
		{
			get => initializationFlags;
		}

		public Action<ILifetimeable> OnCleanedUp { get; set; }

		public Action<ILifetimeable> OnTornDown { get; set; }

		#endregion

		#region ISetUppable

		public void SetUp()
		{
			if (IsSetUp)
			{
				return;
			}

			setUp?.Invoke();

			IsSetUp = true;
			
			logger?.Log<HierarchicalLifetime>(
				$"SET UP. TARGET: {target}");

			SyncLifetimesWithParent();
			
			OnSetUp?.Invoke(this);
		}

		#endregion

		#region IInitializable

		public bool Initialize(
			object[] args = null)
		{
			if ((args == null || args.Length == 0)
			    && !HasFlags(
				    initializationFlags,
				    EInitializationFlags.NO_ARGS_ALLOWED))
			{
				logger?.LogError<Lifetime>(
					"ATTEMPT TO INITIALIZE WITH EMPTY ARGS AND FLAG \"NO_ARGS_ALLOWED\" NOT SET");

				return false;
			}
			
			if (IsInitialized)
			{
				return true;
			}
			
			if (!IsSetUp)
			{
				SetUp();
			}

			if (initialize != null
			    && !initialize.Invoke(args))
			{
				return false;
			}

			IsInitialized = true;
			
			logger?.Log<HierarchicalLifetime>(
				$"INITIALIZED. TARGET: {target}");

			if (hierarchyNode != null
			    && hierarchyNode.ChildCount > 0
			    && HasFlags(
				    initializationFlags,
				    EInitializationFlags.INITIALIZE_CHILDREN_ON_INITIALIZE))
			{
				var childNodes = bufferPool.Pop();
            
				childNodes.AddRange(hierarchyNode.Children);
				
				foreach (var child in childNodes)
				{
					if (child != null
					    && child.Contents != null)
					{
						if (!HasFlags(
							child.Contents.InitializationFlags,
							EInitializationFlags.INITIALIZE_ON_PARENT_INITIALIZE))
						{
							continue;
						}

						var childAsInitializable = child.Contents as IInitializable;
						
						if (childAsInitializable != null)
						{
							if (HasFlags(
								child.Contents.InitializationFlags,
								EInitializationFlags.INITIALIZE_WITH_PARENTS_ARGS))
							{
								childAsInitializable.Initialize(args);
							}
							else if (HasFlags(
								child.Contents.InitializationFlags,
								EInitializationFlags.NO_ARGS_ALLOWED))
							{
								childAsInitializable.Initialize();
							}
						}
					}
				}
				
				bufferPool.Push(childNodes);
			}

			OnInitialized?.Invoke(this, args);

			return true;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (!IsInitialized)
				return;

			cleanup?.Invoke();

			IsInitialized = false;
			
			logger?.Log<HierarchicalLifetime>(
				$"CLEANED UP. TARGET: {target}");
			
			if (hierarchyNode != null
			    && hierarchyNode.ChildCount > 0)
			{
				var childNodes = bufferPool.Pop();
            
				childNodes.AddRange(hierarchyNode.Children);
				
				foreach (var child in childNodes)
				{
					if (child != null
					    && child.Contents != null)
					{
						var childAsCleanUppable = child.Contents as ICleanuppable;
						
						if (childAsCleanUppable != null)
						{
							childAsCleanUppable.Cleanup();
						}
					}
				}
				
				bufferPool.Push(childNodes);
			}

			OnCleanedUp?.Invoke(this);
		}

		#endregion

		#region ITearDownable

		public void TearDown()
		{
			if (!IsSetUp)
				return;

			if (IsInitialized)
				Cleanup();
            
			tearDown?.Invoke();

			IsSetUp = false;
			
			logger?.Log<HierarchicalLifetime>(
				$"TORN DOWN. TARGET: {target}");
			
			if (hierarchyNode != null
			    && hierarchyNode.ChildCount > 0)
			{
				var childNodes = bufferPool.Pop();
            
				childNodes.AddRange(hierarchyNode.Children);
				
				foreach (var child in childNodes)
				{
					if (child != null
					    && child.Contents != null)
					{
						var childAsTearDownable = child.Contents as ITearDownable;
						
						if (childAsTearDownable != null)
						{
							childAsTearDownable.TearDown();
						}
					}
				}
				
				bufferPool.Push(childNodes);
				
				hierarchyNode.RemoveAllChildren();
				
				if (hierarchyNode.Parent != null)
					hierarchyNode.RemoveParent();
			}
			
			OnTornDown?.Invoke(this);

			
			OnSetUp = null;
            
			OnInitialized = null;

			OnCleanedUp = null;

			OnTornDown = null;
		}

		#endregion

		#region ILifetimeTargetProvideable

		public object Target { get => target; }

		#endregion
		
		#region IHierarchySubject

		public IHierarchyNode<ILifetimeable> HierarchyNode { get => hierarchyNode; }

		#endregion
		
		#region IDisposable

		public void Dispose()
		{
			TearDown();
		}

		#endregion

		private void SyncLifetimesWithParent()
		{
			if (parentLifetime == null)
				return;

			if (hierarchyNode != null 
				&& parentLifetime is IHierarchySubject<ILifetimeable> parentAsHierarchySubject)
			{
				if (hierarchyNode.Parent != null)
				{
					logger?.LogError<Lifetime>(
						"PARENT NODE IS NOT NULL");
				}
				else
				{
					parentAsHierarchySubject.HierarchyNode.AddChild(hierarchyNode);
				}
			}
			else
			{
				SyncLifetimesWithParentViaDelegates();
			}
			
			if (parentLifetime.IsSetUp
				&& !IsSetUp)
				SetUp();
			
			if (HasFlags(
				    initializationFlags,
				    EInitializationFlags.INITIALIZE_ON_PARENT_INITIALIZE
						| EInitializationFlags.NO_ARGS_ALLOWED)
			    && parentLifetime.IsInitialized
			    && !IsInitialized)
				Initialize();
		}

		private void SyncLifetimesWithParentViaDelegates()
		{
			bool initializeOnParentInitialize = HasFlags(
				initializationFlags,
				EInitializationFlags.INITIALIZE_ON_PARENT_INITIALIZE
					| EInitializationFlags.INITIALIZE_WITH_PARENTS_ARGS);
			
			Action<ILifetimeable> setUpOnParentSetUpDelegate = (parent) => SetUp();

			Action<ILifetimeable, object[]> initializeOnParentSetUpDelegate = null;
			
			if (initializeOnParentInitialize)
				initializeOnParentSetUpDelegate = (parent, args) => Initialize(args);
			
			Action<ILifetimeable> cleanupOnParentCleanedUpDelegate = (parent) => Cleanup();
			
			Action<ILifetimeable> tearDownOnParentTornDownDelegate = (parent) => TearDown();
			
			
			parentLifetime.OnSetUp += setUpOnParentSetUpDelegate;
			
			if (initializeOnParentInitialize)
				parentLifetime.OnInitialized += initializeOnParentSetUpDelegate;
			
			parentLifetime.OnCleanedUp += cleanupOnParentCleanedUpDelegate;
			
			parentLifetime.OnTornDown += tearDownOnParentTornDownDelegate;
			
			
			Action<ILifetimeable> desyncDelegate = null;

			desyncDelegate = (parent) =>
			{
				parentLifetime.OnSetUp -= setUpOnParentSetUpDelegate;
			
				if (initializeOnParentInitialize)
					parentLifetime.OnInitialized -= initializeOnParentSetUpDelegate;
			
				parentLifetime.OnCleanedUp -= cleanupOnParentCleanedUpDelegate;
			
				parentLifetime.OnTornDown -= tearDownOnParentTornDownDelegate;

				
				parentLifetime.OnTornDown -= desyncDelegate;
			};

			parentLifetime.OnTornDown += desyncDelegate;
		}
		
		private static bool HasFlags(
			EInitializationFlags currentFlags,
			EInitializationFlags flagsToCheckAgainst)
		{
			return (currentFlags & flagsToCheckAgainst) != 0;
		}
	}
}
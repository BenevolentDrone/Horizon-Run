using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;
using HereticalSolutions.Allocations;

namespace HereticalSolutions.Time
{
	public class TimerManager
		: ITimerManager,
		  ICleanuppable,
		  IDisposable
	{
		private readonly string timerManagerID;


		private readonly UShortIDAllocationController idAllocationController;

		private readonly IRepository<int, IPoolElementFacade<TimerWithSubscriptionsContainer>> timerContainerRepository;

		private readonly IRepository<string, List<DurationHandlePair>> sharedTimerHandleRepository;
		
		private readonly IManagedPool<TimerWithSubscriptionsContainer> timerContainersPool;

		private readonly bool renameTimersOnPop;
		
		public TimerManager(
			string timerManagerID,
			UShortIDAllocationController idAllocationController,
			IRepository<int, IPoolElementFacade<TimerWithSubscriptionsContainer>> timerContainerRepository,
			IRepository<string, List<DurationHandlePair>> sharedTimerHandleRepository,
			IManagedPool<TimerWithSubscriptionsContainer> timerContainersPool,
			bool renameTimersOnPop = true)
		{
			this.timerManagerID = timerManagerID;

			this.idAllocationController = idAllocationController;

			this.timerContainerRepository = timerContainerRepository;
			
			this.sharedTimerHandleRepository = sharedTimerHandleRepository;

			this.timerContainersPool = timerContainersPool;

			this.renameTimersOnPop = renameTimersOnPop;
		}

		#region ITimerManager

		public string ID { get => timerManagerID;}

		public bool CreateTimer(
			out ushort timerHandle,
			out IRuntimeTimer timer)
		{
			idAllocationController.AllocateID(
				out timerHandle);


			var pooledTimerContainer = timerContainersPool.Pop();

			timerContainerRepository.Add(
				timerHandle,
				pooledTimerContainer);

			var timerContainer = pooledTimerContainer.Value;

			timer = timerContainer.Timer;

			if (!timerContainer.StartTimerSubscription.Active)
			{
				timerContainer.OnStartPrivateSubscribable.Subscribe(
					timerContainer.StartTimerSubscription);
			}

			if (!timerContainer.FinishTimerSubscription.Active)
			{
				timerContainer.OnFinishPrivateSubscribable.Subscribe(
					timerContainer.FinishTimerSubscription);
			}

			timer.Accumulate = false;
			
			timer.Repeat = false;

			timer.FlushTimeElapsedOnRepeat = false;

			timer.FireRepeatCallbackOnFinish = true;
			
			if (renameTimersOnPop)
			{
				var renameableTimer = timerContainer.Timer as IRenameableTimer;

				if (renameableTimer != null)
				{
					string timerStringID = $"{timerManagerID} timer #{timerHandle}";

					renameableTimer.ID = timerStringID;
				}
			}

			return true;
		}

		public bool GetOrCreateSharedTimer(
			string timerID,
			float expectedDuration,
			out ushort timerHandle,
			out IRuntimeTimer timer,
			out bool newInstance)
		{
			if (sharedTimerHandleRepository.TryGet(
				timerID,
				out var sharedTimers))
			{
				foreach (var candidate in sharedTimers)
				{
					if (Math.Abs(candidate.Duration - expectedDuration) < MathHelpers.EPSILON)
					{
						timerHandle = candidate.Handle;
						
						if (!TryGetTimer(
							timerHandle,
							out timer))
						{
							CreateTimer(
								out timerHandle,
								out timer);
							
							candidate.Handle = timerHandle;
						
							newInstance = true;
							
							return true;
						}
						
						newInstance = false;

						return true;
					}
				}
				
				CreateTimer(
					out timerHandle,
					out timer);
				
				sharedTimers.Add(
					new DurationHandlePair
					{
						Duration = expectedDuration,
						Handle = timerHandle
					});

				newInstance = true;
				
				if (timer is IRenameableTimer)
					(timer as IRenameableTimer).ID = timerID;

				return true;
			}
			
			CreateTimer(
				out timerHandle,
				out timer);

			newInstance = true;
            
			if (timer is IRenameableTimer)
				(timer as IRenameableTimer).ID = timerID;
			
			var sharedTimersList = new List<DurationHandlePair>
			{
				new DurationHandlePair
				{
					Duration = expectedDuration,
					Handle = timerHandle
				}
			};
			
			sharedTimerHandleRepository.Add(
				timerID,
				sharedTimersList);

			return true;
		}

		public bool TryGetTimer(
			ushort timerHandle,
			out IRuntimeTimer timer)
		{
			var result = timerContainerRepository.TryGet(
				timerHandle,
				out var pooledTimerContainer);

			timer = pooledTimerContainer?.Value.Timer;

			return result;
		}

		public bool TryDestroyTimer(
			ushort timerHandle)
		{
			if (!timerContainerRepository.TryGet(
				timerHandle,
				out var pooledTimerContainer))
			{
				return false;
			}

			var timer = pooledTimerContainer.Value.Timer; 
            
			timer.Reset();

			timer.Repeat = false;
			
			timer.Accumulate = false;
			
			timer.FlushTimeElapsedOnRepeat = false;

			timer.FireRepeatCallbackOnFinish = true;

			//pooledTimerContainer.Value.Timer.OnStart.Unsubscribe(
			//	pooledTimerContainer.Value.StartTimerSubscription);
			
			//pooledTimerContainer.Value.Timer.OnFinish.Unsubscribe(
			//	pooledTimerContainer.Value.FinishTimerSubscription);
			
			//if (pooledTimerContainer.Value.Timer is ICleanUppable)
			//	(pooledTimerContainer.Value.Timer as ICleanUppable).Cleanup();
			
			pooledTimerContainer.Value.Timer.OnStart.UnsubscribeAll();

			if (pooledTimerContainer.Value.StartTimerSubscription.Active)
			{
				pooledTimerContainer.Value.OnStartPrivateSubscribable.Unsubscribe(
					pooledTimerContainer.Value.StartTimerSubscription);
			}

			pooledTimerContainer.Value.Timer.OnStartRepeated.UnsubscribeAll();
			
			
			pooledTimerContainer.Value.Timer.OnFinish.UnsubscribeAll();
			
			if (!pooledTimerContainer.Value.FinishTimerSubscription.Active)
			{
				pooledTimerContainer.Value.OnFinishPrivateSubscribable.Unsubscribe(
					pooledTimerContainer.Value.FinishTimerSubscription);
			}
			
			pooledTimerContainer.Value.Timer.OnFinishRepeated.UnsubscribeAll();

			
			pooledTimerContainer.Push();

			timerContainerRepository.TryRemove(timerHandle);

			idAllocationController.FreeID(
				timerHandle);
			
			
			return true;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (timerContainerRepository is ICleanuppable)
				(timerContainerRepository as ICleanuppable).Cleanup();

			if (timerContainersPool is ICleanuppable)
				(timerContainersPool as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (timerContainerRepository is IDisposable)
				(timerContainerRepository as IDisposable).Dispose();

			if (timerContainersPool is IDisposable)
				(timerContainersPool as IDisposable).Dispose();
		}

		#endregion
	}
}
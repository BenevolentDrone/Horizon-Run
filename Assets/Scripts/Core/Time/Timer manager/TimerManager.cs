using System;

using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Repositories;

using HereticalSolutions.Pools;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.Time
{
	public class TimerManager
		: ITimerManager,
		  ICleanUppable,
		  IDisposable
	{
		private readonly string timerManagerID;


		private readonly IRepository<ushort, IPoolElement<TimerWithSubscriptionsContainer>> timerContainersRepository;

		private readonly INonAllocDecoratedPool<TimerWithSubscriptionsContainer> timerContainersPool;

		private readonly bool renameTimersOnPop;
		
		public TimerManager(
			string timerManagerID,
			IRepository<ushort, IPoolElement<TimerWithSubscriptionsContainer>> timerContainersRepository,
			INonAllocDecoratedPool<TimerWithSubscriptionsContainer> timerContainersPool,
			bool renameTimersOnPop = true)
		{
			this.timerManagerID = timerManagerID;

			this.timerContainersRepository = timerContainersRepository;

			this.timerContainersPool = timerContainersPool;

			this.renameTimersOnPop = renameTimersOnPop;
		}

		#region ITimerManager

		public string ID { get => timerManagerID;}

		public bool CreateTimer(
			out ushort timerHandle,
			out IRuntimeTimer timer)
		{
			timerHandle = 0;

			do
			{
				timerHandle = IDAllocationsFactory.BuildUshort();
			}
			while (timerHandle == 0
				|| timerContainersRepository.Has(timerHandle));


			var pooledTimerContainer = timerContainersPool.Pop();

			timerContainersRepository.Add(
				timerHandle,
				pooledTimerContainer);

			var timerContainer = pooledTimerContainer.Value;

			timer = timerContainer.Timer;
			
			timer.OnStart.Subscribe(
				timerContainer.StartTimerSubscription);
			
			timer.OnFinish.Subscribe(
				timerContainer.FinishTimerSubscription);

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

		public bool TryGetTimer(
			ushort timerHandle,
			out IRuntimeTimer timer)
		{
			var result = timerContainersRepository.TryGet(
				timerHandle,
				out var pooledTimerContainer);

			timer = pooledTimerContainer?.Value.Timer;

			return result;
		}

		public bool TryDestroyTimer(
			ushort timerHandle)
		{
			if (!timerContainersRepository.TryGet(
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

			//pooledTimerContainer.Value.Timer.OnStart.Unsubscribe(
			//	pooledTimerContainer.Value.StartTimerSubscription);
			
			//pooledTimerContainer.Value.Timer.OnFinish.Unsubscribe(
			//	pooledTimerContainer.Value.FinishTimerSubscription);
			
			//if (pooledTimerContainer.Value.Timer is ICleanUppable)
			//	(pooledTimerContainer.Value.Timer as ICleanUppable).Cleanup();
			
			pooledTimerContainer.Value.Timer.OnStart.UnsubscribeAll();
			
			pooledTimerContainer.Value.Timer.OnFinish.UnsubscribeAll();

			pooledTimerContainer.Push();

			timerContainersRepository.TryRemove(timerHandle);
			
			
			return true;
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			if (timerContainersRepository is ICleanUppable)
				(timerContainersRepository as ICleanUppable).Cleanup();

			if (timerContainersPool is ICleanUppable)
				(timerContainersPool as ICleanUppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (timerContainersRepository is IDisposable)
				(timerContainersRepository as IDisposable).Dispose();

			if (timerContainersPool is IDisposable)
				(timerContainersPool as IDisposable).Dispose();
		}

		#endregion
	}
}
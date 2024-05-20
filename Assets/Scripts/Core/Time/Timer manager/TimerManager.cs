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


		private readonly IRepository<int, IPoolElement<TimerWithSubscriptionsContainer>> timerContainersRepository;

		private readonly INonAllocDecoratedPool<TimerWithSubscriptionsContainer> timerContainersPool;

		private readonly bool renameTimersOnPop;
		
		public TimerManager(
			string timerManagerID,
			IRepository<int, IPoolElement<TimerWithSubscriptionsContainer>> timerContainersRepository,
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
			out int timerID,
			out IRuntimeTimer timer)
		{
			timerID = -1;

			do
			{
				timerID = IDAllocationsFactory.BuildInt();
			}
			while (timerContainersRepository.Has(timerID));


			var pooledTimerContainer = timerContainersPool.Pop();

			timerContainersRepository.Add(
				timerID,
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
					string timerStringID = $"{timerManagerID} timer #{timerID}";

					renameableTimer.ID = timerStringID;
				}
			}

			return true;
		}

		public bool TryGetTimer(
			int timerID,
			out IRuntimeTimer timer)
		{
			var result = timerContainersRepository.TryGet(
				timerID,
				out var pooledTimerContainer);

			timer = pooledTimerContainer?.Value.Timer;

			return result;
		}

		public bool TryDestroyTimer(
			int timerID)
		{
			if (!timerContainersRepository.TryGet(
				timerID,
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

			timerContainersRepository.TryRemove(timerID);
			
			
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
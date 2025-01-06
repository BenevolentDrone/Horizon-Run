/*
using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Repositories
{
	[Visitor(typeof(DictionaryRepository<,>), typeof(RepositoryDTO))]
	public class RuntimeTimerVisitor<TKey, TValue>
		: ISaveVisitor,
		  ILoadVisitor,
		  IPopulateVisitor
	{
		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public RuntimeTimerVisitor(
			ILoggerResolver loggerResolver = null,
			ILogger logger = null)
		{
			this.loggerResolver = loggerResolver;

			this.logger = logger;
		}

		#region IVisitor

		public bool CanVisit<TVisitable>()
		{
			return typeof(TVisitable) == typeof(DictionaryRepository<TKey, TValue>);
		}

		public bool CanVisit(
			Type visitableType)
		{
			return visitableType == typeof(DictionaryRepository<TKey, TValue>);
		}

		public Type GetDTOType<TVisitable>()
		{
			if (typeof(TVisitable) != typeof(DictionaryRepository<TKey, TValue>))
				return null;

			return typeof(DictionaryRepository<TKey, TValue>);
		}

		public Type GetDTOType(
			Type visitableType)
		{
			if (visitableType != typeof(DictionaryRepository<TKey, TValue>))
				return null;

			return typeof(DictionaryRepository<TKey, TValue>);
		}

		#endregion

		#region ISaveVisitor

		public bool VisitSave<TVisitable>(
			ref object dto,
			TVisitable visitable)
		{
			RuntimeTimer timer = visitable as RuntimeTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(RuntimeTimer).Name}");

				dto = null;

				return false;
			}

			dto = new RuntimeTimerDTO
			{
				ID = timer.ID,
				State = timer.State,
				CurrentTimeElapsed = ((IRuntimeTimerContext)timer).CurrentTimeElapsed,
				Accumulate = timer.Accumulate,
				Repeat = timer.Repeat,
				CurrentDuration = timer.CurrentDuration,
				DefaultDuration = timer.DefaultDuration
			};

			return true;
		}

		public bool VisitSave(
			ref object dto,
			Type visitableType,
			object visitableObject)
		{
			RuntimeTimer timer = visitableObject as RuntimeTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(RuntimeTimer).Name}");

				dto = null;

				return false;
			}

			dto = new RuntimeTimerDTO
			{
				ID = timer.ID,
				State = timer.State,
				CurrentTimeElapsed = ((IRuntimeTimerContext)timer).CurrentTimeElapsed,
				Accumulate = timer.Accumulate,
				Repeat = timer.Repeat,
				CurrentDuration = timer.CurrentDuration,
				DefaultDuration = timer.DefaultDuration
			};

			return true;
		}

		#endregion

		#region ILoadVisitor

		public bool VisitLoad<TVisitable>(
			object dto,
			out TVisitable visitable)
		{
			RuntimeTimerDTO castedDTO = dto as RuntimeTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(RuntimeTimerDTO).Name}");

				visitable = default;

				return false;
			}

			var timer = TimeFactory.BuildRuntimeTimer(
				castedDTO.ID,
				castedDTO.DefaultDuration,
				loggerResolver);

			((ITimerWithState)timer).SetState(castedDTO.State);

			((IRuntimeTimerContext)timer).CurrentTimeElapsed = castedDTO.CurrentTimeElapsed;

			((IRuntimeTimerContext)timer).CurrentDuration = castedDTO.CurrentDuration;

			timer.Accumulate = castedDTO.Accumulate;

			timer.Repeat = castedDTO.Repeat;

			timer.FlushTimeElapsedOnRepeat = castedDTO.FlushTimeElapsedOnRepeat;

			timer.FireRepeatCallbackOnFinish = castedDTO.FireRepeatCallbackOnFinish;

			visitable = timer.CastFromTo<RuntimeTimer, TVisitable>();

			return true;
		}

		public bool VisitLoad(
			object dto,
			Type visitableType,
			out object visitableObject)
		{
			RuntimeTimerDTO castedDTO = dto as RuntimeTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(RuntimeTimerDTO).Name}");

				visitableObject = default;

				return false;
			}

			var timer = TimeFactory.BuildRuntimeTimer(
				castedDTO.ID,
				castedDTO.DefaultDuration,
				loggerResolver);

			((ITimerWithState)timer).SetState(castedDTO.State);

			((IRuntimeTimerContext)timer).CurrentTimeElapsed = castedDTO.CurrentTimeElapsed;

			((IRuntimeTimerContext)timer).CurrentDuration = castedDTO.CurrentDuration;

			timer.Accumulate = castedDTO.Accumulate;

			timer.Repeat = castedDTO.Repeat;

			timer.FlushTimeElapsedOnRepeat = castedDTO.FlushTimeElapsedOnRepeat;

			timer.FireRepeatCallbackOnFinish = castedDTO.FireRepeatCallbackOnFinish;

			visitableObject = timer;

			return true;
		}

		#endregion

		#region IPopulateVisitor

		public bool VisitPopulate<TVisitable>(
			object dto,
			TVisitable visitable)
		{
			RuntimeTimerDTO castedDTO = dto as RuntimeTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(RuntimeTimerDTO).Name}");

				return false;
			}

			RuntimeTimer timer = visitable as RuntimeTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(RuntimeTimer).Name}");

				return false;
			}

			((ITimerWithState)timer).SetState(castedDTO.State);

			((IRuntimeTimerContext)timer).CurrentTimeElapsed = castedDTO.CurrentTimeElapsed;

			((IRuntimeTimerContext)timer).CurrentDuration = castedDTO.CurrentDuration;

			timer.Accumulate = castedDTO.Accumulate;

			timer.Repeat = castedDTO.Repeat;

			timer.FlushTimeElapsedOnRepeat = castedDTO.FlushTimeElapsedOnRepeat;

			timer.FireRepeatCallbackOnFinish = castedDTO.FireRepeatCallbackOnFinish;

			return true;
		}

		public bool VisitPopulate(
			object dto,
			Type visitableType,
			object visitableObject)
		{
			RuntimeTimerDTO castedDTO = dto as RuntimeTimerDTO;

			if (castedDTO == null)
			{
				logger?.LogError(
					GetType(),
					$"DTO IS NOT OF TYPE: {typeof(RuntimeTimerDTO).Name}");

				return false;
			}

			RuntimeTimer timer = visitableObject as RuntimeTimer;

			if (timer == null)
			{
				logger?.LogError(
					GetType(),
					$"VISITABLE IS NOT OF TYPE: {typeof(RuntimeTimer).Name}");

				return false;
			}

			((ITimerWithState)timer).SetState(castedDTO.State);

			((IRuntimeTimerContext)timer).CurrentTimeElapsed = castedDTO.CurrentTimeElapsed;

			((IRuntimeTimerContext)timer).CurrentDuration = castedDTO.CurrentDuration;

			timer.Accumulate = castedDTO.Accumulate;

			timer.Repeat = castedDTO.Repeat;

			timer.FlushTimeElapsedOnRepeat = castedDTO.FlushTimeElapsedOnRepeat;

			timer.FireRepeatCallbackOnFinish = castedDTO.FireRepeatCallbackOnFinish;

			return true;
		}

		#endregion
	}
}
*/
using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time
{
    [Visitor(typeof(PersistentTimer), typeof(PersistentTimerDTO))]
    public class PersistentTimerVisitor
        : ISaveVisitor,
          ILoadVisitor,
          IPopulateVisitor
    {
        private readonly ILoggerResolver loggerResolver;

        private readonly ILogger logger;

        public PersistentTimerVisitor(
            ILoggerResolver loggerResolver = null,
            ILogger logger = null)
        {
            this.loggerResolver = loggerResolver;

            this.logger = logger;
        }

        #region IVisitor

        public bool CanVisit<TVisitable>()
        {
            return typeof(TVisitable) == typeof(PersistentTimer);
        }

        public bool CanVisit(
            Type visitableType)
        {
            return visitableType == typeof(PersistentTimer);
        }

        public Type GetDTOType<TVisitable>()
        {
            if (typeof(TVisitable) != typeof(PersistentTimer))
                return null;

            return typeof(PersistentTimerDTO);
        }

        public Type GetDTOType(
            Type visitableType)
        {
            if (visitableType != typeof(PersistentTimer))
                return null;

            return typeof(PersistentTimerDTO);
        }

        #endregion

        #region ISaveVisitor

        public bool VisitSave<TVisitable>(
            ref object dto,
            TVisitable visitable)
        {
            PersistentTimer timer = visitable as PersistentTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {typeof(PersistentTimer).Name}");

                dto = null;

                return false;
            }

            dto = new PersistentTimerDTO
            {
                ID = timer.ID,
                State = timer.State,
                StartTime = ((IPersistentTimerContext)timer).StartTime,
                EstimatedFinishTime = ((IPersistentTimerContext)timer).EstimatedFinishTime,
                SavedProgress = ((IPersistentTimerContext)timer).SavedProgress,
                Accumulate = timer.Accumulate,
                Repeat = timer.Repeat,
                CurrentDurationSpan = timer.CurrentDurationSpan,
                DefaultDurationSpan = timer.DefaultDurationSpan
            };

            return true;
        }

        public bool VisitSave(
            ref object dto,
            Type visitableType,
            object visitableObject)
        {
            PersistentTimer timer = visitableObject as PersistentTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {typeof(PersistentTimer).Name}");

                dto = null;

                return false;
            }

            dto = new PersistentTimerDTO
            {
                ID = timer.ID,
                State = timer.State,
                StartTime = ((IPersistentTimerContext)timer).StartTime,
                EstimatedFinishTime = ((IPersistentTimerContext)timer).EstimatedFinishTime,
                SavedProgress = ((IPersistentTimerContext)timer).SavedProgress,
                Accumulate = timer.Accumulate,
                Repeat = timer.Repeat,
                CurrentDurationSpan = timer.CurrentDurationSpan,
                DefaultDurationSpan = timer.DefaultDurationSpan
            };

            return true;
        }

        #endregion

        #region ILoadVisitor

        public bool VisitLoad<TVisitable>(
            object dto,
            out TVisitable visitable)
        {
            PersistentTimerDTO castedDTO = dto as PersistentTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {typeof(PersistentTimerDTO).Name}");

                visitable = default;

                return false;
            }

            var timer = TimerFactory.BuildPersistentTimer(
                castedDTO.ID,
                castedDTO.DefaultDurationSpan,
                loggerResolver);

            ((ITimerWithState)timer).SetState(castedDTO.State);

            ((IPersistentTimerContext)timer).StartTime = castedDTO.StartTime;

            ((IPersistentTimerContext)timer).EstimatedFinishTime = castedDTO.EstimatedFinishTime;

            ((IPersistentTimerContext)timer).SavedProgress = castedDTO.SavedProgress;

            ((IPersistentTimerContext)timer).CurrentDurationSpan = castedDTO.CurrentDurationSpan;

            timer.Accumulate = castedDTO.Accumulate;

            timer.Repeat = castedDTO.Repeat;

            timer.FlushTimeElapsedOnRepeat = castedDTO.FlushTimeElapsedOnRepeat;

            timer.FireRepeatCallbackOnFinish = castedDTO.FireRepeatCallbackOnFinish;

            visitable = timer.CastFromTo<PersistentTimer, TVisitable>();

            return true;
        }

        public bool VisitLoad(
            object dto,
            Type visitableType,
            out object visitableObject)
        {
            PersistentTimerDTO castedDTO = dto as PersistentTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {typeof(PersistentTimerDTO).Name}");

                visitableObject = default;

                return false;
            }

            var timer = TimerFactory.BuildPersistentTimer(
                castedDTO.ID,
                castedDTO.DefaultDurationSpan,
                loggerResolver);

            ((ITimerWithState)timer).SetState(castedDTO.State);

            ((IPersistentTimerContext)timer).StartTime = castedDTO.StartTime;

            ((IPersistentTimerContext)timer).EstimatedFinishTime = castedDTO.EstimatedFinishTime;

            ((IPersistentTimerContext)timer).SavedProgress = castedDTO.SavedProgress;

            ((IPersistentTimerContext)timer).CurrentDurationSpan = castedDTO.CurrentDurationSpan;

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
            PersistentTimerDTO castedDTO = dto as PersistentTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {typeof(PersistentTimerDTO).Name}");

                return false;
            }

            PersistentTimer timer = visitable as PersistentTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {typeof(PersistentTimer).Name}");

                return false;
            }

            ((ITimerWithState)timer).SetState(castedDTO.State);

            ((IPersistentTimerContext)timer).StartTime = castedDTO.StartTime;

            ((IPersistentTimerContext)timer).EstimatedFinishTime = castedDTO.EstimatedFinishTime;

            ((IPersistentTimerContext)timer).SavedProgress = castedDTO.SavedProgress;

            ((IPersistentTimerContext)timer).CurrentDurationSpan = castedDTO.CurrentDurationSpan;

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
            PersistentTimerDTO castedDTO = dto as PersistentTimerDTO;

            if (castedDTO == null)
            {
                logger?.LogError(
                    GetType(),
                    $"DTO IS NOT OF TYPE: {typeof(PersistentTimerDTO).Name}");

                return false;
            }

            PersistentTimer timer = visitableObject as PersistentTimer;

            if (timer == null)
            {
                logger?.LogError(
                    GetType(),
                    $"VISITABLE IS NOT OF TYPE: {typeof(PersistentTimer).Name}");

                return false;
            }

            ((ITimerWithState)timer).SetState(castedDTO.State);

            ((IPersistentTimerContext)timer).StartTime = castedDTO.StartTime;

            ((IPersistentTimerContext)timer).EstimatedFinishTime = castedDTO.EstimatedFinishTime;

            ((IPersistentTimerContext)timer).SavedProgress = castedDTO.SavedProgress;

            ((IPersistentTimerContext)timer).CurrentDurationSpan = castedDTO.CurrentDurationSpan;

            timer.Accumulate = castedDTO.Accumulate;

            timer.Repeat = castedDTO.Repeat;

            timer.FlushTimeElapsedOnRepeat = castedDTO.FlushTimeElapsedOnRepeat;

            timer.FireRepeatCallbackOnFinish = castedDTO.FireRepeatCallbackOnFinish;

            return true;
        }

        #endregion
    }
}
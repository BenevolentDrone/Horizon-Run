using System;

using HereticalSolutions.Persistence;

using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time
{
    [Visitor(typeof(RuntimeTimer), typeof(RuntimeTimerDTO))]
    public class RuntimeTimerVisitor
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
            return typeof(TVisitable) == typeof(RuntimeTimer);
        }

        public bool CanVisit(
            Type visitableType)
        {
            return visitableType == typeof(RuntimeTimer);
        }

        public Type GetDTOType<TVisitable>()
        {
            if (typeof(TVisitable) != typeof(RuntimeTimer))
                return null;

            return typeof(RuntimeTimerDTO);
        }

        public Type GetDTOType(
            Type visitableType)
        {
            if (visitableType != typeof(RuntimeTimer))
                return null;

            return typeof(RuntimeTimerDTO);
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

            var timer = TimerFactory.BuildRuntimeTimer(
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

            var timer = TimerFactory.BuildRuntimeTimer(
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
using System;

using HereticalSolutions.Logging;

namespace HereticalSolutions.StanleyScript
{
	public class LoggerWrapperWithContext
		: ILoggerWrapper
	{
		private readonly IStanleyContext context;

		private ILogger innerLogger;

		public LoggerWrapperWithContext(
			IStanleyContext context)
		{
			this.context = context;
		}
		
		#region ILoggerWrapper

		public ILogger InnerLogger
		{
			get => innerLogger;
			set => innerLogger = value;
		}

		#endregion

		#region ILogger

		#region Log

		public void Log(
			string value)
		{

			value = FormatLogWithData(
				value,
				ELogType.LOG);

			innerLogger?.Log(value);
		}

		public void Log<TSource>(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.LOG);

			innerLogger?.Log<TSource>(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.LOG);

			innerLogger?.Log(
				logSource,
				value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.LOG);

			innerLogger?.Log(
				value,
				arguments);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.LOG);

			innerLogger?.Log<TSource>(
				value,
				arguments);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.LOG);

			innerLogger?.Log(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.WARNING);

			innerLogger?.LogWarning(
				value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.WARNING);

			innerLogger?.LogWarning<TSource>(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.WARNING);

			innerLogger?.LogWarning(
				logSource,
				value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.WARNING);

			innerLogger?.LogWarning(
				value,
				arguments);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.WARNING);

			innerLogger?.LogWarning<TSource>(
				value,
				arguments);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.WARNING);

			innerLogger?.LogWarning(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.ERROR);

			innerLogger?.LogError(
				value);
		}

		public void LogError<TSource>(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.ERROR);

			innerLogger?.LogError<TSource>(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.ERROR);

			innerLogger?.LogError(
				logSource,
				value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.ERROR);

			innerLogger?.LogError(
				value,
				arguments);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.ERROR);

			innerLogger?.LogError<TSource>(
				value,
				arguments);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			value = FormatLogWithData(
				value,
				ELogType.ERROR);

			innerLogger?.LogError(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.EXCEPTION);

			return innerLogger.FormatException(value);
		}

		public string FormatException<TSource>(
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.EXCEPTION);

			return innerLogger.FormatException<TSource>(value);
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			value = FormatLogWithData(
				value,
				ELogType.EXCEPTION);

			return innerLogger.FormatException(
				logSource,
				value);
		}

		#endregion

		#endregion

		private string FormatLogWithData(
			string value,
			ELogType logType)
		{
			return $"[LINE {context.StackMachine.LineCounter}] [PC {context.StackMachine.ProgramCounter}] {value}";
		}
	}
}
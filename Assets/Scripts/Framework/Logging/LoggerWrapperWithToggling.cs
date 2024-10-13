using System;

namespace HereticalSolutions.Logging
{
	public class LoggerWrapperWithToggling
		: ILogger,
		  ILoggerWrapper
	{
		private readonly ILogger innerLogger;

		public bool Active { get; set; }

		public bool LogsActive { get; set; }

		public bool WarningsActive { get; set; }

		public bool ErrorsActive { get; set; }

		public LoggerWrapperWithToggling(
			ILogger innerLogger,
			bool active = true,
			bool printLogs = true,
			bool printWarnings = true,
			bool printErrors = true)
		{
			this.innerLogger = innerLogger;

			Active = active;

			LogsActive = printLogs;

			WarningsActive = printWarnings;

			ErrorsActive = printErrors;
		}

		#region ILoggerWrapper

		public ILogger InnerLogger { get => innerLogger; }

		#endregion

		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!LogsActive)
				return;

			innerLogger.Log(value);
		}

		public void Log<TSource>(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!LogsActive)
				return;

			innerLogger.Log<TSource>(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!LogsActive)
				return;

			innerLogger.Log(
				logSource,
				value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!LogsActive)
				return;

			innerLogger.Log(
				value,
				arguments);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!LogsActive)
				return;

			innerLogger.Log<TSource>(
				value,
				arguments);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!LogsActive)
				return;

			innerLogger.Log(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!WarningsActive)
				return;

			innerLogger.LogWarning(
				value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			if (!Active)
			{
				return;
			}

			innerLogger.LogWarning<TSource>(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!WarningsActive)
				return;

			innerLogger.LogWarning(
				logSource,
				value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!WarningsActive)
				return;

			innerLogger.LogWarning(
				value,
				arguments);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!WarningsActive)
				return;

			innerLogger.LogWarning<TSource>(
				value,
				arguments);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!WarningsActive)
				return;

			innerLogger.LogWarning(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!ErrorsActive)
				return;

			innerLogger.LogError(
				value);
		}

		public void LogError<TSource>(
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!ErrorsActive)
				return;

			innerLogger.LogError<TSource>(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			if (!Active)
			{
				return;
			}

			if (!ErrorsActive)
				return;

			innerLogger.LogError(
				logSource,
				value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!ErrorsActive)
				return;

			innerLogger.LogError(
				value,
				arguments);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!ErrorsActive)
				return;

			innerLogger.LogError<TSource>(
				value,
				arguments);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (!Active)
			{
				return;
			}

			if (!ErrorsActive)
				return;

			innerLogger.LogError(
				logSource,
				value,
				arguments);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			return innerLogger.FormatException(value);
		}

		public string FormatException<TSource>(
			string value)
		{
			return innerLogger.FormatException<TSource>(value);
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			return innerLogger.FormatException(
				logSource,
				value);
		}

		#endregion

		#endregion
	}
}
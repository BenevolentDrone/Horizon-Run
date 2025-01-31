using System;
using System.Collections.Generic;

namespace HereticalSolutions.Logging
{
	public class CompositeLoggerWrapper
		: ICompositeLoggerWrapper
	{
		private readonly List<ILogger> innerLoggers;

		public CompositeLoggerWrapper(
			List<ILogger> innerLoggers)
		{
			this.innerLoggers = innerLoggers;
		}

		#region ICompositeLoggerWrapper

		public List<ILogger> InnerLoggers { get => innerLoggers; }

		#endregion

		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.Log(value);
		}

		public void Log<TSource>(
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.Log<TSource>(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.Log(
					logSource,
					value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.Log(
					value,
					arguments);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.Log<TSource>(
					value,
					arguments);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
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
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogWarning(
					value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogWarning<TSource>(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogWarning(
					logSource,
					value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogWarning(
					value,
					arguments);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogWarning<TSource>(
					value,
					arguments);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
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
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogError(
					value);
		}

		public void LogError<TSource>(
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogError<TSource>(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogError(
					logSource,
					value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogError(
					value,
					arguments);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
				innerLogger?.LogError<TSource>(
					value,
					arguments);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			foreach (var innerLogger in innerLoggers)
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
			return value;
		}

		public string FormatException<TSource>(
			string value)
		{
			return value;
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			return value;
		}

		#endregion

		#endregion
	}
}
using System;
using System.Diagnostics;

namespace HereticalSolutions.Logging
{
	public static class LoggingExtensions
	{
		public static string TryFormatException(
			this ILogger logger,
			string value)
		{
			if (logger != null)
			{
				var stackTrace = new StackTrace();

				//In case a static method calls a static method that raises the exception
				for (int i = 1; i < stackTrace.FrameCount; i++)
				{
					var frame = stackTrace.GetFrame(i);

					var declaringType = frame.GetMethod().DeclaringType;

					if (declaringType != null
						&& declaringType != typeof(void))
					{
						value = logger.FormatException(
							declaringType,
							value);

						break;
					}
				}

				value = logger.FormatException(value);
			}

			return value;
		}

		public static string TryFormatException<TSource>(
			this ILogger logger,
			string value)
		{
			if (logger != null)
				value = logger.FormatException<TSource>(value);

			return value;
		}

		public static string TryFormatException(
			this ILogger logger,
			Type logSource,
			string value)
		{
			if (logger != null)
				value = logger.FormatException(
					logSource,
					value);

			return value;
		}
	}
}
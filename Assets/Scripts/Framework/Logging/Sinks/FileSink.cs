using System;

using HereticalSolutions.Persistence;

namespace HereticalSolutions.Logging
{
	public class FileSink
		: ILoggerSink
	{
		private readonly ISerializer serializer;

		public FileSink(
			ISerializer serializer)
		{
			this.serializer = serializer;
		}

		public ISerializer Serializer => serializer;

		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			serializer.Serialize<string>(
				$"{value}\n");
		}

		public void Log<TSource>(
			string value)
		{
			serializer.Serialize<string>(
				$"{value}\n");
		}

		public void Log(
			Type logSource,
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void LogError<TSource>(
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			SerializeOnNewLine(value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			SerializeOnNewLine(value);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			SerializeOnNewLine(value);

			return value;	
		}

		public string FormatException<TSource>(
			string value)
		{
			SerializeOnNewLine(value);

			return value;
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			SerializeOnNewLine(value);

			return value;
		}

		#endregion

		#endregion

		private void SerializeOnNewLine(string value)
		{
			serializer.Serialize<string>(
				$"{value}\n");
		}
	}
}
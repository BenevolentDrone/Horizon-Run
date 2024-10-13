using System;

using HereticalSolutions.Logging;

using UnityEngine.UI;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public class UGUISink
		: ILoggerSink
	{
		private readonly Text logTextComponent;

		private readonly Text warningTextComponent;

		private readonly Text errorTextComponent;

		public UGUISink(
			Text logTextComponent = null,
			Text warningTextComponent = null,
			Text errorTextComponent = null)
		{
			this.logTextComponent = logTextComponent;

			this.warningTextComponent = warningTextComponent;

			this.errorTextComponent = errorTextComponent;
		}

		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			if (logTextComponent != null)
				logTextComponent.text = value;
		}

		public void Log<TSource>(
			string value)
		{
			Log(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			Log(value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			Log(value);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			Log(value);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			Log(value);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			if (warningTextComponent != null)
				warningTextComponent.text = value;
		}

		public void LogWarning<TSource>(
			string value)
		{
			LogWarning(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			LogWarning(value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			LogWarning(value);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			LogWarning(value);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			LogWarning(value);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			if (errorTextComponent != null)
				errorTextComponent.text = value;
		}

		public void LogError<TSource>(
			string value)
		{
			LogError(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			LogError(value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			LogError(value);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			LogError(value);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			LogError(value);
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
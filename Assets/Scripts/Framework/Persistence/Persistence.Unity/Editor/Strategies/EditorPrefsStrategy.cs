using System;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEditor;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class EditorPrefsStrategy
		: ISerializationStrategy,
		  IStrategyWithIODestination
	{
		private static readonly Type[] allowedValueTypes = new Type[]
		{
			typeof(string)
		};

		private readonly string keyPrefs;

		private readonly ILogger logger;

		public EditorPrefsStrategy(
			string keyPrefs,
			ILogger logger)
		{
			this.keyPrefs = keyPrefs;

			this.logger = logger;
		}

		#region ISerializationStrategy

		public Type[] AllowedValueTypes { get => allowedValueTypes; }

		#region Read

		public bool Read<TValue>(
			out TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue));

			if (EditorPrefs.HasKey(keyPrefs))
			{
				value = EditorPrefs.GetString(keyPrefs).CastFromTo<string, TValue>();

				return true;
			}

			value = default;

			return false;
		}

		public bool Read(
			Type valueType,
			out object value)
		{
			AssertStrategyIsValid(
				valueType);

			if (EditorPrefs.HasKey(keyPrefs))
			{
				value = EditorPrefs.GetString(keyPrefs);

				return true;
			}

			value = default;

			return false;
		}

		#endregion

		#region Write

		public bool Write<TValue>(
			TValue value)
		{
			AssertStrategyIsValid(
				typeof(TValue));

			EditorPrefs.SetString(
				keyPrefs,
				value.CastFromTo<TValue, string>());

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType);

			EditorPrefs.SetString(
				keyPrefs,
				value.CastFromTo<object, string>());

			return true;
		}

		#endregion

		#region Append

		public bool Append<TValue>(
			TValue value)
		{
			throw new NotSupportedException();
		}

		public bool Append(
			Type valueType,
			object value)
		{
			throw new NotSupportedException();
		}

		#endregion

		#endregion

		#region IStrategyWithIODestination

		public void EnsureIOTargetDestinationExists()
		{
			//Do nothing
		}

		public bool IOTargetExists()
		{
			return EditorPrefs.HasKey(keyPrefs);
		}

		public void CreateIOTarget()
		{
			//Do nothing
		}

		public void EraseIOTarget()
		{
			EditorPrefs.DeleteKey(keyPrefs);
		}

		#endregion

		private void AssertStrategyIsValid(
			Type valueType)
		{
			if (valueType != typeof(string))
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						$"INVALID VALUE TYPE: {valueType.Name}"));
		}
	}
}
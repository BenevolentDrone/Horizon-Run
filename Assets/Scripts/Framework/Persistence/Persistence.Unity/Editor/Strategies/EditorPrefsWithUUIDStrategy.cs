using System;
using System.Linq;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

using UnityEditor;

using Newtonsoft.Json;

namespace HereticalSolutions.Persistence
{
	[SerializationStrategy]
	public class EditorPrefsWithUUIDStrategy<TUUID>
		: ISerializationStrategy,
		  IStrategyWithIODestination
	{
		private static readonly Type[] allowedValueTypes = new Type[]
		{
			typeof(string)
		};

		private readonly string keyPrefsSerializedValuesList;

		private readonly string keyPrefsValuePrefix;

		private readonly TUUID uuid;

		private readonly ILogger logger;
		
		public EditorPrefsWithUUIDStrategy(
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID uuid,
			ILogger logger = null)
		{
			this.keyPrefsValuePrefix = keyPrefsValuePrefix;

			this.keyPrefsSerializedValuesList = keyPrefsSerializedValuesList;

			this.uuid = uuid;

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

			var serializedValues = GetSerializedValuesListFromEditorPrefs(
				keyPrefsSerializedValuesList);

			if (serializedValues.Length != 0)
			{
				if (TryDeserializeValueFromEditorPrefs(
					keyPrefsSerializedValuesList,
					keyPrefsValuePrefix,
					uuid,
					serializedValues,
					out string valueString))
				{
					value = valueString.CastFromTo<string, TValue>();

					return true;
				}
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

			var serializedValues = GetSerializedValuesListFromEditorPrefs(
				keyPrefsSerializedValuesList);

			if (serializedValues.Length != 0)
			{
				if (TryDeserializeValueFromEditorPrefs(
					keyPrefsSerializedValuesList,
					keyPrefsValuePrefix,
					uuid,
					serializedValues,
					out string valueString))
				{
					value = valueString;

					return true;
				}
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

			var saves = GetSerializedValuesListFromEditorPrefs(
				keyPrefsSerializedValuesList);

			SerializeValueToEditorPrefs(
				keyPrefsSerializedValuesList,
				keyPrefsValuePrefix,
				uuid,
				saves,
				value.CastFromTo<TValue, string>());

			return true;
		}

		public bool Write(
			Type valueType,
			object value)
		{
			AssertStrategyIsValid(
				valueType);

			var saves = GetSerializedValuesListFromEditorPrefs(
				keyPrefsSerializedValuesList);

			SerializeValueToEditorPrefs(
				keyPrefsSerializedValuesList,
				keyPrefsValuePrefix,
				uuid,
				saves,
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
			var serializedValuesList = GetSerializedValuesListFromEditorPrefs(
				keyPrefsSerializedValuesList);

			string key = String.Format(
				keyPrefsValuePrefix,
				uuid.ToString());

			return serializedValuesList.Contains(key) && EditorPrefs.HasKey(key);
		}

		public void CreateIOTarget()
		{
			//Do nothing
		}

		public void EraseIOTarget()
		{
			EraseAllSerializedValuesFromEditorPrefs(
				keyPrefsSerializedValuesList);
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

		public string[] GetSerializedValuesListFromEditorPrefs(
			string keyPrefsSerializedValuesList)
		{
			if (!EditorPrefs.HasKey(keyPrefsSerializedValuesList))
			{
				return new string[0];
			}

			var serializedValuesListJson = EditorPrefs.GetString(keyPrefsSerializedValuesList);

			string[] result = null;

			try
			{
				result = JsonConvert.DeserializeObject<string[]>(serializedValuesListJson);
			}
			catch (Exception e)
			{
				logger?.LogError(
					GetType(),
					e.Message);
			}

			if (result == null)
			{
				result = new string[0];
			}

			return result;
		}

		public void SerializeValueToEditorPrefs(
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID valueUUID,
			string[] serializedValuesList,
			string value)
		{
			string key = String.Format(
				keyPrefsValuePrefix,
				valueUUID.ToString());

			EditorPrefs.SetString(
				key,
				value);

			var newSerializedValuesList = serializedValuesList.Append(key).ToArray();

			string valuesListJson = string.Empty;

			try
			{
				valuesListJson = JsonConvert.SerializeObject(newSerializedValuesList);
			}
			catch (Exception e)
			{
				logger?.LogError(
					GetType(),
					e.Message);
			}

			EditorPrefs.SetString(
				keyPrefsSerializedValuesList,
				valuesListJson);
		}

		public bool TryDeserializeValueFromEditorPrefs(
			string keyPrefsSerializedValuesList,
			string keyPrefsValuePrefix,
			TUUID valueUUID,
			string[] serializedValuesList,
			out string value)
		{
			bool success = false;

			value = string.Empty;

			string key = String.Format(
				keyPrefsValuePrefix,
				valueUUID.ToString());

			if (serializedValuesList.Contains(key))
			{
				if (EditorPrefs.HasKey(key))
				{
					value = EditorPrefs.GetString(key);

					success = true;
				}

				EditorPrefs.DeleteKey(key);


				var newSerializedValuesList = serializedValuesList.Where(x => x != key).ToArray();

				string valuesListJson = string.Empty;

				try
				{
					valuesListJson = JsonConvert.SerializeObject(newSerializedValuesList);
				}
				catch (Exception e)
				{
					logger?.LogError(
						GetType(),
						e.Message);
				}

				EditorPrefs.SetString(
					keyPrefsSerializedValuesList,
					valuesListJson);
			}

			return success;
		}

		public void EraseAllSerializedValuesFromEditorPrefs(
			string keyPrefsSerializedValuesList)
		{
			var serializedValuesList = GetSerializedValuesListFromEditorPrefs(
				keyPrefsSerializedValuesList);

			EditorPrefs.DeleteKey(keyPrefsSerializedValuesList);

			if (serializedValuesList == null || serializedValuesList.Length == 0)
			{
				return;
			}

			foreach (var key in serializedValuesList)
			{
				EditorPrefs.DeleteKey(key);
			}
		}
	}
}
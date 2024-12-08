using HereticalSolutions.Persistence;
using HereticalSolutions.Persistence.Factories;

using HereticalSolutions.Time;

using UnityEditor;

using UnityEngine;

namespace HereticalSolutions.Samples.RuntimeTimerWithSerializationSample
{
	[CustomEditor(typeof(RuntimeTimerWithSerializationSampleBehaviour))]
	[CanEditMultipleObjects]
	public class RuntimeTimerWithSerializationSampleBehaviourEditor : UnityEditor.Editor
	{
		private static readonly string[] pathOptions = new string[]
		{
			"Absolute path",
			"Relative path",
			"Temp path",
			"Application data path",
			"Persistent data path"
		};

		private static readonly string[] formatOptions = new string[]
		{
			"Object",
			"Binary",
			"JSON",
			"XML",
			"CSV",
			"YAML",
			"Protobuf"
		};

		private static readonly string[] mediaTypeOptions = new string[]
		{
			"String",
			"Text file",
			"Binary file",
			"Text stream",
			"File stream",
			"Memory stream",
			"Isolated storage",
			"Player prefs",
			"Editor prefs",
			"Editor prefs with UUID"
		};

		private int selectedPathOptionIndex = 0;

		private int selectedFormatOptionIndex = 0;

		private int selectedMediaTypeOptionIndex = 0;

		void OnEnable()
		{
			UpdateSerializer();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			base.OnInspectorGUI();

			EditorGUIUtility.labelWidth = 120;

			EditorGUILayout.LabelField(
				"Persistence",
				EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal();

			bool dirty = DrawPathOptions();

			dirty |= DrawFormatOptions();

			dirty |= DrawMediaTypeOptions();

			EditorGUILayout.EndHorizontal();

			if (dirty)
			{
				UpdateSerializer();
			}

			DrawControls();

			serializedObject.ApplyModifiedProperties();
		}

		private bool DrawPathOptions()
		{
			bool dirty = false;

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField(
				"Path options",
				EditorStyles.boldLabel);

			int previousSelectedPathOptionIndex = selectedPathOptionIndex;

			for (int i = 0; i < pathOptions.Length; i++)
			{
				if (i == previousSelectedPathOptionIndex)
				{
					GUI.enabled = false;
				}

				if (GUILayout.Button(
					pathOptions[i],
					selectedPathOptionIndex == i
						? EditorStyles.miniButtonMid
						: EditorStyles.miniButton))
					//GUILayout.Width(200)))
				{
					selectedPathOptionIndex = i;

					dirty = true;
				}

				if (i == previousSelectedPathOptionIndex)
				{
					GUI.enabled = true;
				}
			}

			EditorGUILayout.EndVertical();

			return dirty;
		}

		private bool DrawFormatOptions()
		{
			bool dirty = false;

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField(
				"Format options",
				EditorStyles.boldLabel);

			int previousSelectedFormatOptionIndex = selectedFormatOptionIndex;

			for (int i = 0; i < formatOptions.Length; i++)
			{
				if (i == previousSelectedFormatOptionIndex)
				{
					GUI.enabled = false;
				}

				if (GUILayout.Button(
					formatOptions[i],
					selectedFormatOptionIndex == i
						? EditorStyles.miniButtonMid
						: EditorStyles.miniButton))
				{
					selectedFormatOptionIndex = i;

					dirty = true;
				}

				if (i == previousSelectedFormatOptionIndex)
				{
					GUI.enabled = true;
				}
			}

			EditorGUILayout.EndVertical();

			return dirty;
		}

		private bool DrawMediaTypeOptions()
		{
			bool dirty = false;

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField(
				"Media type options",
				EditorStyles.boldLabel);

			int previousSelectedMediaTypeOptionIndex = selectedMediaTypeOptionIndex;

			for (int i = 0; i < mediaTypeOptions.Length; i++)
			{
				if (i == previousSelectedMediaTypeOptionIndex)
				{
					GUI.enabled = false;
				}

				if (GUILayout.Button(
					mediaTypeOptions[i],
					selectedMediaTypeOptionIndex == i
						? EditorStyles.miniButtonMid
						: EditorStyles.miniButton))
				{
					selectedMediaTypeOptionIndex = i;

					dirty = true;
				}

				if (i == previousSelectedMediaTypeOptionIndex)
				{
					GUI.enabled = true;
				}
			}

			EditorGUILayout.EndVertical();

			return dirty;
		}

		private void UpdateSerializer()
		{
			var targetCasted = (RuntimeTimerWithSerializationSampleBehaviour)target;

			var loggerResolver = targetCasted.LoggerResolver;

			var serializerBuilder = PersistenceFactory.BuildSerializerBuilder(
				loggerResolver);

			serializerBuilder.NewSerializer();

			switch (selectedPathOptionIndex)
			{
				case 0:
					serializerBuilder.FromAbsolutePath(
						targetCasted.FileAtAbsolutePathSettings);

					break;

				case 1:
					serializerBuilder.FromRelativePath(
						targetCasted.FileAtRelativePathSettings);

					break;

				case 2:
					serializerBuilder.FromTempPath(
						targetCasted.FileAtTempPathSettings);

					break;

				case 3:
					serializerBuilder.FromApplicationDataPath(
						targetCasted.FileAtApplicationDataPathSettings);

					break;

				case 4:
					serializerBuilder.FromPersistentDataPath(
						targetCasted.FileAtPersistentDataPathSettings);

					break;
			}

			switch (selectedFormatOptionIndex)
			{
				case 0:
					serializerBuilder.ToObject();

					break;

				case 1:
					serializerBuilder.ToBinary();

					break;

				case 2:
					serializerBuilder.ToJSON();

					break;

				case 3:
					serializerBuilder.ToXML();

					break;

				case 4:
					serializerBuilder.ToCSV();

					break;

				case 5:
					serializerBuilder.ToYAML();

					break;

				case 6:
					serializerBuilder.ToProtobuf();

					break;
			}

			switch (selectedMediaTypeOptionIndex)
			{
				case 0:
					serializerBuilder.AsString();

					break;

				case 1:
					serializerBuilder.AsTextFile();

					break;

				case 2:
					serializerBuilder.AsBinaryFile();

					break;

				case 3:
					serializerBuilder.AsTextStream();

					break;

				case 4:
					serializerBuilder.AsFileStream();

					break;

				case 5:
					serializerBuilder.AsMemoryStream();

					break;

				case 6:
					serializerBuilder.AsIsolatedStorageFileStream();

					break;

				case 7:
					serializerBuilder.AsPlayerPrefs(
						"RuntimeTimerWithSerializationSampleBehaviour");

					break;

				case 8:
					serializerBuilder.AsEditorPrefs(
						"RuntimeTimerWithSerializationSampleBehaviour");

					break;

				case 9:
					serializerBuilder.AsEditorPrefsWithUUID(
						"RuntimeTimerWithSerializationSampleBehaviour_Values",
						"RuntimeTimerWithSerializationSampleBehaviour_Value_",
						this.GetInstanceID());

					break;
			}

			targetCasted.Serializer = serializerBuilder.Build();
		}

		private void DrawControls()
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Save"))
			{
				var targetCasted = (RuntimeTimerWithSerializationSampleBehaviour)target;

				RuntimeTimer runtimeTimer = (RuntimeTimer)targetCasted.RuntimeTimer;

				targetCasted.Logger?.Log<RuntimeTimerWithSerializationSampleBehaviour>(
					$"TIMER CHECKUP. PROGRESS: {runtimeTimer.CurrentTimeElapsed} STATE: {runtimeTimer.State}");

				bool result = targetCasted.Serializer.Serialize<RuntimeTimer>(
					runtimeTimer);

				if (targetCasted.Serializer.Context.SerializationStrategy is IStrategyWithStream strategyWithStream)
				{
					strategyWithStream.FinalizeWrite();
				}

				targetCasted.Logger?.Log<RuntimeTimerWithSerializationSampleBehaviour>(
					$"SERIALIZED: {result}");
			}

			if (GUILayout.Button("Load"))
			{
				var targetCasted = (RuntimeTimerWithSerializationSampleBehaviour)target;

				RuntimeTimer runtimeTimer = (RuntimeTimer)targetCasted.RuntimeTimer;

				bool result = targetCasted.Serializer.Populate<RuntimeTimer>(
					ref runtimeTimer);

				if (targetCasted.Serializer.Context.SerializationStrategy is IStrategyWithStream strategyWithStream)
				{
					strategyWithStream.FinalizeRead();
				}

				targetCasted.Logger?.Log<RuntimeTimerWithSerializationSampleBehaviour>(
					$"POPULATED: {result}");

				targetCasted.Logger?.Log<RuntimeTimerWithSerializationSampleBehaviour>(
					$"TIMER CHECKUP. PROGRESS: {runtimeTimer.CurrentTimeElapsed} STATE: {runtimeTimer.State}");
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}
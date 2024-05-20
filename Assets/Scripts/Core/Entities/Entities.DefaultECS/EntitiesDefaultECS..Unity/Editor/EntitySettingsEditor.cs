using System;
using System.Collections.Generic;
using System.Reflection;

using DefaultEcs;

using HereticalSolutions.Persistence.Arguments;
using HereticalSolutions.Persistence.Factories;
using HereticalSolutions.Persistence.Serializers;

using HereticalSolutions.Logging;
using HereticalSolutions.Logging.Factories;

using UnityEditor;
using UnityEditor.Experimental.GraphView;

using UnityEngine;

namespace HereticalSolutions.Entities.Editor
{
	[CustomEditor(typeof(EntitySettings))]
	[CanEditMultipleObjects]
	public class EntitySettingsEditor : UnityEditor.Editor
	{
		private const bool DEBUG_OPERATION = true;

		private ComponentTypesProvider componentTypesProvider;

		private EntityPrototypeDTO entityPrototypeDTO;

		private ILoggerResolver loggerResolver;

		private JSONSerializer jsonSerializer;

		private StringArgument stringArgument;

		
		private GUIStyle structNameLabelStyle;

		private GUIStyle commandStructNameLabelStyle;
		
		private GUIStyle serverAuthoredStructNameLabelStyle;
		
		private GUIStyle serverAuthoredOnInitializationStructNameLabelStyle;
		
		private GUIStyle eventAuthoredStructNameLabelStyle;
		
		private GUIStyle clientDisabledStructNameLabelStyle;

		void OnEnable()
		{
			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log("[EntitySettingsEditor] OnEnable");

			if (componentTypesProvider == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new ComponentTypesProvider");

				componentTypesProvider = new ComponentTypesProvider();

				componentTypesProvider.OnComponentSelected += OnComponentTypeToAddSelected;
			}

			if (loggerResolver == null)
			{
				ILoggerBuilder loggerBuilder = LoggersFactory.BuildLoggerBuilder();

				loggerBuilder
					.ToggleAllowedByDefault(true)
					.AddOrWrap(
						LoggersFactoryUnity.BuildUnityDebugLogger())
					.AddOrWrap(
						LoggersFactory.BuildLoggerWrapperWithLogTypePrefix(
							loggerBuilder.CurrentLogger))
					.AddOrWrap(
						LoggersFactory.BuildLoggerWrapperWithSourceTypePrefix(
							loggerBuilder.CurrentLogger));

				loggerResolver = (ILoggerResolver)loggerBuilder;
			}

			if (jsonSerializer == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new JSONSerializer");

				jsonSerializer = UnityPersistenceFactory.BuildSimpleUnityJSONSerializer(loggerResolver);
			}

			if (stringArgument == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new StringArgument");

				stringArgument = new StringArgument();
			}

			if (structNameLabelStyle == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new GUIStyle");

				structNameLabelStyle = new GUIStyle();
				structNameLabelStyle.fontSize = 12;
				structNameLabelStyle.fontStyle = FontStyle.Bold;
				structNameLabelStyle.normal.textColor = Color.white;
			}

			if (commandStructNameLabelStyle == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new GUIStyle");

				commandStructNameLabelStyle = new GUIStyle();
				commandStructNameLabelStyle.fontStyle = FontStyle.Bold;
				commandStructNameLabelStyle.fontSize = 10;
				commandStructNameLabelStyle.normal.textColor = new Color(0f, 0.353f, 1f);
			}
			
			if (serverAuthoredStructNameLabelStyle == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new GUIStyle");

				serverAuthoredStructNameLabelStyle = new GUIStyle();
				serverAuthoredStructNameLabelStyle.fontStyle = FontStyle.Bold;
				serverAuthoredStructNameLabelStyle.fontSize = 10;
				serverAuthoredStructNameLabelStyle.normal.textColor = new Color(1f, 0.647f, 0f);
			}
			
			if (serverAuthoredOnInitializationStructNameLabelStyle == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new GUIStyle");

				serverAuthoredOnInitializationStructNameLabelStyle = new GUIStyle();
				serverAuthoredOnInitializationStructNameLabelStyle.fontStyle = FontStyle.Bold;
				serverAuthoredOnInitializationStructNameLabelStyle.fontSize = 10;
				serverAuthoredOnInitializationStructNameLabelStyle.normal.textColor = new Color(1f, 0.549f, 0f);
			}
			
			if (eventAuthoredStructNameLabelStyle == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new GUIStyle");

				eventAuthoredStructNameLabelStyle = new GUIStyle();
				eventAuthoredStructNameLabelStyle.fontStyle = FontStyle.Bold;
				eventAuthoredStructNameLabelStyle.fontSize = 10;
				eventAuthoredStructNameLabelStyle.normal.textColor = new Color(0f, 1f, 0.647f);
			}
			
			if (clientDisabledStructNameLabelStyle == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Creating new GUIStyle");

				clientDisabledStructNameLabelStyle = new GUIStyle();
				clientDisabledStructNameLabelStyle.fontStyle = FontStyle.Bold;
				clientDisabledStructNameLabelStyle.fontSize = 10;
				clientDisabledStructNameLabelStyle.normal.textColor = new Color(1f, 0f, 0.353f);
			}

			//globalDirty = false;

			Deserialize();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			bool dirty = false;

			string prototypeID = EditorGUILayout.TextField(
				entityPrototypeDTO.PrototypeID);

			if (prototypeID != entityPrototypeDTO.PrototypeID)
			{
				entityPrototypeDTO.PrototypeID = prototypeID;

				dirty = true;

				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] PrototypeID dirty");
			}

			if (DrawComponents())
			{
				dirty = true;

				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] DrawComponents dirty");
			}

			EditorGUILayout.Space(10f);

			if (GUILayout.Button("ERASE"))
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Erasing");

				Erase();
			}

			if (GUILayout.Button("ADD COMPONENT"))
			{
				SearchWindow.Open(
					new SearchWindowContext(
						GUIUtility.GUIToScreenPoint(
							Event.current.mousePosition)),
					componentTypesProvider);
			}

			if (dirty)
			{
				Serialize();

				dirty = false;

				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] dirty reset");
			}

			serializedObject.ApplyModifiedProperties();
		}

		private bool DrawComponents()
		{
			if (entityPrototypeDTO.Components == null)
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] entityPrototypeDTO has no components");

				Erase();
			}

			bool localDirty = false;

			for (int i = 0; i < entityPrototypeDTO.Components.Length; i++)
			{
				if (DrawStruct(
						ref entityPrototypeDTO.Components[i],
						i))
					localDirty = true;
			}

			return localDirty;
		}

		private bool DrawStruct(
			ref object structObject,
			int index,
			int level = 0)
		{
			Type structType = structObject.GetType();

			var fields = structType.GetFields(
				BindingFlags.Public
				| BindingFlags.Instance);

			if (level == 0)
				EditorGUILayout.BeginVertical("Box");
			else
				EditorGUILayout.BeginVertical("Window");

			EditorGUILayout.BeginHorizontal();

			bool isCommandStruct = structType.GetCustomAttribute<InitializationCommandComponentAttribute>(false) != null;

			bool isServerAuthoredStruct = structType.GetCustomAttribute<ServerAuthoredComponentAttribute>(false) != null;

			bool isServerAuthoredOnInitializationStruct = structType.GetCustomAttribute<ServerAuthoredOnInitializationComponent>(false) != null;
			
			bool isEventAuthoredStruct = structType.GetCustomAttribute<EventAuthoredComponentAttribute>(false) != null;
			
			bool isClientDisabledStruct = structType.GetCustomAttribute<ClientDisabledComponentAttribute>(false) != null;
			
			GUIStyle selectedStyle = structNameLabelStyle;
			
			/*
			if (isCommandStruct)
				selectedStyle = commandStructNameLabelStyle;

			if (isServerAuthoredStruct)
				selectedStyle = serverAuthoredStructNameLabelStyle;
			*/
				
			EditorGUILayout.LabelField(
				structType.Name,
				selectedStyle);

			bool modified = false;

			if (level == 0
				&& index != -1)
			{
				if (GUILayout.Button("UP"))
				{
					if (Event.current.shift)
					{
						MoveComponentToTheTop(index);
					}
					else if (Event.current.control)
					{
						MoveComponentUpX5(index);
					}
					else
					{
						MoveComponentUp(index);
					}
					
					modified = true;
				}

				if (GUILayout.Button("DOWN"))
				{
					if (Event.current.shift)
					{
						MoveComponentToTheBottom(index);
					}
					else if (Event.current.control)
					{
						MoveComponentDownX5(index);
					}
					else
					{
						MoveComponentDown(index);
					}

					modified = true;
				}

				if (GUILayout.Button("REMOVE"))
				{
					RemoveComponent(index);

					modified = true;
				}
			}

			EditorGUILayout.EndVertical();

			if (isCommandStruct)
			{
				EditorGUILayout.LabelField(
					"[InitializationCommandComponent]",
					commandStructNameLabelStyle);
			}
			
			if (isServerAuthoredStruct)
			{
				EditorGUILayout.LabelField(
					"[ServerAuthoredComponent]",
					serverAuthoredStructNameLabelStyle);
			}
			
			if (isServerAuthoredOnInitializationStruct)
			{
				EditorGUILayout.LabelField(
					"[ServerAuthoredOnInitializationComponent]",
					serverAuthoredOnInitializationStructNameLabelStyle);
			}
			
			if (isEventAuthoredStruct)
			{
				EditorGUILayout.LabelField(
					"[EventAuthoredComponent]",
					eventAuthoredStructNameLabelStyle);
			}
			
			if (isClientDisabledStruct)
			{
				EditorGUILayout.LabelField(
					"[ClientDisabledComponent]",
					clientDisabledStructNameLabelStyle);
			}

			EditorGUILayout.Space(10f);

			bool localDirty = false;

			if (!modified)
			{
				foreach (var fieldInfo in fields)
				{
					if (DrawField(
							ref structObject,
							fieldInfo,
							level))
						localDirty = true;
				}
			}

			EditorGUILayout.EndVertical();

			return localDirty;
		}

		private bool DrawField(
			ref object structObject,
			FieldInfo fieldInfo,
			int level)
		{
			var value = fieldInfo.GetValue(structObject);

			var valueType = fieldInfo.FieldType;

			var fieldName = fieldInfo.Name;

			bool localDirty = false;

			switch (value)
			{
				case Guid guidValue:
					EditorGUI.BeginDisabledGroup(true);

					EditorGUILayout.TextField(
						fieldName,
						guidValue.ToString());

					EditorGUI.EndDisabledGroup();

					break;

				case Entity entityValue:
					EditorGUI.BeginDisabledGroup(true);

					EditorGUILayout.TextField(
						fieldName,
						entityValue.ToString());

					EditorGUI.EndDisabledGroup();

					break;

				case Enum enumValue:
					var enumResult = EditorGUILayout.EnumPopup(fieldName, enumValue);

					if (!Equals(enumResult, enumValue))
					{
						fieldInfo.SetValue(structObject, enumResult);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] enum field {fieldName} dirty");
					}

					break;

				case byte byteValue:

					int byteResultAsInt = EditorGUILayout.IntField(
						fieldName,
						(byte)value);

					if (byteResultAsInt != byteValue
					    && byteResultAsInt >= 0
					    && byteResultAsInt <= byte.MaxValue)
					{
						fieldInfo.SetValue(structObject, (byte)byteResultAsInt);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] byte field {fieldName} dirty");
					}

					break;
				
				case ushort ushortValue:

					int ushortResultAsInt = EditorGUILayout.IntField(
						fieldName,
						(ushort)value);

					if (ushortResultAsInt != ushortValue
					    && ushortResultAsInt >= 0
					    && ushortResultAsInt <= ushort.MaxValue)
					{
						fieldInfo.SetValue(structObject, (ushort)ushortResultAsInt);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] ushort field {fieldName} dirty");
					}

					break;
                
				case int intValue:

					int intResult = EditorGUILayout.IntField(
						fieldName,
						(int)value);

					if (intResult != intValue)
					{
						fieldInfo.SetValue(structObject, intResult);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] int field {fieldName} dirty");
					}

					break;

				case long longValue:

					long longResult = EditorGUILayout.LongField(
						fieldName,
						(long)value);

					if (longResult != longValue)
					{
						fieldInfo.SetValue(structObject, longResult);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] long field {fieldName} dirty");
					}

					break;

				case float floatValue:

					float floatResult = EditorGUILayout.FloatField(
						fieldName,
						(float)value);

					if (floatResult != floatValue)
					{
						fieldInfo.SetValue(structObject, floatResult);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] float field {fieldName} dirty");
					}

					break;

				case double doubleValue:

					double doubleResult = EditorGUILayout.DoubleField(
						fieldName,
						(double)value);

					if (doubleResult != doubleValue)
					{
						fieldInfo.SetValue(structObject, doubleResult);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] double field {fieldName} dirty");
					}

					break;

				case bool boolValue:

					bool boolResult = EditorGUILayout.Toggle(
						fieldName,
						(bool)value);

					if (boolResult != boolValue)
					{
						fieldInfo.SetValue(structObject, boolResult);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] bool field {fieldName} dirty");
					}

					break;

				//FOR SOME REASON ITS NOT EVEN ENTERING THIS CASE
				//NEITHER string NOR String WORK
				/*
                case string stringValue:

                    string stringResult = EditorGUILayout.TextField(
                        fieldName,
                        (string)value);

                    if (stringResult != stringValue)
                    {
                        fieldInfo.SetValue(structObject, stringResult);

                        localDirty = true;
                    }

                    break;
                */

				case Vector2 vector2Value:

					Vector2 vector2Result = EditorGUILayout.Vector2Field(
						fieldName,
						(Vector2)value);

					if (vector2Result != vector2Value)
					{
						fieldInfo.SetValue(structObject, vector2Result);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] Vector2 field {fieldName} dirty");
					}

					break;

				case Vector3 vector3Value:

					Vector3 vector3Result = EditorGUILayout.Vector3Field(
						fieldName,
						(Vector3)value);

					if (vector3Result != vector3Value)
					{
						fieldInfo.SetValue(structObject, vector3Result);

						localDirty = true;

						if (DEBUG_OPERATION)
							UnityEngine.Debug.Log($"[EntitySettingsEditor] Vector3 field {fieldName} dirty");
					}

					break;

				//Layer mask in a component? Blah
				/*
				case LayerMask layerMask:
					LayerMask temp =
						EditorGUILayout.MaskField(
							fieldName,
							InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask),
							InternalEditorUtility.layers);

					var result = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(temp);

					if (result != layerMask)
					{
						fieldInfo.SetValue(structObject, result);

						localDirty = true;

						UnityEngine.Debug.Log($"[EntitySettingsEditor] layer mask field {fieldName} dirty");
					}

					break;
				*/

				//Arrays in a component may be a problem for network serialization
				/*case Array array:
                    if(array == null)
                        array = (Array)Activator.CreateInstance(fieldInfo.FieldType);

                    foreach (var property in array)
                    {
                        foreach (var propertyInfo in property.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            EditorGUILayout.LabelField(propertyInfo.Name);
                        }
                    }
                    
                    break;*/

				default:

					if (valueType == typeof(string))
					{
						var stringValue = (string)value;

						string stringResult = EditorGUILayout.TextField(
							fieldName,
							(string)value);

						if (stringResult != stringValue)
						{
							fieldInfo.SetValue(structObject, stringResult);

							localDirty = true;

							if (DEBUG_OPERATION)
								UnityEngine.Debug.Log($"[EntitySettingsEditor] string field {fieldName} dirty");
						}

						break;
					}

					//Enumerables may have the same problems as arrays
					//TODO: implement fixed-size buffers instead of arrays
					//Courtesy of: https://stackoverflow.com/questions/8704161/c-sharp-array-within-a-struct/8704505#8704505
					//https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/unsafe-code?redirectedfrom=MSDN#fixed-size-buffers

					if (fieldInfo.FieldType.GetInterface(typeof(IEnumerable<>).FullName) != null
						&& fieldInfo.FieldType.GetElementType() != typeof(Entity))
					{
						var array = fieldInfo.GetValue(structObject);

						if (array == null)
							array = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), 0);

						var newArray = DrawArray(fieldName, (Array)array, out localDirty);

						if (localDirty)
						{
							fieldInfo.SetValue(structObject, newArray);
							//localDirty = true;
						}

						break;
					}

					/*
					bool isEmptyScriptable = valueType.Name.Contains("Config"); //don't know how to show scriptable field with Null value to set it

					if (value as ScriptableObject || isEmptyScriptable)
					{
						var configValue = (ScriptableObject)value;

						var objectResult = EditorGUILayout.ObjectField(fieldName, configValue, typeof(ScriptableObject), false);

						if (!Equals(objectResult, configValue))
						{
							fieldInfo.SetValue(structObject, objectResult);

							localDirty = true;
						}

						break;
					}
					*/

					bool isStruct = valueType.IsValueType && !valueType.IsPrimitive;

					if (isStruct)
					{
						EditorGUILayout.BeginVertical();

						EditorGUILayout.LabelField(fieldName);

						if (DrawStruct(
								ref value,
								-1,
								level + 1))
							localDirty = true;

						EditorGUILayout.EndVertical();

						if (localDirty)
							fieldInfo.SetValue(
								structObject, value);
					}

					break;
			}

			return localDirty;
		}

		private Array DrawArray(string fieldName, Array arrayValue, out bool dirty)
		{
			dirty = false;

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(fieldName);

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField($"Length: {arrayValue.Length}");

			for (int i = 0; i < arrayValue.Length; i++)
			{
				var element = arrayValue.GetValue(i);

				bool localDirty = false;

				EditorGUILayout.BeginHorizontal();

				if (DrawValue(
					ref element))
					localDirty = true;

				if (localDirty)
				{
					arrayValue.SetValue(element, i);

					if (DEBUG_OPERATION)
						UnityEngine.Debug.Log($"[EntitySettingsEditor] array field {fieldName} dirty");
				}
				
				if (GUILayout.Button("Remove", EditorStyles.miniButtonRight))
				{
					arrayValue = RemoveElement(arrayValue, i, out dirty);
					break;
				}

				EditorGUILayout.EndHorizontal();

				dirty = localDirty;
			}

			if (GUILayout.Button("Add Element"))
			{
				arrayValue = AddNewElement(arrayValue, out dirty);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			return arrayValue;
		}

		private bool DrawValue(
			ref object valueObject)
		{
			bool localDirty = false;

			switch (valueObject)
			{
				case Guid guidValue:
					EditorGUI.BeginDisabledGroup(true);

					EditorGUILayout.TextField(
						guidValue.ToString());

					EditorGUI.EndDisabledGroup();

					break;

				case Entity entityValue:
					EditorGUI.BeginDisabledGroup(true);

					EditorGUILayout.TextField(
						entityValue.ToString());

					EditorGUI.EndDisabledGroup();

					break;

				case Enum enumValue:
					var enumResult = EditorGUILayout.EnumPopup(enumValue);

					if (!Equals(enumResult, enumValue))
					{
						valueObject = enumResult;

						localDirty = true;
					}

					break;
				
				case byte byteValue:

					int byteResultAsInt = EditorGUILayout.IntField(
						(byte)byteValue);

					if (byteResultAsInt != byteValue
					    && byteResultAsInt >= 0
					    && byteResultAsInt <= byte.MaxValue)
					{
						valueObject = (byte)byteResultAsInt;

						localDirty = true;
					}

					break;
				
				case ushort ushortValue:

					int ushortResultAsInt = EditorGUILayout.IntField(
						(ushort)ushortValue);

					if (ushortResultAsInt != ushortValue
					    && ushortResultAsInt >= 0
					    && ushortResultAsInt <= ushort.MaxValue)
					{
						valueObject = (ushort)ushortResultAsInt;

						localDirty = true;
					}

					break;

				case int intValue:

					int intResult = EditorGUILayout.IntField(
						intValue);

					if (intResult != intValue)
					{
						valueObject = intResult;

						localDirty = true;
					}

					break;

				case long longValue:

					long longResult = EditorGUILayout.LongField(
						longValue);

					if (longResult != longValue)
					{
						valueObject = longResult;

						localDirty = true;
					}

					break;

				case float floatValue:

					float floatResult = EditorGUILayout.FloatField(
						floatValue);

					if (floatResult != floatValue)
					{
						valueObject = floatResult;

						localDirty = true;
					}

					break;

				case double doubleValue:

					double doubleResult = EditorGUILayout.DoubleField(
						doubleValue);

					if (doubleResult != doubleValue)
					{
						valueObject = doubleResult;

						localDirty = true;
					}

					break;

				case bool boolValue:

					bool boolResult = EditorGUILayout.Toggle(
						boolValue);

					if (boolResult != boolValue)
					{
						valueObject = boolResult;

						localDirty = true;
					}

					break;

				case Vector2 vector2Value:

					Vector2 vector2Result = EditorGUILayout.Vector2Field(
						"", //it does not accept 1 argument
						vector2Value);

					if (vector2Result != vector2Value)
					{
						valueObject = vector2Result;

						localDirty = true;
					}

					break;

				case Vector3 vector3Value:

					Vector3 vector3Result = EditorGUILayout.Vector3Field(
						"", //it does not accept 1 argument
						vector3Value);

					if (vector3Result != vector3Value)
					{
						valueObject = vector3Result;

						localDirty = true;
					}

					break;

				default:

					if (valueObject is string stringValue)
					{
						string stringResult = EditorGUILayout.TextField(
							stringValue);

						if (stringResult != stringValue)
						{
							valueObject = stringResult;

							localDirty = true;
						}

						break;
					}

					break;
			}

			return localDirty;
		}

		private Array AddNewElement(Array arrayValue, out bool dirty)
		{
			dirty = false;

			var newArray = Array.CreateInstance(arrayValue.GetType().GetElementType(), arrayValue.Length + 1);

			for (int i = 0; i < arrayValue.Length; i++)
			{
				newArray.SetValue(arrayValue.GetValue(i), i);
			}

			object newElement = null;

			var elementType = arrayValue.GetType().GetElementType();

			if (elementType.IsValueType)
			{
				newElement = Activator.CreateInstance(elementType);
			}
			else
			{
				if (elementType == typeof(string))
					newElement = string.Empty;
			}

			newArray.SetValue(newElement, arrayValue.Length);

			dirty = true;

			return newArray;
		}

		private Array RemoveElement(Array arrayValue, int index, out bool dirty)
		{
			dirty = false;
			var newArray = Array.CreateInstance(arrayValue.GetType().GetElementType(), arrayValue.Length - 1);

			for (int i = 0, j = 0; i < arrayValue.Length; i++)
			{
				if (i != index)
				{
					newArray.SetValue(arrayValue.GetValue(i), j);
					j++;

					dirty = true;
				}
			}

			return newArray;
		}

		private void Erase()
		{
			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log("[EntitySettingsEditor] Erasing");

			entityPrototypeDTO = new EntityPrototypeDTO();

			entityPrototypeDTO.Components = new object[0];

			Serialize();
		}

		private void Serialize()
		{
			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log("[EntitySettingsEditor] Serializing");

			jsonSerializer.Serialize<EntityPrototypeDTO>(
				stringArgument,
				entityPrototypeDTO);

			((EntitySettings)target).EntityJson = stringArgument.Value;

			if (DEBUG_OPERATION)
			{
				UnityEngine.Debug.Log($"[EntitySettingsEditor] New json: {((EntitySettings)target).EntityJson}");

				UnityEngine.Debug.Log("[EntitySettingsEditor] Json updated");
			}

			EditorUtility.SetDirty(target);
		}

		private void Deserialize()
		{
			if (DEBUG_OPERATION)
			{
				UnityEngine.Debug.Log("[EntitySettingsEditor] Deserializing");

				UnityEngine.Debug.Log($"[EntitySettingsEditor] Current json: {((EntitySettings)target).EntityJson}");
			}

			if (string.IsNullOrEmpty(((EntitySettings)target).EntityJson))
			{
				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Json is empty");

				return;
			}

			stringArgument.Value = ((EntitySettings)target).EntityJson;

			bool success = jsonSerializer.Deserialize(
				stringArgument,
				typeof(EntityPrototypeDTO),
				out object newEntityDTO);

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Success: {success}");

			if (success)
			{
				entityPrototypeDTO = (EntityPrototypeDTO)newEntityDTO;

				if (DEBUG_OPERATION)
					UnityEngine.Debug.Log("[EntitySettingsEditor] Updated entityPrototypeDTO");
			}

			//FOR SOME REASON THIS ONE FAILS TO POPULATE
			/*
            bool success = jsonSerializer.Deserialize<EntityDTO>(
                stringArgument,
                out EntityDTO newEntityDTO);
            
            if (success)
                entityDTO = newEntityDTO;
            */
		}

		private void OnComponentTypeToAddSelected(Type componentType)
		{
			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component type added: {componentType.Name}");

			object[] newComponentsArray = new object[entityPrototypeDTO.Components.Length + 1];

			Array.Copy(
				entityPrototypeDTO.Components,
				0,
				newComponentsArray,
				0,
				entityPrototypeDTO.Components.Length);

			newComponentsArray[newComponentsArray.Length - 1] = Activator.CreateInstance(
				componentType);

			entityPrototypeDTO.Components = newComponentsArray;

			Serialize();
		}

		private void RemoveComponent(int index)
		{
			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component №{index} removed");

			object[] newComponentsArray = new object[entityPrototypeDTO.Components.Length - 1];

			Array.Copy(
				entityPrototypeDTO.Components,
				0,
				newComponentsArray,
				0,
				index);

			Array.Copy(
				entityPrototypeDTO.Components,
				index + 1,
				newComponentsArray,
				index,
				entityPrototypeDTO.Components.Length - index - 1);

			entityPrototypeDTO.Components = newComponentsArray;

			Serialize();
		}

		private void MoveComponentUp(int index)
		{
			if (index == 0)
				return;

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component № {index} moved up");

			object temp = entityPrototypeDTO.Components[index];

			entityPrototypeDTO.Components[index] = entityPrototypeDTO.Components[index - 1];

			entityPrototypeDTO.Components[index - 1] = temp;

			Serialize();
		}
		
		private void MoveComponentUpX5(int index)
		{
			if (index == 0)
				return;

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component № {index} moved up 5 times");

			int destinationIndex = Math.Clamp(
				index - 5,
				0,
				index);
			
			object temp = entityPrototypeDTO.Components[index];

			for (int i = index; i > destinationIndex; i--)
				entityPrototypeDTO.Components[i] = entityPrototypeDTO.Components[i - 1];

			entityPrototypeDTO.Components[destinationIndex] = temp;

			Serialize();
		}
		
		private void MoveComponentToTheTop(int index)
		{
			if (index == 0)
				return;

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component № {index} moved to the top");
			
			object temp = entityPrototypeDTO.Components[index];

			for (int i = index; i > 0; i--)
				entityPrototypeDTO.Components[i] = entityPrototypeDTO.Components[i - 1];

			entityPrototypeDTO.Components[0] = temp;

			Serialize();
		}

		private void MoveComponentDown(int index)
		{
			if (index == entityPrototypeDTO.Components.Length - 1)
				return;

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component № {index} moved down");

			object temp = entityPrototypeDTO.Components[index];

			entityPrototypeDTO.Components[index] = entityPrototypeDTO.Components[index + 1];

			entityPrototypeDTO.Components[index + 1] = temp;

			Serialize();
		}
		
		private void MoveComponentDownX5(int index)
		{
			if (index == entityPrototypeDTO.Components.Length - 1)
				return;

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component № {index} moved down 5 times");

			int destinationIndex = Math.Clamp(
				index + 5,
				index,
				entityPrototypeDTO.Components.Length - 1);
			
			object temp = entityPrototypeDTO.Components[index];

			for (int i = index; i < destinationIndex; i++)
				entityPrototypeDTO.Components[i] = entityPrototypeDTO.Components[i + 1];

			entityPrototypeDTO.Components[destinationIndex] = temp;

			Serialize();
		}
		
		private void MoveComponentToTheBottom(int index)
		{
			if (index == entityPrototypeDTO.Components.Length - 1)
				return;

			if (DEBUG_OPERATION)
				UnityEngine.Debug.Log($"[EntitySettingsEditor] Component № {index} moved to the bottom");

			object temp = entityPrototypeDTO.Components[index];

			for (int i = index; i < entityPrototypeDTO.Components.Length - 1; i++)
				entityPrototypeDTO.Components[i] = entityPrototypeDTO.Components[i + 1];

			entityPrototypeDTO.Components[entityPrototypeDTO.Components.Length - 1] = temp;

			Serialize();
		}
	}
}
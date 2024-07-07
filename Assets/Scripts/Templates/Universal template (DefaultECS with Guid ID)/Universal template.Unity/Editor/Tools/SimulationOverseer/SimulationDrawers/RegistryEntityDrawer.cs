using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using HereticalSolutions.Entities;
using HereticalSolutions.Entities.Editor;

using DefaultEcs;

using UnityEngine;
using UnityEditor;

namespace HereticalSolutions.Templates.Universal.Unity.Editor
{
    public class RegistryEntityDrawer : SimulationDrawer
    {
        private IEntityIDEditorHelper[] entityIDEditorHelpers;
        
        private IEntityIDEditorHelper effectiveEntityIDEditorHelper;
        
        private object entityManager;

        private IContainsEntityWorlds<World, IDefaultECSEntityWorldController> entityWorldsRepository;
     
        private Dictionary<Guid, bool> toggledEntities;
     
        private string searchPattern = string.Empty;
        
        protected override string Title => "Registry entities";

        public RegistryEntityDrawer()
        {
            toggledEntities = new Dictionary<Guid, bool>();
        }

        public override void Initialize()
        {
            LazyInitialization();
        }

        private void LazyInitialization()
        {
            if (entityIDEditorHelpers == null)
            {
                entityIDEditorHelpers = GetHelpers();
            }
            
            if (entityManager == null)
            {
                foreach (var helper in entityIDEditorHelpers)
                {
                    if (helper.TryGetEntityManager(out entityManager))
                    {
                        effectiveEntityIDEditorHelper = helper;
                        
                        entityWorldsRepository = entityManager as IContainsEntityWorlds<World, IDefaultECSEntityWorldController>;

                        break;
                    }
                }
            }
        }

        private static IEntityIDEditorHelper[] GetHelpers()
        {
            var interfaceType = typeof(IEntityIDEditorHelper);

            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(
                    s => s.GetTypes())
                .Where(
                    p => interfaceType.IsAssignableFrom(p)
                         && p.IsClass
                         && !p.IsAbstract);

            IEntityIDEditorHelper[] result = new IEntityIDEditorHelper[types.Count()];

            for (int i = 0; i < types.Count(); i++)
            {
                result[i] = (IEntityIDEditorHelper)Activator.CreateInstance(types.ElementAt(i));
            }

            return result;
        }

        protected override void Draw()
        {
            LazyInitialization();

            searchPattern = EditorGUILayout.TextField(
                "Search pattern",
                searchPattern);

            GUILayout.Space(10);

            var allRegistryEntityIDs = effectiveEntityIDEditorHelper
                .GetAllRegistryEntityIDs(entityManager);

            if (allRegistryEntityIDs == null || allRegistryEntityIDs.Count() == 0)
            {
                return;
            }

            var keysClosure = toggledEntities.Keys.ToArray();

            foreach (var key in keysClosure)
            {
                if (!allRegistryEntityIDs.Contains<Guid>(key))
                {
                    toggledEntities.Remove(key);
                }
            }

            foreach (var registryEntityID in allRegistryEntityIDs)
            {
                if (!toggledEntities.ContainsKey(registryEntityID))
                {
                    toggledEntities.Add(registryEntityID, false);
                }
            }

            //allRegistryEntityIDs = allRegistryEntityIDs.OrderBy(x => x).ToArray();

            foreach (var registryEntityID in allRegistryEntityIDs)
            {
                if (!string.IsNullOrEmpty(searchPattern))
                {
                    if (!registryEntityID.ToString().StartsWith(searchPattern))
                        continue;
                }

                EditorGUIUtility.labelWidth = 350;

                toggledEntities[registryEntityID] = EditorGUILayout.Toggle(
                    new GUIContent(registryEntityID.ToString()),
                    toggledEntities[registryEntityID],
                    GUILayout.ExpandWidth(true));

                if (!toggledEntities[registryEntityID])
                    continue;

                var registryEntity = effectiveEntityIDEditorHelper.GetRegistryEntity(
                    entityManager,
                    registryEntityID);

                DrawEntity(
                    registryEntity,
                    "Registry world");

                EditorGUILayout.Space();

                foreach (var worldID in entityWorldsRepository.EntityWorldsRepository.AllWorldIDs)
                {
                    var localEntity = default(Entity);

                    localEntity = effectiveEntityIDEditorHelper.GetEntity(
                        entityManager,
                        registryEntityID,
                        worldID);

                    if (localEntity == default)
                    {
                        continue;
                    }

                    DrawEntity(
                        localEntity,
                        worldID);

                    EditorGUILayout.Space();
                }
            }
        }

        private void DrawEntity(
            Entity entity,
            string worldID)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField(worldID);

            EditorGUILayout.Space();

            var entitySerializationWrapper = new EntitySerializationWrapper(entity);

            foreach (var component in entitySerializationWrapper.Components)
            {
                //EditorGUILayout.BeginVertical("Window");
				GUILayout.BeginVertical(component.Type.Name, "Window");

				//EditorGUILayout.LabelField($"{component.Type.Name}");

				EditorGUILayout.Space();

				ToDetailedString(component.ObjectValue);

				//EditorGUILayout.EndVertical();
				GUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        private static void ToDetailedString(object component)
        {
            if (component == null)
                return;

            var fields = component.GetType().GetFields();

            foreach (var fieldInfo in fields)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(fieldInfo.Name);

                var value = fieldInfo.GetValue(component);

                string valueString = ValueToString(
                    fieldInfo,
                    value);

                //EditorGUILayout.LabelField(valueString);
                GUILayout.TextArea(valueString);

                EditorGUILayout.EndHorizontal();
            }

            if (fields.Length == 0)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("");

                EditorGUILayout.EndHorizontal();
            }
        }

        private static string ValueToString(
            FieldInfo fieldInfo,
            object value,
            bool recursive = false)
        {
            if (value == null)
                return "NULL";

            switch (value)
            {
                case Guid guidValue:
                    return guidValue.ToString();

                case Entity entityValue:
                    return entityValue.ToString();

                case Enum enumValue:
                    return enumValue.ToString();

                case byte byteValue:
                    return byteValue.ToString();

                case ushort ushortValue:
                    return ushortValue.ToString();

                case int intValue:
                    return intValue.ToString();

                case long longValue:
                    return longValue.ToString();

                case float floatValue:
                    return floatValue.ToString();

                case double doubleValue:
                    return doubleValue.ToString();

                case bool boolValue:
                    return boolValue.ToString();

                case Vector2 vector2Value:
                    return value.ToString();
                    //return $"{vector2Value.x:f3} {vector2Value.y:f3}";

                case Vector3 vector3Value:
                    return value.ToString();
                    //return $"{vector3Value.x:f3} {vector3Value.y:f3} {vector3Value.z:f3}";

                case Matrix4x4 matrix4X4Value:
                    return $"{matrix4X4Value.m00:f4} {matrix4X4Value.m10:f4} {matrix4X4Value.m20:f4} {matrix4X4Value.m30:f4}\n" +
                           $"{matrix4X4Value.m01:f4} {matrix4X4Value.m11:f4} {matrix4X4Value.m21:f4} {matrix4X4Value.m31:f4}\n" +
                           $"{matrix4X4Value.m02:f4} {matrix4X4Value.m12:f4} {matrix4X4Value.m22:f4} {matrix4X4Value.m32:f4}\n" +
                           $"{matrix4X4Value.m03:f4} {matrix4X4Value.m13:f4} {matrix4X4Value.m23:f4} {matrix4X4Value.m33:f4}";

                default:

                    if (fieldInfo.FieldType == typeof(string))
                    {
                        return value.ToString();
                    }

                    if (recursive)
                    {
                        return value.ToString();
                    }

                    //Enumerables may have the same problems as arrays
                    //TODO: implement fixed-size buffers instead of arrays
                    //Courtesy of: https://stackoverflow.com/questions/8704161/c-sharp-array-within-a-struct/8704505#8704505
                    //https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/unsafe-code?redirectedfrom=MSDN#fixed-size-buffers

                    if (fieldInfo.FieldType.GetInterface(typeof(IEnumerable<>).FullName) != null
                        && fieldInfo.FieldType.GetElementType() != typeof(Entity))
                    {
                        StringBuilder stringBuilder = new StringBuilder();

                        Array arrayValue = value as Array;

                        if (arrayValue == null)
                        {
                            return value.ToString();
                        }

                        for (int i = 0; i < arrayValue.Length; i++)
                        {
                            stringBuilder.Append(
                                ValueToString(
                                    fieldInfo,
                                    arrayValue.GetValue(i),
                                    true));

                            if (i < arrayValue.Length - 1)
                            {
                                stringBuilder.Append("\n");
                            }
                        }

                        return stringBuilder.ToString();
                    }

                    bool isStruct = fieldInfo.FieldType.IsValueType && !fieldInfo.FieldType.IsPrimitive;

                    if (isStruct)
                    {
                        var innerFields = value.GetType().GetFields();

                        StringBuilder stringBuilder = new StringBuilder();

                        foreach (var innerFieldInfo in innerFields)
                        {
                            var innerValue = innerFieldInfo.GetValue(value);

                            string innerValueString = ValueToString(
                                innerFieldInfo,
                                innerValue,
                                true);

                            stringBuilder.Append($"{innerFieldInfo.Name}: {innerValueString}");

                            stringBuilder.Append("\n");
                        }

                        return stringBuilder.ToString();
                    }

                    break;
            }

            return value.ToString();
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;

public static class EditorUtils
{
    private static GUIStyle labelStyleBoldRegular;

    public static GUIStyle LabelStyleBoldRegular
    {
        get
        {
            if (labelStyleBoldRegular == null)
            {
                labelStyleBoldRegular = new GUIStyle(GUI.skin.label);

                labelStyleBoldRegular.alignment = TextAnchor.MiddleCenter;

                labelStyleBoldRegular.fontStyle = FontStyle.Bold;
            }

            return labelStyleBoldRegular;
        }
    }

    private static GUIStyle labelStyleBoldHeading;

    public static GUIStyle LabelStyleBoldHeading
    {
        get
        {
            if (labelStyleBoldHeading == null)
            {
                labelStyleBoldHeading = new GUIStyle(GUI.skin.label);

                labelStyleBoldHeading.fontSize = 14;

                labelStyleBoldHeading.alignment = TextAnchor.MiddleCenter;

                labelStyleBoldHeading.fontStyle = FontStyle.Bold;
            }

            return labelStyleBoldHeading;
        }
    }

    public static void ColumnsSameWidth(Rect rect, Action<float>[] drawers)
    {
        var width = rect.width;

        EditorGUILayout.BeginHorizontal();

        var widthPerDrawer = rect.width / drawers.Length;

        for (int i = 0; i < drawers.Length; i++)
        {
            GUILayout.BeginVertical(GUILayout.Width(widthPerDrawer));

            if (drawers[i] != null)
                drawers[i](widthPerDrawer);

            GUILayout.EndVertical();
        }

        EditorGUILayout.EndHorizontal();
    }

    public static void ObjectAndPreviewColumn(string labelText, ref GameObject source, ref Editor editor, float width)
    {
        GUILayout.Label(labelText, LabelStyleBoldRegular);

        source = (GameObject)EditorGUILayout.ObjectField(source, typeof(GameObject), true);

        if (source != null)
        {
            if (editor == null)
                editor = Editor.CreateEditor(source);

            width = Mathf.Max(width, 100f);

            editor.OnPreviewGUI(GUILayoutUtility.GetRect(width, width), EditorStyles.toolbar);
        }
    }

    public static int PopupSelectionColumn(string labelText, string[] options, int currentSelection, float width)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(width));

        GUILayout.Label(labelText, LabelStyleBoldRegular);

        int result = EditorGUILayout.Popup(currentSelection, options);

        EditorGUILayout.EndHorizontal();

        return result;
    }

    public static void ObjectAndLabelRow<T>(string labelText, ref T source) where T : UnityEngine.Object
    {
        EditorGUILayout.BeginHorizontal();

        var textSize = LabelStyleBoldRegular.CalcSize(new GUIContent(labelText));

        GUILayout.Label(labelText, LabelStyleBoldRegular, GUILayout.Width(textSize.x));

        source = (T)EditorGUILayout.ObjectField(source, typeof(T), true);

        EditorGUILayout.EndHorizontal();
    }

    public static float FloatAndLabelRow(string labelText, float initialValue)
    {
        EditorGUILayout.BeginHorizontal();

        var textSize = LabelStyleBoldRegular.CalcSize(new GUIContent(labelText));

        GUILayout.Label(labelText, LabelStyleBoldRegular, GUILayout.Width(textSize.x));

        var result = EditorGUILayout.FloatField(initialValue);

        EditorGUILayout.EndHorizontal();

        return result;
    }

    public static bool BoolAndLabelRow(string labelText, bool initialValue)
    {
        EditorGUILayout.BeginHorizontal();

        var textSize = LabelStyleBoldRegular.CalcSize(new GUIContent(labelText));

        GUILayout.Label(labelText, LabelStyleBoldRegular, GUILayout.Width(textSize.x));

        var result = EditorGUILayout.Toggle(initialValue);

        EditorGUILayout.EndHorizontal();

        return result;
    }

    public static T EnumAndLabelRow<T>(string labelText, T initialValue) where T : Enum
    {
        EditorGUILayout.BeginHorizontal();

        var textSize = LabelStyleBoldRegular.CalcSize(new GUIContent(labelText));

        GUILayout.Label(labelText, LabelStyleBoldRegular, GUILayout.Width(textSize.x));

        var result = (T)EditorGUILayout.EnumPopup(initialValue);

        EditorGUILayout.EndHorizontal();

        return result;
    }

    public static bool PathIsValidForProject(string path)
    {
        return path.Substring(0, Application.dataPath.Length) == Application.dataPath;
    }
}
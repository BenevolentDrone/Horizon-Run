using System;
using System.Linq;

using UnityEngine;

using UnityEditor;

namespace HereticalSolutions.HorizonRun
{
    public static class EditorUtilities
    {
        private static Vector2 toggleDefaultSize = GUI.skin.toggle.CalcSize(GUIContent.none);

        private static GUIStyle labelCenterAlignedStyle = GenerateLabelCenterAlignedStyle();

        private static GUIStyle GenerateLabelCenterAlignedStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);

            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }

        private static GUIStyle labelCenterAlignedBoldStyle = GenerateLabelCenterAlignedBoldStyle();

        private static GUIStyle GenerateLabelCenterAlignedBoldStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);

            style.alignment = TextAnchor.MiddleCenter;

            style.fontSize = 16;

            style.fontStyle = FontStyle.Bold;

            return style;
        }

        private static GUIStyle warningBoxRawStyle = new GUIStyle(GUI.skin.box);

        private static GUIStyle warningBoxStyle = GenerateWarningBoxStyle();

        private static GUIStyle GenerateWarningBoxStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);

            style.alignment = TextAnchor.MiddleCenter;

            style.fontSize = 20;

            style.fontStyle = FontStyle.Bold;

            style.stretchWidth = true;

            return style;
        }

        private static GUIStyle labelBoxRawStyle = new GUIStyle("flow node 0");

        private static GUIStyle labelBoxStyle = GenerateLabelBoxStyle();

        private static GUIStyle GenerateLabelBoxStyle()
        {
            GUIStyle style = new GUIStyle("TE NodeBox");

            style.alignment = TextAnchor.MiddleCenter;

            style.fontSize = 16;

            style.fontStyle = FontStyle.Bold;

            return style;
        }

        public static void LabelInBox(string text)
        {
            EditorGUILayout.LabelField(text, GUI.skin.box);
        }

        public static void LabelInBoxStretchHorizontally(string text)
        {
            EditorGUILayout.LabelField(text, GUI.skin.box, GUILayout.ExpandWidth(true));
        }

        public static void WarningBox(string text)
        {
            EditorGUILayout.LabelField(text, warningBoxStyle, GUILayout.Height(50));
        }

        public static void LabelBox(string text)
        {
            EditorGUILayout.LabelField(text, labelBoxStyle, GUILayout.Height(34));
        }

        public static bool LabelBoxWithToggle(string text, bool value)
        {
            GUIStyle style = labelBoxStyle;

            Rect rect = EditorGUILayout.BeginHorizontal(style, GUILayout.Height(40));

            float leftPadding = style.padding.left;

            bool result = GUI.Toggle(new Rect(
                new Vector2(rect.x + leftPadding, rect.center.y - toggleDefaultSize.y / 2f),
                toggleDefaultSize),
                value, "");

            EditorGUILayout.LabelField(text, labelCenterAlignedBoldStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.EndHorizontal();

            return result;
        }

        public static void AddPadding(Action innerRenderDelegate)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(GUIStyle.none.padding.left);

            EditorGUILayout.BeginVertical();

            GUILayout.Space(GUIStyle.none.padding.top);

            innerRenderDelegate();

            GUILayout.Space(GUIStyle.none.padding.bottom);

            EditorGUILayout.EndVertical();

            GUILayout.Space(GUIStyle.none.padding.right);

            EditorGUILayout.EndHorizontal();
        }

        public static void AddTablePadding(Action innerRenderDelegate)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            innerRenderDelegate();

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        public static void FoldoutBox(bool defaultValue, string text, Action toggleOff, Action toggleOn, Action innerRenderDelegate)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            bool toggled = EditorGUILayout.Toggle(defaultValue,
                GUILayout.Width(toggleDefaultSize.x));

            /*
            var debug = GUILayoutUtility.GetLastRect();

            EditorGUI.DrawRect(debug, Color.green);
            */

            Vector2 textSize = labelCenterAlignedStyle.CalcSize(new GUIContent(text));

            EditorGUILayout.LabelField(text, GUILayout.Width(textSize.x));

            /*
            debug = GUILayoutUtility.GetLastRect();

            EditorGUI.DrawRect(debug, Color.red);
            */

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if (!toggled && defaultValue)
                toggleOff();

            if (toggled && !defaultValue)
                toggleOn();

            if (toggled)
            {
                innerRenderDelegate();
            }

            EditorGUILayout.EndVertical();
        }

        public static void MultiValueProgressBar(string name, int[] values, Color[] colors)
        {
            int maxValue = values.Last();

            EditorGUILayout.BeginVertical(GUI.skin.box);

            var rect = EditorGUILayout.BeginHorizontal();

            Vector2 nameTextSize = labelCenterAlignedStyle.CalcSize(new GUIContent(name));

            Vector2 containerCapacityTextSize = labelCenterAlignedStyle.CalcSize(new GUIContent(maxValue.ToString()));

            EditorGUILayout.LabelField(name, GUILayout.Width(nameTextSize.x));

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(maxValue.ToString(), GUILayout.Width(containerCapacityTextSize.x));

            float start = rect.x + nameTextSize.x;

            float width = rect.width - containerCapacityTextSize.x - nameTextSize.x;

            Color cachedColor = GUI.color;

            for (int i = values.Length - 1; i > -1; i--)
            {
                GUI.color = colors[i];

                GUI.Box(new Rect(start, rect.y, ((float)values[i] / maxValue) * width, rect.height), GUIContent.none);
            }

            GUI.color = cachedColor;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        public static void DrawWireCapsule(Vector3 _pos, Quaternion _rot, float _radius, float _height, Color _color = default(Color))
        {
            if (_color != default(Color))
                Handles.color = _color;
            Matrix4x4 angleMatrix = Matrix4x4.TRS(_pos, _rot, Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(angleMatrix))
            {
                var pointOffset = (_height - (_radius * 2)) / 2;

                //draw sideways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.left, Vector3.back, -180, _radius);
                Handles.DrawLine(new Vector3(0, pointOffset, -_radius), new Vector3(0, -pointOffset, -_radius));
                Handles.DrawLine(new Vector3(0, pointOffset, _radius), new Vector3(0, -pointOffset, _radius));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.left, Vector3.back, 180, _radius);
                //draw frontways
                Handles.DrawWireArc(Vector3.up * pointOffset, Vector3.back, Vector3.left, 180, _radius);
                Handles.DrawLine(new Vector3(-_radius, pointOffset, 0), new Vector3(-_radius, -pointOffset, 0));
                Handles.DrawLine(new Vector3(_radius, pointOffset, 0), new Vector3(_radius, -pointOffset, 0));
                Handles.DrawWireArc(Vector3.down * pointOffset, Vector3.back, Vector3.left, -180, _radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * pointOffset, Vector3.up, _radius);
                Handles.DrawWireDisc(Vector3.down * pointOffset, Vector3.up, _radius);

            }
        }
    }
}
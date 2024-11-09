using System;
using System.Collections.Generic;

using HereticalSolutions.Repositories.Factories;

using UnityEditor;

using UnityEngine;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public static class ARToolboxEditorHelpers
	{
		#region Constants

		#region UI consts

		private const string UI_TEXT_ENABLED = "Enabled";

		private const string UI_TEXT_DISNABLED = "Disabled";

		private const string UI_TEXT_ADD_PATTERN = "Add {0}";

		private const string UI_TEXT_SUBCONTEXT_HEADER_PATTERN = "{0} {1}";

		private const string UI_TEXT_FROM_SELECTION = "From selection";

		private const string UI_TEXT_SELECT = "Select";

		private const string UI_TEXT_UP = "Up";

		private const string UI_TEXT_DOWN = "Down";

		private const string UI_TEXT_REMOVE = "Remove";

		#endregion

		#region Metadata keys

		private const string KEY_ENABLED_PATTERN = "{0}_Enabled";

		private const string KEY_MOVE_UP_PATTERN = "{0}_MoveUp";

		private const string KEY_MOVE_DOWN_PATTERN = "{0}_MoveDown";

		private const string KEY_MOVE_UP_5_PATTERN = "{0}_MoveUp5";

		private const string KEY_MOVE_DOWN_5_PATTERN = "{0}_MoveDown5";

		private const string KEY_MOVE_TOP_PATTERN = "{0}_MoveTop";

		private const string KEY_MOVE_BOTTOM_PATTERN = "{0}_MoveBottom";

		private const string KEY_PENDING_REMOVAL_PATTERN = "{0}_PendingRemoval";

		#endregion

		public const float TOGGLE_TIGHT_WIDTH = 14f;

		public const float HANDLES_LENGTH_FACTOR = 0.882f;

		#endregion

		#region Styles

		private static Color greyer = new Color(0.8f, 0.8f, 0.8f, 1.0f);

		private static GUIStyle labelStyleRegular;

		public static GUIStyle LabelStyleRegular
		{
			get
			{
				if (labelStyleRegular == null)
				{
					labelStyleRegular = new GUIStyle(GUI.skin.label);

					labelStyleRegular.alignment = TextAnchor.MiddleCenter;
				}

				return labelStyleRegular;
			}
		}

		private static GUIStyle labelStyleRegularBold;

		public static GUIStyle LabelStyleRegularBold
		{
			get
			{
				if (labelStyleRegularBold == null)
				{
					labelStyleRegularBold = new GUIStyle(GUI.skin.label);

					labelStyleRegularBold.alignment = TextAnchor.MiddleCenter;

					labelStyleRegularBold.fontStyle = FontStyle.Bold;
				}

				return labelStyleRegularBold;
			}
		}

		private static GUIStyle labelStyleTitleBold;

		public static GUIStyle LabelStyleTitleBold
		{
			get
			{
				if (labelStyleTitleBold == null)
				{
					labelStyleTitleBold = new GUIStyle(
						GUI.skin.label);

					labelStyleTitleBold.fontSize = 14;

					labelStyleTitleBold.alignment = TextAnchor.MiddleCenter;

					labelStyleTitleBold.fontStyle = FontStyle.Bold;
				}

				return labelStyleTitleBold;
			}
		}

		private static GUIStyle boxStyleInner;

		public static GUIStyle BoxStyleInner
		{
			get
			{
				if (boxStyleInner == null)
				{
					boxStyleInner = new GUIStyle("Box");

					if (boxStyleInner.normal.background != null)
					{
						boxStyleInner.normal.background = MakeTex(
							boxStyleInner.normal.background.width,
							boxStyleInner.normal.background.height,
							boxStyleInner.normal.background,
							greyer);
					}
					else
					{
						boxStyleInner.normal.background = MakeTex(
							2,
							2,
							null,
							greyer);
					}
				}

				return boxStyleInner;
			}
		}

		private static Texture2D MakeTex(
			int width,
			int height,
			Texture2D source,
			Color col)
		{
			Color[] pix = new Color[width * height];

			for (int i = 0; i < pix.Length; i++)
			{
				if (source != null)
				{
					pix[i] = source.GetPixelBilinear(
						(float)i / width,
						(float)i / height)
						* col;
				}
				else
				{
					pix[i] = col;
				}
			}

			Texture2D result = new Texture2D(width, height);

			result.SetPixels(pix);

			result.Apply();

			return result;
		}

		#endregion

		#region Labels

		public static void DrawLabelTight(
			string labelText)
		{
			EditorGUILayout.LabelField(
				labelText,
				GUILayout.Width(
					GUI
						.skin
						.toggle
						.CalcSize(
							new GUIContent(labelText))
						.x));
		}

		//Courtesy of https://discussions.unity.com/t/how-to-make-textarea-or-a-textfield-uneditable/7780
		public static void DrawLabelSelectable(
			string labelText)
		{
			EditorGUILayout.SelectableLabel(
				labelText,
				LabelStyleRegular,
				GUILayout.Height(EditorGUIUtility.singleLineHeight));
		}

		#endregion

		#region Buttons

		public static bool DrawButtonTight(
			string buttonText)
		{
			return GUILayout.Button(
				buttonText,
				GUILayout.Width(
					GUI
						.skin
						.button
						.CalcSize(
							new GUIContent(buttonText))
						.x));
		}

		#endregion

		#region Toggles

		public static bool DrawToggleTight(
			string labelText,
			bool previousValue)
		{
			DrawLabelTight(labelText);

			var newValue = EditorGUILayout.Toggle(
				previousValue,
				GUILayout.Width(TOGGLE_TIGHT_WIDTH));

			return newValue;
		}

		#endregion

		#region Inner box

		public static void BeginInnerBoxWithTitle(
			string title)
		{
			var rect = EditorGUILayout.BeginVertical(
				GUI.skin.box);

			GUILayout.Label(
				title,
				LabelStyleTitleBold);
		}

		public static void EndInnerBox()
		{
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
		}

		#endregion

		#region Enabled

		public static bool BeginEnabledCheck(
			IARToolboxContext context,
			string toolPrefix)
		{
			var enabledKey = String.Format(
				KEY_ENABLED_PATTERN,
				toolPrefix);

			if (!context.TryGet<bool>(
				enabledKey,
				out var previousEnabled))
			{
				//Initialize to default value
				previousEnabled = true;

				context.AddOrUpdate(
					enabledKey,
					previousEnabled);
			}

			bool currentEnabled = EditorGUILayout.Foldout(
				previousEnabled,
				(previousEnabled)
					? UI_TEXT_ENABLED
					: UI_TEXT_DISNABLED);

			if (currentEnabled != previousEnabled)
			{
				context.AddOrUpdate(
					enabledKey,
					currentEnabled);
			}

			return currentEnabled;
		}

		public static bool GetEnabled(
			IARToolboxContext context,
			string toolPrefix)
		{
			var enabledKey = String.Format(
				KEY_ENABLED_PATTERN,
				toolPrefix);

			if (!context.TryGet<bool>(
				enabledKey,
				out var enabled))
			{
				return false;
			}

			return enabled;
		}

		#endregion

		#region Subcontext

		public static void BeginSubcontextBoxWithTitle(
			string subcontextTitlePrefix,
			int subcontextIndex)
		{
			GUILayout.BeginVertical(
				BoxStyleInner);

			EditorGUILayout.LabelField(
				string.Format(
					UI_TEXT_SUBCONTEXT_HEADER_PATTERN,
					subcontextTitlePrefix,
					subcontextIndex + 1),
				LabelStyleTitleBold);

			EditorGUILayout.Separator();
		}

		public static void EndSubcontextBox()
		{
			GUILayout.EndVertical();
		}

		public static List<IARToolboxContext> GetSubcontextList(
			IARToolboxContext context,
			string subcontextKey,
			string subcontextElementText)
		{
			if (!context.TryGet<List<IARToolboxContext>>(
				subcontextKey,
				out var subcontexts))
			{
				//Initialize to default value
				subcontexts = new List<IARToolboxContext>();

				context.AddOrUpdate(
					subcontextKey,
					subcontexts);
			}

			if (GUILayout.Button(
				String.Format(
					UI_TEXT_ADD_PATTERN,
					subcontextElementText)))
			{
				subcontexts.Add(
					new ARToolboxChildContext(
						context,
						RepositoriesFactory.BuildDictionaryRepository<string, object>()));
			}

			return subcontexts;
		}

		public static void DrawSubcontextControls(
			IARToolboxContext context,
			int subcontextIndex,
			int totalSubcontextsCount,
			string subcontextKeyPrefix)
		{
			bool ctrlPressed = false;

			bool shiftPressed = false;

			if (Event.current.shift)
			{
				shiftPressed = true;
			}
			else if (Event.current.control)
			{
				ctrlPressed = true;
			}

			EditorGUILayout.BeginHorizontal();

			var currentEnabled = GUI.enabled;

			if (subcontextIndex == 0)
			{
				GUI.enabled = false;
			}

			if (GUILayout.Button(UI_TEXT_UP))
			{
				if (ctrlPressed)
				{
					string moveUp5Key = String.Format(
						KEY_MOVE_UP_5_PATTERN,
						subcontextKeyPrefix);

					context.AddOrUpdate(
						moveUp5Key,
						true);
				}
				else if (shiftPressed)
				{
					string moveTopKey = String.Format(
						KEY_MOVE_TOP_PATTERN,
						subcontextKeyPrefix);

					context.AddOrUpdate(
						moveTopKey,
						true);
				}
				else
				{
					string moveUpKey = String.Format(
						KEY_MOVE_UP_PATTERN,
						subcontextKeyPrefix);

					context.AddOrUpdate(
						moveUpKey,
						true);
				}
			}

			if (subcontextIndex == 0)
			{
				GUI.enabled = currentEnabled;
			}

			if (subcontextIndex == totalSubcontextsCount - 1)
			{
				GUI.enabled = false;
			}

			if (GUILayout.Button(UI_TEXT_DOWN))
			{
				if (ctrlPressed)
				{
					string moveDown5Key = String.Format(
						KEY_MOVE_DOWN_5_PATTERN,
						subcontextKeyPrefix);

					context.AddOrUpdate(
						moveDown5Key,
						true);
				}
				else if (shiftPressed)
				{
					string moveBottomKey = String.Format(
						KEY_MOVE_BOTTOM_PATTERN,
						subcontextKeyPrefix);

					context.AddOrUpdate(
						moveBottomKey,
						true);
				}
				else
				{
					string moveDownKey = String.Format(
						KEY_MOVE_DOWN_PATTERN,
						subcontextKeyPrefix);

					context.AddOrUpdate(
						moveDownKey,
						true);
				}
			}

			if (subcontextIndex == totalSubcontextsCount - 1)
			{
				GUI.enabled = currentEnabled;
			}

			if (GUILayout.Button(UI_TEXT_REMOVE))
			{
				string pendingRemovalKey = String.Format(
					KEY_PENDING_REMOVAL_PATTERN,
					subcontextKeyPrefix);

				context.AddOrUpdate(
					pendingRemovalKey,
					true);
			}

			EditorGUILayout.EndHorizontal();
		}

		public static void UpdateSubcontextList(
			List<IARToolboxContext> subcontexts,
			string subcontextKeyPrefix)
		{
			for (int i = subcontexts.Count - 1; i >= 0; i--)
			{
				var currentTrack = subcontexts[i];

				if (currentTrack.TryGet<bool>(
					String.Format(
						KEY_PENDING_REMOVAL_PATTERN,
						subcontextKeyPrefix),
					out var _))
				{
					subcontexts.RemoveAt(i);

					continue;
				}

				string moveUpKey = String.Format(
					KEY_MOVE_UP_PATTERN,
					subcontextKeyPrefix);

				if (currentTrack.TryGet<bool>(
					moveUpKey,
					out var _))
				{
					if (i > 0)
					{
						var temp = subcontexts[i - 1];

						subcontexts[i - 1] = subcontexts[i];

						subcontexts[i] = temp;
					}

					currentTrack.TryRemove(moveUpKey);

					continue;
				}

				string moveDownKey = String.Format(
					KEY_MOVE_DOWN_PATTERN,
					subcontextKeyPrefix);

				if (currentTrack.TryGet<bool>(
					moveDownKey,
					out var _))
				{
					if (i < subcontexts.Count - 1)
					{
						var temp = subcontexts[i + 1];

						subcontexts[i + 1] = subcontexts[i];

						subcontexts[i] = temp;
					}

					currentTrack.TryRemove(moveDownKey);

					continue;
				}

				string moveUp5Key = String.Format(
					KEY_MOVE_UP_5_PATTERN,
					subcontextKeyPrefix);

				if (currentTrack.TryGet<bool>(
					moveUp5Key,
					out var _))
				{
					if (i > 0)
					{
						int topmostIndex = Mathf.Max(i - 5, 0);

						var temp = subcontexts[topmostIndex];

						subcontexts[topmostIndex] = subcontexts[i];

						subcontexts[i] = temp;
					}

					currentTrack.TryRemove(moveUp5Key);

					continue;
				}

				string moveDown5Key = String.Format(
					KEY_MOVE_DOWN_5_PATTERN,
					subcontextKeyPrefix);

				if (currentTrack.TryGet<bool>(
					moveDown5Key,
					out var _))
				{
					if (i < subcontexts.Count - 1)
					{
						int bottommostIndex = Mathf.Min(i + 5, subcontexts.Count - 1);

						var temp = subcontexts[bottommostIndex];

						subcontexts[bottommostIndex] = subcontexts[i];

						subcontexts[i] = temp;
					}

					currentTrack.TryRemove(moveDown5Key);

					continue;
				}

				string moveTopKey = String.Format(
					KEY_MOVE_TOP_PATTERN,
					subcontextKeyPrefix);

				if (currentTrack.TryGet<bool>(
					moveTopKey,
					out var _))
				{
					if (i > 0)
					{
						var temp = subcontexts[0];

						subcontexts[0] = subcontexts[i];

						subcontexts[i] = temp;
					}

					currentTrack.TryRemove(moveTopKey);

					continue;
				}

				string moveBottomKey = String.Format(
					KEY_MOVE_BOTTOM_PATTERN,
					subcontextKeyPrefix);

				if (currentTrack.TryGet<bool>(
					moveBottomKey,
					out var _))
				{
					if (i < subcontexts.Count - 1)
					{
						var temp = subcontexts[subcontexts.Count - 1];

						subcontexts[subcontexts.Count - 1] = subcontexts[i];

						subcontexts[i] = temp;
					}

					currentTrack.TryRemove(moveBottomKey);

					continue;
				}
			}
		}

		#endregion

		#region Selection

		public static void DrawGameObjectInputWithSelectionControls(
			string label,
			GameObject previousSelected,
			out GameObject newSelected)
		{
			EditorGUILayout.BeginHorizontal();

			newSelected = (GameObject)EditorGUILayout.ObjectField(
				label,
				previousSelected,
				typeof(GameObject),
				true);

			if (DrawButtonTight(UI_TEXT_FROM_SELECTION))
			{
				newSelected = Selection.activeGameObject;
			}

			if (DrawButtonTight(UI_TEXT_SELECT))
			{
				Selection.activeGameObject = newSelected;
			}

			EditorGUILayout.EndHorizontal();
		}

		public static void DrawInputWithSelectionFromAssetsControls<TValue>(
			string label,
			TValue previousValue,
			out TValue newValue)
			where TValue : UnityEngine.Object
		{
			EditorGUILayout.BeginHorizontal();

			newValue = (TValue)EditorGUILayout.ObjectField(
				label,
				previousValue,
				typeof(TValue),
				true);

			if (DrawButtonTight(UI_TEXT_FROM_SELECTION))
			{
				TValue selectedAsset = null;

				foreach (UnityEngine.Object obj
					in Selection.GetFiltered(
						typeof(TValue),
						SelectionMode.Assets))
				{
					selectedAsset = obj as TValue;

					break;
				}

				newValue = selectedAsset;
			}

			if (DrawButtonTight(UI_TEXT_SELECT))
			{
				Selection.activeObject = newValue;
			}

			EditorGUILayout.EndHorizontal();
		}

		#endregion

		#region Columns

		public static int PopupSelectionColumn(
			string labelText,
			string[] options,
			int currentSelectionIndex)
		{
			EditorGUILayout.BeginHorizontal();

			GUILayout.Label(labelText, LabelStyleRegularBold);

			int result = EditorGUILayout.Popup(currentSelectionIndex, options);

			EditorGUILayout.EndHorizontal();

			return result;
		}

		public static int PopupSelectionColumn(
			string labelText,
			string[] options,
			int currentSelectionIndex,
			float width)
		{
			EditorGUILayout.BeginHorizontal(GUILayout.Width(width));

			GUILayout.Label(labelText, LabelStyleRegularBold);

			int result = EditorGUILayout.Popup(currentSelectionIndex, options);

			EditorGUILayout.EndHorizontal();

			return result;
		}

		#endregion

		#region Handles

		public static void DrawAdjustablePositionHandle(
			Vector3 position,
			Quaternion rotation,
			float handleLength)
		{
			var handlesColor = Handles.color;

			Handles.color = Color.blue;

			Handles.ArrowHandleCap(
				0,
				position,
				Quaternion.LookRotation(
					rotation * Vector3.forward,
					Vector3.up),
				handleLength * HANDLES_LENGTH_FACTOR,
				EventType.Repaint);

			Handles.color = Color.red;

			Handles.ArrowHandleCap(
				0,
				position,
				Quaternion.LookRotation(
					rotation * Vector3.right,
					Vector3.up),
				handleLength * HANDLES_LENGTH_FACTOR,
				EventType.Repaint);

			Handles.color = Color.green;

			Handles.ArrowHandleCap(
				0,
				position,
				Quaternion.LookRotation(
					rotation * Vector3.up,
					Vector3.up),
				handleLength * HANDLES_LENGTH_FACTOR,
				EventType.Repaint);

			Handles.color = handlesColor;
		}

		#endregion

		#region Game objects

		public static void AssumeDefaultPose(GameObject gameObject)
		{
			foreach (var @override in PrefabUtility.GetObjectOverrides(gameObject, true))
			{
				var assetObject = @override.GetAssetObject();

				if (assetObject != null
					&& assetObject is Transform)
				{
					@override.Revert();
				}
			}
		}

		#endregion
	}
}
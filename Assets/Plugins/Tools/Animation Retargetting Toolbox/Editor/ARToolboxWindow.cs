using HereticalSolutions.Metadata.Factories;

using UnityEditor;

using UnityEngine;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class ARToolboxWindow : EditorWindow
	{
		private const string WINDOW_TITLE = "ARToolbox"; //"Animation Retargetting Toolbox";

		private const bool SINGLE_INSTANCE = false;

		private const string KEY_PREFS_SAVED_EDITOR_STATES = "ARToolbox_SavedEditors";

		private  const string KEY_PREFS_EDITOR_STATE_SAVE_PREFIX = "ARToolbox_Save_{0}";

		private static IARToolboxToolWindow[] toolWindows;

		private IARToolboxContext context;

		private double lastEditorUpdateTime;

		private Vector2 scrollPos;

		#region EditorWindow callbacks

		[MenuItem("Tools/Animation/Animation retargetting tool")]
		static void Init()
		{
			//UnityEngine.Debug.Log("[ARToolboxWindow] Init");

			//StaticLazyInitialization();

			ARToolboxWindow window = null;

			if (SINGLE_INSTANCE)
			{
				// Get existing open window or if none, make a new one:
				window = (ARToolboxWindow)EditorWindow.GetWindow(
					typeof(ARToolboxWindow),
					false,
					WINDOW_TITLE);
			}
			else
			{
				window = (ARToolboxWindow)EditorWindow.CreateWindow<ARToolboxWindow>(
					WINDOW_TITLE);
			}
		}

		void OnFocus()
		{
			//UnityEngine.Debug.Log($"[ARToolboxWindow] OnFocus {this.GetInstanceID()}");
		}

		//OnEnable and OnFocus are inconsistent on which one is called first
		//But OnFocus is called every time the window is selected
		//So it's OnEnable from now
		void OnEnable()
		{
			//UnityEngine.Debug.Log($"[ARToolboxWindow] OnEnable {this.GetInstanceID()}");

			Initialize();
		}

		void OnGUI()
		{
			if (toolWindows == null)
				return;

			scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

			foreach (var toolWindow in toolWindows)
			{
				toolWindow.Draw(context);
			}

			EditorGUILayout.EndScrollView();
		}

		// Window has been destroyed
		//void OnDestroy()
		void OnDisable()
		{
			//UnityEngine.Debug.Log($"[ARToolboxWindow] OnDisable {this.GetInstanceID()}");

			Cleanup();
		}

		void OnDestroy()
		{
			//UnityEngine.Debug.Log($"[ARToolboxWindow] OnDestroy {this.GetInstanceID()}");
		}

		#endregion

		static void StaticLazyInitialization()
		{
			if (toolWindows == null)
			{
				//UnityEngine.Debug.Log("[ARToolboxWindow] StaticLazyInitialization");

				toolWindows = new IARToolboxToolWindow[]
				{
					 new SkeletonTool(),
					 new MappingTool(),
					 new AnimationTool()
				};
			}
		}

		void LazyInitialization()
		{
			if (context == null)
			{
				//UnityEngine.Debug.Log("[ARToolboxWindow] LazyInitialization");

				context = new ARToolboxRootContext(
					MetadataFactory.BuildWeaklyTypedMetadata());
			}

			if (lastEditorUpdateTime == 0)
			{
				lastEditorUpdateTime = EditorApplication.timeSinceStartup;
			}
		}

		void OnBeforeAssemblyReload()
		{
			//Debug.Log($"[ARToolboxWindow] OnBeforeAssemblyReload {this.GetInstanceID()}");

			var saves = ARToolboxEditorHelpers.GetSavedEditorStatesFromEditorPrefs(
				KEY_PREFS_SAVED_EDITOR_STATES);

			ARToolboxEditorHelpers.SerializeStateToEditorPrefs<object>(
				KEY_PREFS_SAVED_EDITOR_STATES,
				KEY_PREFS_EDITOR_STATE_SAVE_PREFIX,
				this.GetInstanceID(),
				saves,
				null);

			//Debug.Log($"[ARToolboxWindow] SAVED");
		}

		void OnAfterAssemblyReload()
		{
			//Debug.Log($"[ARToolboxWindow] OnAfterAssemblyReload {this.GetInstanceID()}");

			var saves = ARToolboxEditorHelpers.GetSavedEditorStatesFromEditorPrefs(
				KEY_PREFS_SAVED_EDITOR_STATES);

			if (saves.Length != 0)
			{
				if (ARToolboxEditorHelpers.TryDeserializeStateFromEditorPrefs<object>(
					KEY_PREFS_SAVED_EDITOR_STATES,
					KEY_PREFS_EDITOR_STATE_SAVE_PREFIX,
					this.GetInstanceID(),
					saves,
					out _))
				{
					//Debug.Log($"[ARToolboxWindow] RESTORED");
				}
			}
		}

		void OnSceneGUI(
			SceneView sceneView)
		{
			if (toolWindows == null)
				return;

			foreach (var toolWindow in toolWindows)
			{
				toolWindow.DrawHandles(context);
			}
		}

		void OnSceneUpdate()
		{
			var currentTime = EditorApplication.timeSinceStartup;

			var deltaTime = currentTime - lastEditorUpdateTime;

			if (lastEditorUpdateTime == 0)
			{
				deltaTime = 0;
			}

			lastEditorUpdateTime = currentTime;

			//UnityEngine.Debug.Log(deltaTime);

			((ARToolboxRootContext)context).DeltaTime = deltaTime;

			if (toolWindows == null)
				return;

			foreach (var toolWindow in toolWindows)
			{
				toolWindow.SceneUpdate(context);
			}
		}

		void Initialize()
		{
			StaticLazyInitialization();

			LazyInitialization();

			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;

			SceneView.duringSceneGui -= this.OnSceneGUI;
			SceneView.duringSceneGui += this.OnSceneGUI;


			EditorApplication.update -= this.OnSceneUpdate;
			EditorApplication.update += this.OnSceneUpdate;
		}

		void Cleanup()
		{
			AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
			AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;

			SceneView.duringSceneGui -= this.OnSceneGUI;

			EditorApplication.update -= this.OnSceneUpdate;

			//if (!savedToEditorPrefs)
			//{
			//	UnityEngine.Debug.Log($"[ARToolboxWindow] NOT SAVED");
			//}
		}
	}
}
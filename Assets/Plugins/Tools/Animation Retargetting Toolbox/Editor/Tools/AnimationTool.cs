using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

using UnityEditor;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class AnimationTool
		: IARToolboxToolWindow
	{
		#region Constants

		#region UI consts

		private const string UI_TEXT_TITLE = "Animation tool";

		private const string UI_TEXT_TRACK_LOWERCASE = "track";

		private const string UI_TEXT_TRACK_CAMELCASE = "Track";

		private const string UI_TEXT_TARGET_ANIMATABLE = "Target animatable:";

		private const string UI_TEXT_NO_ANIMATOR = "Selected game object has no Animator component";

		private const string UI_TEXT_ADD_ANIMATOR = "Add Animator";

		private const string UI_TEXT_NO_CONTROLLER = "Animator does not have an AnimatorController serialized";

		private const string UI_TEXT_SELECTED_CLIP_FROM_ANIMATOR = "Select clip from Animator:";

		private const string UI_TEXT_TARGET_CLIP = "Target clip:";

		private const string UI_TEXT_FORCE_LOOP = "Force loop";

		private const string UI_TEXT_NORMAL = "Normal";

		private const string UI_TEXT_PLAY = "Play";

		private const string UI_TEXT_STOP = "Stop";

		private const string UI_TEXT_SAMPLE = "Sample";

		private const string UI_TEXT_SAMPLING = "Sampling";

		private const string UI_TEXT_DEFAULT_POSE = "Default pose";

		private const string UI_TEXT_SAMPLE_TIME = "Sample time:";

		#endregion


		private const string GRAPH_OUTPUT_PLAYABLE_NAME = "AnimationOutput";

		#region Metadata keys

		private const string KEY_TOOL_PREFIX = "AnimationTool";

		private const string KEY_TRACKS = "AnimationTool_Tracks";

		private const string KEY_TRACK_PREFIX = "Track";



		private const string KEY_SELECTED_ANIMATABLE = "Track_SelectedAnimatable";

		private const string KEY_SELECTED_ANIMATION = "Track_SelectedAnimation";

		private const string KEY_SELECTED_ANIMATABLE_CPLAYABLE = "Track_SelectedAnimatableClipPlayable";

		private const string KEY_SELECTED_ANIMATABLE_PGRAPH = "Track_SelectedAnimatablePlayableGraph";

		private const string KEY_PLAY_ANIMATION = "Track_PlayAnimation";

		private const string KEY_SAMPLE_ANIMATION = "Track_SampleAnimation";

		private const string KEY_SAMPLE_ANIMATION_NORMAL_TIME = "Track_SampleAnimationNormalTime";

		private const string KEY_SAMPLE_ANIMATION_NORMAL_TIME_SNORMAL = "Track_SampleAnimationNormalTimeShowInNormal";

		private const string KEY_ANIMATION_PROGRESS_SNORMAL = "Track_AnimationProgressShowInNormal";

		private const string KEY_FORCE_LOOP = "Track_ForceLoop";

		#endregion

		#endregion

		#region IARToolboxToolWindow

		public void Draw(IARToolboxContext context)
		{
			ARToolboxEditorHelpers.BeginInnerBoxWithTitle(
				UI_TEXT_TITLE);

			bool enabled = ARToolboxEditorHelpers.BeginEnabledCheck(
				context,
				KEY_TOOL_PREFIX);

			if (enabled)
			{
				var tracks = ARToolboxEditorHelpers.GetSubcontextList(
					context,
					KEY_TRACKS,
					UI_TEXT_TRACK_LOWERCASE);

				for (int i = 0; i < tracks.Count; i++)
				{
					DrawTrack(
						tracks[i],
						i,
						tracks.Count);
				}

				ARToolboxEditorHelpers.UpdateSubcontextList(
					tracks,
					KEY_TRACK_PREFIX);
			}

			ARToolboxEditorHelpers.EndInnerBox();
		}

		public void DrawHandles(IARToolboxContext context)
		{
		}

		public void SceneUpdate(IARToolboxContext context)
		{
			//if (!ARToolboxEditorHelpers.GetEnabled(
			//	context,
			//	KEY_TOOL_PREFIX))
			//{
			//	return;
			//}

			if (!context.TryGet<List<IARToolboxContext>>(
				KEY_TRACKS,
				out var tracks))
			{
				return;
			}

			foreach (var track in tracks)
			{
				PlayTrack(track);
			}
		}

		#endregion

		private void DrawTrack(
			IARToolboxContext context,
			int trackIndex,
			int totalTracksCount)
		{
			ARToolboxEditorHelpers.BeginSubcontextBoxWithTitle(
				UI_TEXT_TRACK_CAMELCASE,
				trackIndex);


			bool playAnimationConditionsMet = true;

			bool animationSourcesDirty = false;

			RuntimeAnimatorController currentSelectedRuntimeAnimatorController = null;


			DrawCurrentSelectedAnimatable(
				context,
				ref animationSourcesDirty,
				ref playAnimationConditionsMet,
				ref currentSelectedRuntimeAnimatorController,
				out var currentSelectedAnimatable);

			DrawCurrentSelectedAnimation(
				context,
				ref animationSourcesDirty,
				ref playAnimationConditionsMet,
				currentSelectedRuntimeAnimatorController);

			DrawForceLoop(context);

			EditorGUILayout.Separator();

			EditorGUILayout.BeginHorizontal();

			DrawPlayAnimation(
				context,
				out var playAnimationToggled,
				out var currentPlayAnimation,
				playAnimationConditionsMet);

			DrawSampleAnimation(
				context,
				out var sampleAnimationToggled,
				out var currentSampleAnimation,
				playAnimationConditionsMet);

			DrawAssumeDefaultPose(
				currentSelectedAnimatable,
				currentPlayAnimation,
				currentSampleAnimation);

			EditorGUILayout.EndHorizontal();

			DrawSampleAnimationNormalTime(
				context,
				playAnimationConditionsMet);

			DrawAnimationProgress(context);

			EditorGUILayout.Separator();

			if (playAnimationToggled && currentSampleAnimation)
			{
				currentSampleAnimation = false;

				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION,
					currentSampleAnimation);
			}

			if (sampleAnimationToggled && currentPlayAnimation)
			{
				currentPlayAnimation = false;

				context.AddOrUpdate(
					KEY_PLAY_ANIMATION,
					currentPlayAnimation);
			}

			if (!playAnimationConditionsMet || animationSourcesDirty)
			{
				context.TryRemove(KEY_SELECTED_ANIMATABLE_PGRAPH);

				context.TryRemove(KEY_SELECTED_ANIMATABLE_CPLAYABLE);
			}

			ARToolboxEditorHelpers.DrawSubcontextControls(
				context,
				trackIndex,
				totalTracksCount,
				KEY_TRACK_PREFIX);

			ARToolboxEditorHelpers.EndSubcontextBox();
		}

		private void DrawCurrentSelectedAnimatable(
			IARToolboxContext context,
			ref bool animationSourcesDirty,
			ref bool playAnimationConditionsMet,
			ref RuntimeAnimatorController currentSelectedRuntimeAnimatorController,
			out GameObject currentSelectedAnimatable)
		{
			if (!context.TryGet<GameObject>(
				KEY_SELECTED_ANIMATABLE,
				out var previousSelectedAnimatable))
			{
				//Initialize to default value
			}

			ARToolboxEditorHelpers.DrawGameObjectInputWithSelectionControls(
				UI_TEXT_TARGET_ANIMATABLE,
				previousSelectedAnimatable,
				out currentSelectedAnimatable);

			if (currentSelectedAnimatable != previousSelectedAnimatable)
			{
				if (currentSelectedAnimatable != null)
				{
					context.AddOrUpdate(
						KEY_SELECTED_ANIMATABLE,
						currentSelectedAnimatable);

					animationSourcesDirty = true;
				}
				else
				{
					context.TryRemove(KEY_SELECTED_ANIMATABLE);
				}
			}

			if (currentSelectedAnimatable != null)
			{
				var animator = currentSelectedAnimatable.GetComponent<Animator>();

				if (animator == null)
				{
					playAnimationConditionsMet = false;

					EditorGUILayout.HelpBox(
						UI_TEXT_NO_ANIMATOR,
						MessageType.Error);

					if (GUILayout.Button(UI_TEXT_ADD_ANIMATOR))
					{
						currentSelectedAnimatable.AddComponent<Animator>();
					}
				}
				else
				{
					currentSelectedRuntimeAnimatorController = animator.runtimeAnimatorController;
				}
			}
			else
			{
				playAnimationConditionsMet = false;
			}
		}

		private void DrawCurrentSelectedAnimation(
			IARToolboxContext context,
			ref bool animationSourcesDirty,
			ref bool playAnimationConditionsMet,
			RuntimeAnimatorController currentSelectedRuntimeAnimatorController)
		{
			if (!context.TryGet<AnimationClip>(
				KEY_SELECTED_ANIMATION,
				out var previousSelectedAnimation))
			{
				//Initialize to default value
			}

			var currentSelectedAnimation = previousSelectedAnimation;

			if (playAnimationConditionsMet)
			{
				if (currentSelectedRuntimeAnimatorController == null)
				{
					EditorGUILayout.HelpBox(
						UI_TEXT_NO_CONTROLLER,
						MessageType.Warning);
				}
				else
				{
					var allClips = currentSelectedRuntimeAnimatorController.animationClips;

					var clipNames = new string[allClips.Length];

					for (int i = 0; i < clipNames.Length; i++)
						clipNames[i] = allClips[i].name;

					int previousSelectedClipIndex = -1;

					if (previousSelectedAnimation != null)
					{
						previousSelectedClipIndex = Array.IndexOf(
							allClips,
							previousSelectedAnimation);
					}

					int currentSelectedClipIndex = EditorGUILayout.Popup(
						UI_TEXT_SELECTED_CLIP_FROM_ANIMATOR,
						previousSelectedClipIndex,
						clipNames);

					if (currentSelectedClipIndex != previousSelectedClipIndex)
					{
						var currentSelectedAnimationFromList = allClips[currentSelectedClipIndex];

						if (currentSelectedAnimationFromList != null)
						{
							context.AddOrUpdate(
								KEY_SELECTED_ANIMATION,
								currentSelectedAnimationFromList);

							animationSourcesDirty = true;
						}
						else
						{
							context.TryRemove(KEY_SELECTED_ANIMATION);
						}

						currentSelectedAnimation = currentSelectedAnimationFromList;
					}
				}
			}

			ARToolboxEditorHelpers.DrawInputWithSelectionFromAssetsControls<AnimationClip>(
				UI_TEXT_TARGET_CLIP,
				previousSelectedAnimation,
				out var currentSelectedAnimationFromInput);

			if (currentSelectedAnimationFromInput != previousSelectedAnimation)
			{
				if (currentSelectedAnimationFromInput != null)
				{
					context.AddOrUpdate(
						KEY_SELECTED_ANIMATION,
						currentSelectedAnimationFromInput);

					animationSourcesDirty = true;
				}
				else
				{
					context.TryRemove(KEY_SELECTED_ANIMATION);
				}

				currentSelectedAnimation = currentSelectedAnimationFromInput;
			}

			if (currentSelectedAnimation == null)
			{
				playAnimationConditionsMet = false;
			}
		}

		private void DrawForceLoop(IARToolboxContext context)
		{
			if (!context.TryGet<bool>(
				KEY_FORCE_LOOP,
				out var previousForceLoop))
			{
				//Initialize to default value
				previousForceLoop = false;

				context.AddOrUpdate(
					KEY_FORCE_LOOP,
					previousForceLoop);
			}

			var currentForceLoop = EditorGUILayout.Toggle(
				UI_TEXT_FORCE_LOOP,
				previousForceLoop);

			if (currentForceLoop != previousForceLoop)
			{
				context.AddOrUpdate(
					KEY_FORCE_LOOP,
					currentForceLoop);
			}
		}

		private void DrawPlayAnimation(
			IARToolboxContext context,
			out bool playAnimationToggled,
			out bool currentPlayAnimation,
			bool playAnimationConditionsMet)
		{
			playAnimationToggled = false;

			if (!context.TryGet<bool>(
				KEY_PLAY_ANIMATION,
				out var previousPlayAnimation))
			{
				//Initialize to default value
				previousPlayAnimation = false;

				context.AddOrUpdate(
					KEY_PLAY_ANIMATION,
					previousPlayAnimation);
			}

			if (previousPlayAnimation && !playAnimationConditionsMet)
			{
				previousPlayAnimation = false;

				context.AddOrUpdate(
					KEY_PLAY_ANIMATION,
					previousPlayAnimation);
			}

			currentPlayAnimation = previousPlayAnimation;

			if (!playAnimationConditionsMet)
				GUI.enabled = false;

			var previousGUIEnabled = GUI.enabled;


			if (previousPlayAnimation)
				GUI.enabled = false;

			if (GUILayout.Button(UI_TEXT_PLAY))
			{
				currentPlayAnimation = true;
			}

			if (previousPlayAnimation)
				GUI.enabled = previousGUIEnabled;


			if (!previousPlayAnimation)
				GUI.enabled = false;

			if (GUILayout.Button(UI_TEXT_STOP))
			{
				currentPlayAnimation = false;
			}

			if (!previousPlayAnimation)
				GUI.enabled = previousGUIEnabled;

			if (!playAnimationConditionsMet)
				GUI.enabled = true;

			if (currentPlayAnimation != previousPlayAnimation)
			{
				playAnimationToggled = true;

				context.AddOrUpdate(
					KEY_PLAY_ANIMATION,
					currentPlayAnimation);
			}
		}

		private void DrawSampleAnimation(
			IARToolboxContext context,
			out bool sampleAnimationToggled,
			out bool currentSampleAnimation,
			bool playAnimationConditionsMet)
		{
			sampleAnimationToggled = false;

			if (!context.TryGet<bool>(
				KEY_SAMPLE_ANIMATION,
				out var previousSampleAnimation))
			{
				//Initialize to default value
				previousSampleAnimation = false;

				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION,
					previousSampleAnimation);
			}

			if (previousSampleAnimation && !playAnimationConditionsMet)
			{
				previousSampleAnimation = false;

				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION,
					previousSampleAnimation);
			}

			currentSampleAnimation = previousSampleAnimation;

			if (!playAnimationConditionsMet)
				GUI.enabled = false;

			if (!currentSampleAnimation)
			{
				if (GUILayout.Button(UI_TEXT_SAMPLE))
				{
					currentSampleAnimation = true;
				}
			}
			else
			{
				if (GUILayout.Button(UI_TEXT_SAMPLING))
				{
					currentSampleAnimation = false;
				}
			}

			if (!playAnimationConditionsMet)
				GUI.enabled = true;

			if (currentSampleAnimation != previousSampleAnimation)
			{
				sampleAnimationToggled = true;

				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION,
					currentSampleAnimation);
			}
		}

		private void DrawAssumeDefaultPose(
			GameObject currentSelectedAnimatable,
			bool currentPlayAnimation,
			bool currentSampleAnimation)
		{
			var cannotAssumeDefaultPose = currentSelectedAnimatable == null
				|| currentPlayAnimation
				|| currentSampleAnimation;

			var previousGUIEnabled = GUI.enabled;

			if (cannotAssumeDefaultPose)
				GUI.enabled = false;

			if (GUILayout.Button(UI_TEXT_DEFAULT_POSE))
			{
				ARToolboxEditorHelpers.AssumeDefaultPose(
					currentSelectedAnimatable);
			}

			if (cannotAssumeDefaultPose)
				GUI.enabled = previousGUIEnabled;
		}

		private void DrawAnimationProgress(IARToolboxContext context)
		{
			if (!context.TryGet<bool>(
				KEY_ANIMATION_PROGRESS_SNORMAL,
				out var previousAnimationProgressShowInNormal))
			{
				//Initialize to default value
				previousAnimationProgressShowInNormal = false;

				context.AddOrUpdate(
					KEY_ANIMATION_PROGRESS_SNORMAL,
					previousAnimationProgressShowInNormal);
			}

			float currentProgress = 0f;

			float totalDuration = 0f;

			float normalProgress = 0f;

			if (context.TryGet<AnimationClip>(
				KEY_SELECTED_ANIMATION,
				out var clip))
			{
				totalDuration = clip.length;
			}

			if (context.TryGet<AnimationClipPlayable>(
				KEY_SELECTED_ANIMATABLE_CPLAYABLE,
				out var clipPlayable))
			{
				if (clipPlayable.IsValid())
				{
					currentProgress = Convert.ToSingle(
						PlayableExtensions.GetTime(clipPlayable));
				}
			}

			if (totalDuration > 0f)
			{
				currentProgress = currentProgress % totalDuration;
	
				normalProgress = currentProgress / totalDuration;
			}

			EditorGUI.ProgressBar(
				EditorGUILayout.GetControlRect(),
				normalProgress,
				string.Empty);

			EditorGUILayout.BeginHorizontal();

			if (!previousAnimationProgressShowInNormal)
			{
				ARToolboxEditorHelpers.DrawLabelSelectable(
					$"[ {currentProgress:F2} / {totalDuration:F2} ]");
			}
			else
			{
				ARToolboxEditorHelpers.DrawLabelSelectable(
					$"[ {normalProgress:F2} / {(1f):F2} ]");
			}

			var currentAnimationProgressShowInNormal = ARToolboxEditorHelpers.DrawToggleTight(
				UI_TEXT_NORMAL,
				previousAnimationProgressShowInNormal);

			if (currentAnimationProgressShowInNormal != previousAnimationProgressShowInNormal)
			{
				context.AddOrUpdate(
					KEY_ANIMATION_PROGRESS_SNORMAL,
					currentAnimationProgressShowInNormal);
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawSampleAnimationNormalTime(
			IARToolboxContext context,
			bool playAnimationConditionsMet)
		{
			if (!context.TryGet<float>(
				KEY_SAMPLE_ANIMATION_NORMAL_TIME,
				out var previousSampleAnimationNormalTime))
			{
				//Initialize to default value
				previousSampleAnimationNormalTime = 0f;

				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION_NORMAL_TIME,
					previousSampleAnimationNormalTime);
			}

			if (!context.TryGet<bool>(
				KEY_SAMPLE_ANIMATION_NORMAL_TIME_SNORMAL,
				out var previousSampleAnimationNormalTimeShowInNormal))
			{
				//Initialize to default value
				previousSampleAnimationNormalTimeShowInNormal = true;

				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION_NORMAL_TIME_SNORMAL,
					previousSampleAnimationNormalTimeShowInNormal);
			}

			if (!playAnimationConditionsMet)
				GUI.enabled = false;

			float currentSampleAnimationNormalTime = 0f;

			EditorGUILayout.BeginHorizontal();

			if (previousSampleAnimationNormalTimeShowInNormal
				|| !playAnimationConditionsMet)
			{
				currentSampleAnimationNormalTime = EditorGUILayout.Slider(
					UI_TEXT_SAMPLE_TIME,
					previousSampleAnimationNormalTime,
					0f,
					1f);
			}
			else
			{
				context.TryGet<AnimationClip>(
					KEY_SELECTED_ANIMATION,
					out var clip);

				currentSampleAnimationNormalTime = EditorGUILayout.Slider(
					UI_TEXT_SAMPLE_TIME,
					previousSampleAnimationNormalTime * clip.length,
					0f,
					clip.length) / clip.length;
			}

			var currentSampleAnimationNormalTimeShowInNormal = ARToolboxEditorHelpers.DrawToggleTight(
				UI_TEXT_NORMAL,
				previousSampleAnimationNormalTimeShowInNormal);

			if (currentSampleAnimationNormalTimeShowInNormal != previousSampleAnimationNormalTimeShowInNormal)
			{
				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION_NORMAL_TIME_SNORMAL,
					currentSampleAnimationNormalTimeShowInNormal);
			}

			EditorGUILayout.EndHorizontal();

			if (!playAnimationConditionsMet)
				GUI.enabled = true;

			if (currentSampleAnimationNormalTime != previousSampleAnimationNormalTime)
			{
				context.AddOrUpdate(
					KEY_SAMPLE_ANIMATION_NORMAL_TIME,
					currentSampleAnimationNormalTime);
			}
		}

		private void PlayTrack(
			IARToolboxContext context)
		{
			if (!context.TryGet<bool>(
				KEY_PLAY_ANIMATION,
				out var playAnimation))
			{
				return;
			}

			if (!context.TryGet<bool>(
				KEY_SAMPLE_ANIMATION,
				out var sampleAnimation))
			{
				return;
			}

			if (playAnimation)
			{
				if (!context.Has(KEY_SELECTED_ANIMATABLE_PGRAPH))
				{
					InitializeAnimation(context);
				}
				else
				{
					PlayAnimation(context);
				}
			}

			if (sampleAnimation)
			{
				if (!context.Has(KEY_SELECTED_ANIMATABLE_PGRAPH))
				{
					InitializeAnimation(context);
				}

				SampleAnimation(context);
			}
		}

		//Courtesy of https://discussions.unity.com/t/preview-playable-in-editor/677702/3
		private void InitializeAnimation(
			IARToolboxContext context)
		{
			if (!context.TryGet<GameObject>(
				KEY_SELECTED_ANIMATABLE,
				out var targetAnimatable))
			{
				UnityEngine.Debug.LogError(
					"ANIMATABLE NOT SELECTED");

				return;
			}

			if (!context.TryGet<AnimationClip>(
				KEY_SELECTED_ANIMATION,
				out var targetClip))
			{
				UnityEngine.Debug.LogError(
					"CLIP NOT SELECTED");

				return;
			}

			PlayableGraph graph = PlayableGraph.Create();

			var animOutput = AnimationPlayableOutput.Create(
				graph,
				GRAPH_OUTPUT_PLAYABLE_NAME,
				targetAnimatable.GetComponent<Animator>());

			var clipPlayable = AnimationClipPlayable.Create(
				graph,
				targetClip);

			animOutput.SetSourcePlayable(
				clipPlayable);

			graph.Evaluate(0f);

			context.AddOrUpdate(
				KEY_SELECTED_ANIMATABLE_PGRAPH,
				graph);

			context.AddOrUpdate(
				KEY_SELECTED_ANIMATABLE_CPLAYABLE,
				clipPlayable);
		}

		private void PlayAnimation(
			IARToolboxContext context)
		{
			if (!context.TryGet<PlayableGraph>(
				KEY_SELECTED_ANIMATABLE_PGRAPH,
				out var graph))
			{
				UnityEngine.Debug.LogError(
					"INVALID GRAPH");

				return;
			}

			if (!graph.IsValid())
			{
				UnityEngine.Debug.LogError(
					"INVALIDATED GRAPH");

				context.TryRemove(KEY_SELECTED_ANIMATABLE_PGRAPH);

				return;
			}

			if (!context.TryGet<AnimationClipPlayable>(
				KEY_SELECTED_ANIMATABLE_CPLAYABLE,
				out var clipPlayable))
			{
				UnityEngine.Debug.LogError(
					"INVALID ANIMATION CLIP PLAYABLE");

				return;
			}

			if (!clipPlayable.IsValid())
			{
				UnityEngine.Debug.LogError(
					"INVALIDATED CLIP PLAYABLE");

				context.TryRemove(KEY_SELECTED_ANIMATABLE_CPLAYABLE);

				return;
			}

			if (!context.TryGet<AnimationClip>(
				KEY_SELECTED_ANIMATION,
				out var clip))
			{
				UnityEngine.Debug.LogError(
					"INVALID ANIMATION CLIP");

				return;
			}

			if (!context.TryGet<bool>(
				KEY_FORCE_LOOP,
				out var forceLoop))
			{
				UnityEngine.Debug.LogError(
					"INVALID FORCE LOOP");

				return;
			}

			var currentProgress = Convert.ToSingle(
				PlayableExtensions.GetTime(clipPlayable));

			var totalDuration = clip.length;

			bool doTick = true;

			if (currentProgress >= totalDuration)
			{
				var updatedProgress = currentProgress % totalDuration;

				PlayableExtensions.SetTime(
					clipPlayable,
					updatedProgress);

				if (!clip.isLooping && !forceLoop)
				{
					doTick = false;

					PlayableExtensions.SetTime(
						clipPlayable,
						0f);
						//totalDuration - 0.01); //0.01f so the progress bar does not overflow

					graph.Evaluate(0f);

					context.AddOrUpdate(
						KEY_PLAY_ANIMATION,
						false);
				}
			}

			if (doTick)
				graph.Evaluate((float)context.DeltaTime);
		}

		private void SampleAnimation(
			IARToolboxContext context)
		{
			if (!context.TryGet<PlayableGraph>(
				KEY_SELECTED_ANIMATABLE_PGRAPH,
				out var graph))
			{
				UnityEngine.Debug.LogError(
					"INVALID GRAPH");

				return;
			}

			if (!graph.IsValid())
			{
				UnityEngine.Debug.LogError(
					"INVALIDATED GRAPH");

				context.TryRemove(KEY_SELECTED_ANIMATABLE_PGRAPH);

				return;
			}

			if (!context.TryGet<AnimationClipPlayable>(
				KEY_SELECTED_ANIMATABLE_CPLAYABLE,
				out var clipPlayable))
			{
				UnityEngine.Debug.LogError(
					"INVALID ANIMATION CLIP PLAYABLE");

				return;
			}

			if (!clipPlayable.IsValid())
			{
				UnityEngine.Debug.LogError(
					"INVALIDATED CLIP PLAYABLE");

				context.TryRemove(KEY_SELECTED_ANIMATABLE_CPLAYABLE);

				return;
			}

			if (!context.TryGet<AnimationClip>(
				KEY_SELECTED_ANIMATION,
				out var clip))
			{
				UnityEngine.Debug.LogError(
					"INVALID ANIMATION CLIP");

				return;
			}

			if (!context.TryGet<float>(
				KEY_SAMPLE_ANIMATION_NORMAL_TIME,
				out var normalTime))
			{
				UnityEngine.Debug.LogError(
					"INVALID ANIMATION NORMAL TIME");

				return;
			}

			PlayableExtensions.SetTime(
				clipPlayable,
				Convert.ToDouble(normalTime * clip.length));

			graph.Evaluate(0f);
		}
	}
}
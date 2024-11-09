using System;
using System.Collections.Generic;
using System.Linq;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Hierarchy;

using UnityEngine;

using UnityEditor;

namespace HereticalSolutions.Tools.AnimationRetargettingToolbox
{
	public class SkeletonTool
		: IARToolboxToolWindow
	{
		#region Constants

		#region UI consts

		private const string UI_TEXT_TITLE = "Skeleton tool";

		private const string UI_TEXT_SKELETON_LOWERCASE = "skeleton";

		private const string UI_TEXT_SKELETON_CAMELCASE = "Skeleton";

		private const string UI_TEXT_TARGET_SKELETON_DONOR = "Target skeleton donor:";

		private const string UI_TEXT_NO_SMESHRENDERER = "No SkinnedMeshRenderer found in the selected skeleton donor";

		private const string UI_TEXT_SAMPLE = "Sample";

		#endregion

		#region Metadata keys

		private const string KEY_TOOL_PREFIX = "SkeletonTool";

		private const string KEY_SKELETONS = "SkeletonTool_Tracks";

		private const string KEY_SKELETON_PREFIX = "Skeleton";



		private const string KEY_SELECTED_SKELETON_DONOR = "Skeleton_SelectedSkeletonDonor";

		private const string KEY_SKELETON = "Skeleton_Skeleton";

		private const string KEY_SKELETON_CURRENT_POSE = "Skeleton_CurrentPose";

		private const string KEY_SKELETON_SELECTED_BONE = "Skeleton_SelectedBone";

		private const string KEY_HANDLE_TO_BONE_MAP = "HandleToBoneMap";

		#endregion

		private const float BONE_FORWARD_HANDLES_LENGTH = 0.05f;

		private const int START_BONE_HANDLE_INDEX = 1000;

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
				var skeletons = ARToolboxEditorHelpers.GetSubcontextList(
					context,
					KEY_SKELETONS,
					UI_TEXT_SKELETON_LOWERCASE);

				for (int i = 0; i < skeletons.Count; i++)
				{
					DrawSkeleton(
						skeletons[i],
						i,
						skeletons.Count);
				}

				ARToolboxEditorHelpers.UpdateSubcontextList(
					skeletons,
					KEY_SKELETON_PREFIX);
			}

			ARToolboxEditorHelpers.EndInnerBox();
		}

		public void DrawHandles(IARToolboxContext context)
		{
			//if (!ARToolboxEditorHelpers.GetEnabled(
			//	context,
			//	KEY_TOOL_PREFIX))
			//{
			//	return;
			//}

			if (!context.TryGet<List<IARToolboxContext>>(
				KEY_SKELETONS,
				out var skeletons))
			{
				return;
			}

			if (!context.TryGet<IRepository<int, BoneWrapper>>(
				KEY_HANDLE_TO_BONE_MAP,
				out var handleToBoneMap))
			{
				handleToBoneMap = RepositoriesFactory.BuildDictionaryRepository<int, BoneWrapper>();

				context.AddOrUpdate<IRepository<int, BoneWrapper>>(
					KEY_HANDLE_TO_BONE_MAP,
					handleToBoneMap);
			}

			int currentFreeHandle = START_BONE_HANDLE_INDEX;

			foreach (var skeleton in skeletons)
			{
				DrawSkeletonHandles(
					skeleton,
					ref currentFreeHandle,
					handleToBoneMap);
			}
		}

		public void SceneUpdate(IARToolboxContext context)
		{
		}

		#endregion

		private void DrawSkeleton(
			IARToolboxContext context,
			int skeletonIndex,
			int totalSkeletonsCount)
		{
			ARToolboxEditorHelpers.BeginSubcontextBoxWithTitle(
				UI_TEXT_SKELETON_CAMELCASE,
				skeletonIndex);

			DrawCurrentSelectedSkeletonDonor(
				context);

			DrawSampleSkeleton(
				context);

			EditorGUILayout.Separator();

			DrawCurrentSelectedBone(
				context);

			ARToolboxEditorHelpers.DrawSubcontextControls(
				context,
				skeletonIndex,
				totalSkeletonsCount,
				KEY_SKELETON_PREFIX);

			ARToolboxEditorHelpers.EndSubcontextBox();
		}

		private void DrawCurrentSelectedSkeletonDonor(
			IARToolboxContext context)
		{
			if (!context.TryGet<GameObject>(
				KEY_SELECTED_SKELETON_DONOR,
				out var previousSelectedSkeletonDonor))
			{
				//Initialize to default value
			}

			ARToolboxEditorHelpers.DrawGameObjectInputWithSelectionControls(
				UI_TEXT_TARGET_SKELETON_DONOR,
				previousSelectedSkeletonDonor,
				out var currentSelectedSkeletonDonor);

			if (currentSelectedSkeletonDonor != null)
			{
				var skinnedMeshRenderer = currentSelectedSkeletonDonor.GetComponentInChildren<SkinnedMeshRenderer>();

				if (skinnedMeshRenderer == null)
				{
					EditorGUILayout.HelpBox(
						UI_TEXT_NO_SMESHRENDERER,
						MessageType.Warning);
				}
			}

			if (currentSelectedSkeletonDonor != previousSelectedSkeletonDonor)
			{
				if (currentSelectedSkeletonDonor != null)
				{
					context.AddOrUpdate(
						KEY_SELECTED_SKELETON_DONOR,
						currentSelectedSkeletonDonor);
				}
				else
				{
					context.TryRemove(KEY_SELECTED_SKELETON_DONOR);
				}
			}
		}

		private void DrawSampleSkeleton(
			IARToolboxContext context)
		{
			if (!context.TryGet<GameObject>(
				KEY_SELECTED_SKELETON_DONOR,
				out var selectedSkeletonDonor))
			{
				return;
			}

			if (GUILayout.Button(
				UI_TEXT_SAMPLE))
			{
				SampleSkeleton(context);
			}
		}

		private void DrawCurrentSelectedBone(
			IARToolboxContext context)
		{
			
		}

		private void SampleSkeleton(
			IARToolboxContext context)
		{
			if (!context.TryGet<GameObject>(
				KEY_SELECTED_SKELETON_DONOR,
				out var selectedSkeletonDonor))
			{
				UnityEngine.Debug.LogError(
					"INVALID SKELETON DONOR");

				return;
			}

			ARToolboxEditorHelpers.AssumeDefaultPose(selectedSkeletonDonor);

			var skinnedMeshRenderer = selectedSkeletonDonor
				.transform
				.GetComponentInChildren<SkinnedMeshRenderer>();

			SampleBoneTransform(
				selectedSkeletonDonor.transform,
				skinnedMeshRenderer,
				out var rootNode);

			var skeleton = new SkeletonWrapper(
				selectedSkeletonDonor,
				rootNode);

			context.AddOrUpdate(
				KEY_SKELETON,
				skeleton);

			var currentPose = SamplePose(skeleton);

			context.AddOrUpdate(
				KEY_SKELETON_CURRENT_POSE,
				currentPose);
		}

		private void SampleBoneTransform(
			Transform boneTransform,
			SkinnedMeshRenderer skinnedMeshRenderer,
			out IHierarchyNode<BoneWrapper> boneNode)
		{
			boneNode = new HierarchyNode<BoneWrapper>(
				new List<IReadOnlyHierarchyNode<BoneWrapper>>());

			boneNode.Contents = new BoneWrapper(
				boneTransform.gameObject.name,
				boneTransform,
				new BoneSnapshot
				{
					Position = boneTransform.position,
					Rotation = boneTransform.rotation,
					BoneMode = (skinnedMeshRenderer == null)
						? EBoneMode.BONE
						: (skinnedMeshRenderer.bones.Contains(boneTransform))
							? EBoneMode.BONE
							: EBoneMode.LOCATOR
				});

			foreach (Transform child in boneTransform)
			{
				SampleBoneTransform(
					child,
					skinnedMeshRenderer,
					out var childNode);

				boneNode.AddChild(
					childNode);
			}
		}

		private PoseSnapshot SamplePose(
			SkeletonWrapper skeleton)
		{
			IRepository<BoneWrapper, BoneSnapshot> boneSnapshots =
				RepositoriesFactory.BuildDictionaryRepository<BoneWrapper, BoneSnapshot>();

			var skinnedMeshRenderer = skeleton
				.RootNode
				.Contents
				.BoneTransform
				.GetComponentInChildren<SkinnedMeshRenderer>();

			SampleBone(
				skeleton.RootNode,
				boneSnapshots,
				skinnedMeshRenderer);

			return new PoseSnapshot(
				boneSnapshots);
		}

		private void SampleBone(
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			IRepository<BoneWrapper, BoneSnapshot> boneSnapshots,
			SkinnedMeshRenderer skinnedMeshRenderer)
		{
			var boneTransform = boneNode.Contents.BoneTransform;

			boneSnapshots.Add(
				boneNode.Contents,
				new BoneSnapshot
				{
					Position = boneTransform.position,
					Rotation = boneTransform.rotation,
					BoneMode = (skinnedMeshRenderer == null)
						? EBoneMode.BONE
						: (skinnedMeshRenderer.bones.Contains(boneTransform))
							? EBoneMode.BONE
							: EBoneMode.LOCATOR
				});

			foreach (var child in boneNode.Children)
			{
				SampleBone(
					child,
					boneSnapshots,
					skinnedMeshRenderer);
			}
		}

		private void UpdatePose(
			SkeletonWrapper skeleton,
			PoseSnapshot pose)
		{
			var snapshotKeys = pose.BoneSnapshots.Keys.ToArray();

			foreach (var snapshotKey in snapshotKeys)
			{
				var boneTransform = snapshotKey.BoneTransform;

				var previousSnapshot = pose.BoneSnapshots.Get(snapshotKey);

				((IRepository<BoneWrapper, BoneSnapshot>)pose.BoneSnapshots).Update(
					snapshotKey,
					new BoneSnapshot
					{
						Position = boneTransform.position,
						Rotation = boneTransform.rotation,
						BoneMode = previousSnapshot.BoneMode
					});
			}
		}

		//Courtesy of https://adroit-things.com/game-engine/exploring-unity-editor-handle-caps/
		private void DrawSkeletonHandles(
			IARToolboxContext context,
			ref int currentFreeHandle,
			IRepository<int, BoneWrapper> handleToBoneMap)
		{
			if (!context.TryGet<SkeletonWrapper>(
				KEY_SKELETON,
				out var skeleton))
			{
				return;
			}

			if (!context.TryGet<PoseSnapshot>(
				KEY_SKELETON_CURRENT_POSE,
				out var currentPose))
			{
				return;
			}

			UpdatePose(
				skeleton,
				currentPose);

			var currentEvent = Event.current;

			if (currentEvent.type == EventType.Layout)
			{
				DrawBoneHandles(
					context,

					ref currentFreeHandle,
					handleToBoneMap,
					EventType.Layout,

					skeleton.RootNode,
					currentPose);
			}

			if (currentEvent.type == EventType.Repaint)
			{
				DrawBoneHandles(
					context,

					ref currentFreeHandle,
					handleToBoneMap,
					EventType.Repaint,

					skeleton.RootNode,
					currentPose);
			}

			if (currentEvent.type == EventType.MouseDown
				&& currentEvent.button == 0)
			{
				SelectClickedBone(
					handleToBoneMap);
			}
		}

		private void DrawBoneHandles(
			IARToolboxContext context,

			ref int currentFreeHandle,
			IRepository<int, BoneWrapper> handleToBoneMap,
			EventType eventType,

			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			PoseSnapshot currentPose)
		{
			if (boneNode.Parent != null)
			{
				if (currentPose.BoneSnapshots.TryGet(
					boneNode.Contents,
					out var boneSnapshot))
				{
					var parentPosition = boneNode.Parent.Contents.BoneTransform.position;

					var positionDistance = boneSnapshot.Position - parentPosition;

					if (positionDistance.magnitude > 0f)
					{
						var rotationFromParent = Quaternion.LookRotation(
							positionDistance,
							Vector3.up);
	
						var previousHandlesColor = Handles.color;
	
						switch (boneSnapshot.BoneMode)
						{
							case EBoneMode.BONE:
								Handles.color = Color.white;
								break;
	
							case EBoneMode.LOCATOR:
								Handles.color = Color.magenta;
								break;
	
							case EBoneMode.IK_TARGET:
								Handles.color = Color.cyan;
								break;
						}


						var handle = currentFreeHandle;

						handleToBoneMap.AddOrUpdate(
							handle,
							boneNode.Contents);

						currentFreeHandle++;


						Handles.ArrowHandleCap(
							handle,
							parentPosition,
							rotationFromParent,
							positionDistance.magnitude * ARToolboxEditorHelpers.HANDLES_LENGTH_FACTOR,
							eventType);

						Handles.color = previousHandlesColor;

						if (eventType == EventType.Repaint)
						{
							ARToolboxEditorHelpers.DrawAdjustablePositionHandle(
								boneSnapshot.Position,
								boneSnapshot.Rotation,
								BONE_FORWARD_HANDLES_LENGTH);
						}

						//Handles.PositionHandle(
						//	boneSnapshot.Position,
						//	boneSnapshot.Rotation);
					}
				}
			}

			foreach (var child in boneNode.Children)
			{
				DrawBoneHandles(
					context,

					ref currentFreeHandle,
					handleToBoneMap,
					eventType,

					child,
					currentPose);
			}
		}

		//Courtesy of https://discussions.unity.com/t/how-to-drag-and-select-multiple-handles-in-editor-script/830359/3
		private void SelectClickedBone(
			IRepository<int, BoneWrapper> handleToBoneMap)
		{
			var handle = HandleUtility.nearestControl;

			if (handleToBoneMap.TryGet(
				handle,
				out var bone))
			{
				UnityEngine.Debug.Log(
					$"Selected bone: {bone.BoneTransform.gameObject.name} control: {handle}");
			}
		}
	}
}
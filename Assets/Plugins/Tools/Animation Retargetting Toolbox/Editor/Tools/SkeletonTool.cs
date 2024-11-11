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

		private const float MIN_BONE_LENGTH = 0.1f;

		private const float BONE_CUSTOM_EPSILON = 0.01f;

		private static readonly Color BONE_DOTTED_LINE_COLOR = new Color(1f, 1f, 1f, 0.8f);

		private const float BONE_DOTTED_LINE_LENGTH = 5;

		private const int START_BONE_HANDLE_INDEX = 1000;

		private static readonly Quaternion[] ROTATION_1_AXIS_PERMUTATIONS = new Quaternion[]
		{
			Quaternion.identity,
			Quaternion.Euler(0, 0, 0),
			Quaternion.Euler(0, 0, 90),
			Quaternion.Euler(0, 0, -90),
			Quaternion.Euler(0, 0, 180)
		};

		private static readonly Quaternion[] ROTATION_3_AXIS_PERMUTATIONS = new Quaternion[]
		{
			Quaternion.identity,
			Quaternion.Euler(90, 0, 0),
			Quaternion.Euler(-90, 0, 0),
			Quaternion.Euler(0, 90, 0),
			Quaternion.Euler(0, -90, 0),
			Quaternion.Euler(0, 0, 90),
			Quaternion.Euler(0, 0, -90),
			Quaternion.Euler(180, 0, 0),
			Quaternion.Euler(0, 180, 0),
			Quaternion.Euler(0, 0, 180)
		};

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

			SanitizeBones(
				skeleton,
				selectedSkeletonDonor.transform);

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

		private void SanitizeBones(
			SkeletonWrapper skeleton,
			Transform origin)
		{
			SanitizeBone(
				skeleton.RootNode,
				origin);
		}

		private void SanitizeBone(
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			Transform origin)
		{
			TrySanitizePose(
				boneNode,
				origin);

			foreach (var child in boneNode.Children)
			{
				SanitizeBone(
					child,
					origin);
			}
		}

		private void TrySanitizePose(
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			Transform origin)
		{
			var poseSnapshot = boneNode.Contents.PoseSnapshot;

			//if (poseSnapshot.BoneMode != EBoneMode.BONE)
			//{
			//	return;
			//}

			Vector3 desiredDirection = Vector3.zero;

			if (boneNode.ChildCount != 0)
			{
				Vector3 averageChildPosition = Vector3.zero;

				foreach (var child in boneNode.Children)
				{
					averageChildPosition += child.Contents.PoseSnapshot.Position;
				}

				averageChildPosition /= boneNode.ChildCount;

				var positionDistance = averageChildPosition - poseSnapshot.Position;

				if (!(positionDistance.magnitude < MathHelpers.EPSILON))
				{
					desiredDirection = positionDistance.normalized;
				}
			}
			
			if (desiredDirection.magnitude < MathHelpers.EPSILON
				&& boneNode.Parent != null)
			{
				var parentPosition = boneNode.Parent.Contents.PoseSnapshot.Position;

				var positionDistance = poseSnapshot.Position - parentPosition;

				if (positionDistance.magnitude < MathHelpers.EPSILON)
				{
					return;
				}

				desiredDirection = positionDistance.normalized;
			}

			var closiestRotation = GetClosiestRotationWithPermutations(
				poseSnapshot.Rotation,
				desiredDirection);

			closiestRotation = RotateAroundZToMatchDirectionWithPermutations(
				closiestRotation,
				origin.forward);

			//I like this one more. Why?
			//Because the blue arrow is pointed away from the parent bone
			//And once the character is stretching their arm forward, the blue arrow points forward,
			//The green arrow points upward, and the red arrow points to the right
			//Just as expected
			closiestRotation *= Quaternion.Euler(0, 0, -90);

			var rotationDifference = Quaternion.Inverse(poseSnapshot.Rotation) * closiestRotation;

			//var rotationDifference = Quaternion.Inverse(poseSnapshot.Rotation) * rotationFromParent;

			boneNode.Contents.SanitationSnapshot = new BoneSnapshot
			{
				Position = Vector3.zero,
				Rotation = rotationDifference,
				BoneMode = EBoneMode.SANITATION
			};
		}

		private Quaternion GetClosiestRotationWithPermutations(
			Quaternion boneRotation,
			Vector3 desiredDirection)
		{
			Quaternion bestRotation = boneRotation;

			float bestAlignment = float.MinValue;

			// Iterate through all possible rotations
			foreach (var rotationPermutation in ROTATION_3_AXIS_PERMUTATIONS)
			{
				// Apply the rotation to the bone's quaternion
				Quaternion rotatedBone = boneRotation * rotationPermutation;

				// Calculate the alignment with the desired direction
				Vector3 boneForward = rotatedBone * Vector3.forward;

				float alignment = Vector3.Dot(
					boneForward.normalized,
					desiredDirection.normalized);

				// Select the rotation with the best alignment
				if (alignment > bestAlignment)
				{
					bestAlignment = alignment;

					bestRotation = rotatedBone;
				}
			}

			return bestRotation;
		}

		private Quaternion RotateAroundZToMatchDirectionWithPermutations(
			Quaternion boneRotation,
			Vector3 desiredDirection)
		{
			Quaternion bestRotation = boneRotation;

			float bestAlignment = float.MinValue;

			// Iterate through all possible rotations
			foreach (var rotationPermutation in ROTATION_1_AXIS_PERMUTATIONS)
			{
				// Apply the rotation to the bone's quaternion
				Quaternion rotatedBone = boneRotation * rotationPermutation;

				// Calculate the alignment with the desired direction
				Vector3 boneForward = rotatedBone * Vector3.forward;
				Vector3 boneRight = rotatedBone * Vector3.right;

				float alignment = Vector3.Dot(boneForward.normalized, desiredDirection.normalized) +
								  Vector3.Dot(boneRight.normalized, desiredDirection.normalized);

				// Select the rotation with the best alignment
				if (alignment > bestAlignment)
				{
					bestAlignment = alignment;
					bestRotation = rotatedBone;
				}
			}

			return bestRotation;
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
					ref currentFreeHandle,
					handleToBoneMap,
					EventType.Layout,

					skeleton.RootNode,
					currentPose);
			}

			if (currentEvent.type == EventType.Repaint)
			{
				DrawBoneHandles(
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
			ref int currentFreeHandle,
			IRepository<int, BoneWrapper> handleToBoneMap,
			EventType eventType,

			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			PoseSnapshot currentPose)
		{
			TryDrawBoneHandles(
				ref currentFreeHandle,
				handleToBoneMap,
				eventType,

				boneNode,
				currentPose);

			foreach (var child in boneNode.Children)
			{
				DrawBoneHandles(
					ref currentFreeHandle,
					handleToBoneMap,
					eventType,

					child,
					currentPose);
			}
		}

		private void TryDrawBoneHandles(
			ref int currentFreeHandle,
			IRepository<int, BoneWrapper> handleToBoneMap,
			EventType eventType,

			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			PoseSnapshot currentPose)
		{
			if (!currentPose.BoneSnapshots.TryGet(
				boneNode.Contents,
				out var boneSnapshot))
			{
				return;
			}


			Color previousHandlesColor;

			
			//Get bone data
			var bonePosition = boneSnapshot.Position;

			var boneSanitizedRotation = boneSnapshot.Rotation * boneNode.Contents.SanitationSnapshot.Rotation;

			var boneForward = boneSanitizedRotation * Vector3.forward;

			float boneLength = MIN_BONE_LENGTH;

			//Calculate bone length
			if (boneNode.ChildCount != 0)
			{
				float averageDot = 0f;

				int count = 0;

				bool bestFitFound = false;

				foreach (var child in boneNode.Children)
				{
					//Ignore children that are not in the current pose
					if (!currentPose
						.BoneSnapshots
						.TryGet(
							child.Contents,
							out var childSnapshot))
					{
						continue;
					}

					Vector3 childPosition = childSnapshot.Position;

					Vector3 positionDistance = childPosition - bonePosition;

					var positionDistanceMagnitude = positionDistance.magnitude;

					float dot = Vector3.Dot(
						positionDistance,
						boneForward);

					dot = Mathf.Clamp(
						dot,
						-positionDistanceMagnitude,
						positionDistanceMagnitude);

					averageDot += dot;

					count++;


					//Account for best fit (i.e. when a child starts at the bone tip's position)
					if ((1f - (dot / positionDistanceMagnitude)) < BONE_CUSTOM_EPSILON
						&& dot > 0)
					{
						boneLength = dot;

						bestFitFound = true;

						break;
					}
				}

				//If no best fit found, bone tip shall be at the average of the children's bone positions projected on the bone's forward vector
				if (!bestFitFound && count > 0)
				{
					averageDot /= count;

					if (averageDot > 0)
					{
						boneLength = averageDot;
					}
				}
			}

			var boneTipPosition = bonePosition + boneForward * boneLength;

			//For each child bone draw a dotted line in case the child bone is not properly joined to the bone's tip
			foreach (var child in boneNode.Children)
			{
				//Ignore children that are not in the current pose
				if (!currentPose
					.BoneSnapshots
					.TryGet(
						child.Contents,
						out var childSnapshot))
				{
					continue;
				}

				Vector3 childPosition = childSnapshot.Position;


				//Set the bone color
				previousHandlesColor = Handles.color;

				Handles.color = BONE_DOTTED_LINE_COLOR;

				Handles.DrawDottedLine(
					//bonePosition,
					boneTipPosition,
					childPosition,
					BONE_DOTTED_LINE_LENGTH);

				Handles.color = previousHandlesColor;
			}

			//Allocate handle
			var handle = currentFreeHandle;

			handleToBoneMap.AddOrUpdate(
				handle,
				boneNode.Contents);

			currentFreeHandle++;


			//Set the bone color
			previousHandlesColor = Handles.color;

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

			//Draw the bone
			Handles.ArrowHandleCap(
				handle,
				bonePosition,
				boneSanitizedRotation,
				boneLength * ARToolboxEditorHelpers.HANDLES_LENGTH_FACTOR,
				eventType);

			Handles.color = previousHandlesColor;


			if (eventType == EventType.Repaint)
			{
				ARToolboxEditorHelpers.DrawAdjustablePositionHandle(
					boneSnapshot.Position,
					boneSanitizedRotation,
					BONE_FORWARD_HANDLES_LENGTH);

				//Handles.PositionHandle(
				//	boneSnapshot.Position,
				//	boneSnapshot.Rotation * boneNode.Contents.SanitationSnapshot.Rotation);
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
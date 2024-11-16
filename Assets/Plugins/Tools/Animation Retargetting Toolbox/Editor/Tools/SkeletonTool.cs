using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Hierarchy;

using UnityEngine;

using UnityEditor;

using ChangeParentRequest = System.Tuple<
	HereticalSolutions.Hierarchy.IReadOnlyHierarchyNode<HereticalSolutions.Tools.AnimationRetargettingToolbox.BoneWrapper>,
	HereticalSolutions.Hierarchy.IReadOnlyHierarchyNode<HereticalSolutions.Tools.AnimationRetargettingToolbox.BoneWrapper>[]>;

using RemoveBoneRequest = System.Tuple<
	HereticalSolutions.Hierarchy.IReadOnlyHierarchyNode<HereticalSolutions.Tools.AnimationRetargettingToolbox.BoneWrapper>,
	bool>;

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

		private const string UI_TEXT_SANITIZE = "Sanitize";

		private const string UI_TEXT_SHOW_BONE_DIRECTION = "Show bone direction";

		private const string UI_TEXT_HIERARCHY = "Hierarchy";

		private const string UI_TEXT_SELECT_ALL = "Select all";

		private const string UI_TEXT_DESELECT_ALL = "Deselect all";

		private const string UI_TEXT_HIERARCHY_CHILD_PREFIX = "└";

		private const string UI_TEXT_HIERARCHY_PASSTHROUGH_PREFIX = "│";

		private const string UI_TEXT_SELECT = "Select";

		private const string UI_TEXT_SELECTED = "Selected";

		private const string UI_TEXT_SET_PARENT = "Set parent";

		private const string UI_TEXT_REMOVE = "Remove";

		private const string UI_TEXT_NO_SMESHRENDERER = "No SkinnedMeshRenderer found in the selected skeleton donor";

		private const string UI_TEXT_SAMPLE = "Sample";

		#endregion

		#region Metadata keys

		private const string KEY_TOOL_PREFIX = "SkeletonTool";

		private const string KEY_SKELETONS = "SkeletonTool_Tracks";

		private const string KEY_SKELETON_PREFIX = "Skeleton";



		private const string KEY_SELECTED_SKELETON_DONOR = "Skeleton_SelectedSkeletonDonor";

		private const string KEY_SKELETON_SANITIZE = "Skeleton_Sanitize";

		private const string KEY_SKELETON_DRAW_BONE_DIRECTION_GIZMO = "Skeleton_DrawBoneDirectionGizmo";

		private const string KEY_SKELETON_DRAW_HIERARCHY = "Skeleton_DrawHierarchy";

		private const string KEY_SKELETON = "Skeleton_Skeleton";

		private const string KEY_SKELETON_CURRENT_POSE = "Skeleton_CurrentPose";

		private const string KEY_SKELETON_SELECTED_BONES = "Skeleton_SelectedBones";

		private const string KEY_SKELETON_BONES_CHANGE_PARENT_REQUEST = "Skeleton_BonesChangeParentRequest";

		private const string KEY_SKELETON_BONE_REMOVE_REQUEST = "Skeleton_BoneRemoveRequest";

		private const string KEY_HANDLE_TO_BONE_MAP = "HandleToBoneMap";

		#endregion

		private const float BONE_FORWARD_HANDLES_LENGTH = 0.05f;

		private const float MIN_BONE_LENGTH = 0.1f;

		private const float BONE_CUSTOM_EPSILON = 0.01f;

		private static readonly Color BONE_DOTTED_LINE_COLOR = new Color(1f, 1f, 1f, 0.8f);

		private static readonly Color BONE_UNSELECTED_COLOR_MULTIPLICATIVE = new Color(0.6f, 0.6f, 0.6f, 1f);

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
			if (!context.TryGet<List<IARToolboxContext>>(
				KEY_SKELETONS,
				out var skeletons))
			{
				return;
			}

			if (!context.TryGet<IRepository<int, IReadOnlyHierarchyNode<BoneWrapper>>>(
				KEY_HANDLE_TO_BONE_MAP,
				out var handleToBoneMap))
			{
				handleToBoneMap = RepositoriesFactory.BuildDictionaryRepository<int, IReadOnlyHierarchyNode<BoneWrapper>>();

				context.AddOrUpdate<IRepository<int, IReadOnlyHierarchyNode<BoneWrapper>>>(
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
			bool enabled = ARToolboxEditorHelpers.BeginSubcontextBoxWithTitle(
				context,
				KEY_SKELETON_PREFIX,
				UI_TEXT_SKELETON_CAMELCASE,
				skeletonIndex);

			if (!enabled)
			{
				ARToolboxEditorHelpers.DrawSubcontextControls(
					context,
					skeletonIndex,
					totalSkeletonsCount,
					KEY_SKELETON_PREFIX);

				ARToolboxEditorHelpers.EndSubcontextBox();

				return;
			}

			DrawCurrentSelectedSkeletonDonor(
				context);

			DrawSanitize(
				context);

			DrawSampleSkeleton(
				context);

			EditorGUILayout.Separator();

			DrawHierarchy(
				context);

			DrawBoneDirectionGizmo(
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

		private void DrawSanitize(
			IARToolboxContext context)
		{
			if (!context.TryGet<bool>(
				KEY_SKELETON_SANITIZE,
				out var previousSanitize))
			{
				//Initialize to default value
				previousSanitize = true;

				context.AddOrUpdate(
					KEY_SKELETON_SANITIZE,
					previousSanitize);
			}

			var currentSanitize = EditorGUILayout.Toggle(
				UI_TEXT_SANITIZE,
				previousSanitize);

			if (currentSanitize != previousSanitize)
			{
				context.AddOrUpdate(
					KEY_SKELETON_SANITIZE,
					currentSanitize);
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

		private void DrawBoneDirectionGizmo(
			IARToolboxContext context)
		{
			if (!context.TryGet<SkeletonWrapper>(
				KEY_SKELETON,
				out var _))
			{
				return;
			}

			if (!context.TryGet<bool>(
				KEY_SKELETON_DRAW_BONE_DIRECTION_GIZMO,
				out var previousDrawGizmo))
			{
				//Initialize to default value
				previousDrawGizmo = true;

				context.AddOrUpdate(
					KEY_SKELETON_DRAW_BONE_DIRECTION_GIZMO,
					previousDrawGizmo);
			}

			var currentDrawGizmo = EditorGUILayout.Toggle(
				UI_TEXT_SHOW_BONE_DIRECTION,
				previousDrawGizmo);

			if (currentDrawGizmo != previousDrawGizmo)
			{
				context.AddOrUpdate(
					KEY_SKELETON_DRAW_BONE_DIRECTION_GIZMO,
					currentDrawGizmo);
			}
		}

		private void DrawHierarchy(
			IARToolboxContext context)
		{
			if (!context.TryGet<SkeletonWrapper>(
				KEY_SKELETON,
				out var skeleton))
			{
				return;
			}


			if (!context.TryGet<List<IReadOnlyHierarchyNode<BoneWrapper>>>(
				KEY_SKELETON_SELECTED_BONES,
				out var selectedBones))
			{
				selectedBones = new List<IReadOnlyHierarchyNode<BoneWrapper>>();

				context.AddOrUpdate(
					KEY_SKELETON_SELECTED_BONES,
					selectedBones);
			}


			if (!context.TryGet<bool>(
				KEY_SKELETON_DRAW_HIERARCHY,
				out var previousDrawHierarchy))
			{
				//Initialize to default value
				previousDrawHierarchy = true;

				context.AddOrUpdate(
					KEY_SKELETON_DRAW_HIERARCHY,
					previousDrawHierarchy);
			}

			bool currentDrawHierarchy = ARToolboxEditorHelpers.DrawFoldout(
				UI_TEXT_HIERARCHY,
				previousDrawHierarchy);

			if (currentDrawHierarchy != previousDrawHierarchy)
			{
				context.AddOrUpdate(
					KEY_SKELETON_DRAW_HIERARCHY,
					currentDrawHierarchy);
			}

			if (!currentDrawHierarchy)
			{
				return;
			}

			DrawSelectionControls(
				skeleton,
				selectedBones);


			DrawBoneInHierarchyRecursively(
				context,
				skeleton,
				skeleton.RootNode,
				selectedBones);

			if (context.TryGet<ChangeParentRequest>(
				KEY_SKELETON_BONES_CHANGE_PARENT_REQUEST,
				out var changeParentRequest))
			{
				var parentBone = changeParentRequest.Item1 as IHierarchyNode<BoneWrapper>;

				foreach (var boneChild in changeParentRequest.Item2)
				{
					parentBone.AddChild(boneChild);	
				}

				context.TryRemove(KEY_SKELETON_BONES_CHANGE_PARENT_REQUEST);
			}

			if (context.TryGet<RemoveBoneRequest>(
				KEY_SKELETON_BONE_REMOVE_REQUEST,
				out var boneRemoveRequest))
			{
				var boneNode = boneRemoveRequest.Item1 as IHierarchyNode<BoneWrapper>;

				var removeRecursively = boneRemoveRequest.Item2;

				if (boneNode.IsRoot)
				{
					if (!removeRecursively && boneNode.ChildCount == 1)
					{
						var childBone = boneNode.Children.First() as IHierarchyNode<BoneWrapper>;

						skeleton.RootNode = childBone;

						childBone.SetParent(null);

						selectedBones.Remove(boneNode);
					}
					else
					{
						skeleton.RootNode = null;
	
						selectedBones.Clear();
					}
				}
				else
				{
					if (removeRecursively)
					{
						var parentBone = boneNode.Parent as IHierarchyNode<BoneWrapper>;

						RemoveBoneRecursively(
							boneNode,
							selectedBones);

						parentBone.RemoveChild(boneNode);
					}
					else
					{
						var parentBone = boneNode.Parent as IHierarchyNode<BoneWrapper>;
	
						foreach (var childBone in boneNode.Children.ToArray())
						{
							parentBone.AddChild(childBone);
						}
	
						parentBone.RemoveChild(boneNode);

						selectedBones.Remove(boneNode);
					}
				}

				context.TryRemove(KEY_SKELETON_BONE_REMOVE_REQUEST);
			}
		}

		private void DrawSelectionControls(
			SkeletonWrapper skeleton,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones)
		{
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button(UI_TEXT_SELECT_ALL))
			{
				SelectAllBones(
					skeleton,
					selectedBones);
			}

			if (GUILayout.Button(UI_TEXT_DESELECT_ALL))
			{
				selectedBones.Clear();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void RemoveBoneRecursively(
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones)
		{
			foreach (var child in boneNode.Children.ToArray())
			{
				RemoveBoneRecursively(
					child,
					selectedBones);
			}

			var boneNodeCasted = boneNode as IHierarchyNode<BoneWrapper>;

			boneNodeCasted.RemoveAllChildren();

			selectedBones.Remove(boneNode);
		}

		private void DrawBoneInHierarchyRecursively(
			IARToolboxContext context,
			SkeletonWrapper skeleton,
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones,
			int depth = 0)
		{
			if (boneNode == null)
			{
				return;
			}

			if (boneNode.Contents != null)
			{
				EditorGUILayout.BeginHorizontal();

				StringBuilder stringBuilder = new StringBuilder();

				for (int i = 0; i < depth - 1; i++)
					stringBuilder.Append(UI_TEXT_HIERARCHY_PASSTHROUGH_PREFIX);

				if (depth > 0)
					stringBuilder.Append(UI_TEXT_HIERARCHY_CHILD_PREFIX);

				bool selected = selectedBones.Contains(boneNode);

				EditorGUILayout.LabelField(
					$"{stringBuilder.ToString()}{boneNode.Contents.BoneName}");

				if (selected)
				{
					if (ARToolboxEditorHelpers.DrawButtonTight(
						UI_TEXT_SELECTED))
					{
						selectedBones.Remove(boneNode);
					}
				}
				else
				{
					if (ARToolboxEditorHelpers.DrawButtonTight(
						UI_TEXT_SELECT))
					{
						selectedBones.Add(boneNode);
					}
				}

				bool anySelected = selectedBones.Count > 0;

				//There is no sence to set current bone to parent if there's noone to be parent to
				//There is no sence to set current bone to parent if it's already selected to be a child
				bool setParentActive = anySelected && !selected;

				//There is no sence to set current bone to parent if it is a descendant of any selected bone
				//Otherwise it will create a cycle which violates the tree structure and it also creates an island
				//which is not connected to the rest of the tree
				if (setParentActive)
				{
					var currentBone = boneNode;

					while (!currentBone.IsRoot)
					{
						if (selectedBones.Contains(currentBone))
						{
							setParentActive = false;

							break;
						}

						currentBone = currentBone.Parent;
					}
				}

				//There is no sence to set current bone to parent if it is already a parent of any selected bone
				//
				//Actually there is. It may be required to be a parent for some distant bone hierarchy
				//that is selected as well
				//
				//if (setParentActive)
				//{
				//	foreach (var selectedBone in selectedBones)
				//	{
				//		if (selectedBone.Parent == boneNode)
				//		{
				//			setParentActive = false;
				//
				//			break;
				//		}
				//	}
				//}

				if (!setParentActive)
					GUI.enabled = false;

				if (ARToolboxEditorHelpers.DrawButtonTight(
					UI_TEXT_SET_PARENT))
				{
					context.AddOrUpdate(
						KEY_SKELETON_BONES_CHANGE_PARENT_REQUEST,
						new ChangeParentRequest
						(
							boneNode,
							selectedBones.ToArray()
						));
				}

				if (!setParentActive)
					GUI.enabled = true;

				bool ctrlPressed = Event.current.control;

				if (ARToolboxEditorHelpers.DrawButtonTight(
					UI_TEXT_REMOVE))
				{
					context.AddOrUpdate(
						KEY_SKELETON_BONE_REMOVE_REQUEST,
						new RemoveBoneRequest(
							boneNode,
							ctrlPressed));
				}

				EditorGUILayout.EndHorizontal();
			}

			depth++;

			foreach (var child in boneNode.Children)
			{
				DrawBoneInHierarchyRecursively(
					context,
					skeleton,
					child,
					selectedBones,
					depth);
			}
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

			//Create a skeleton
			SampleBoneTransformRecursively(
				selectedSkeletonDonor.transform,
				skinnedMeshRenderer,
				out var rootNode);

			var skeleton = new SkeletonWrapper(
				selectedSkeletonDonor,
				rootNode);

			if (context.TryGet<bool>(
				KEY_SKELETON_SANITIZE,
				out var sanitize)
				&& sanitize)
			{
				SanitizeBones(
					skeleton,
					selectedSkeletonDonor.transform);
			}

			context.AddOrUpdate(
				KEY_SKELETON,
				skeleton);
			

			//Create a current pose snapshot
			var currentPose = SamplePose(skeleton);

			context.AddOrUpdate(
				KEY_SKELETON_CURRENT_POSE,
				currentPose);

			//Select all bones of the skeleton
			if (!context.TryGet<List<IReadOnlyHierarchyNode<BoneWrapper>>>(
				KEY_SKELETON_SELECTED_BONES,
				out var selectedBones))
			{
				selectedBones = new List<IReadOnlyHierarchyNode<BoneWrapper>>();

				context.AddOrUpdate(
					KEY_SKELETON_SELECTED_BONES,
					selectedBones);
			}

			SelectAllBones(
				skeleton,
				selectedBones);
		}

		#region Sample bone transform

		private void SampleBoneTransformRecursively(
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
				SampleBoneTransformRecursively(
					child,
					skinnedMeshRenderer,
					out var childNode);

				boneNode.AddChild(
					childNode);
			}
		}

		#endregion

		#region Sample pose

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

		#endregion

		#region Selecting bones

		private void SelectAllBones(
			SkeletonWrapper skeleton,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones)
		{
			selectedBones.Clear();

			SelectBoneRecursively(
				skeleton.RootNode,
				selectedBones);
		}

		private void SelectBoneRecursively(
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones)
		{
			if (boneNode.Contents != null)
			{
				selectedBones.Add(
					boneNode);
			}

			foreach (var child in boneNode.Children)
			{
				SelectBoneRecursively(
					child,
					selectedBones);
			}
		}

		#endregion

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

		#region Sanitizing

		private void SanitizeBones(
			SkeletonWrapper skeleton,
			Transform origin)
		{
			SanitizeBoneRecursively(
				skeleton.RootNode,
				origin);
		}

		private void SanitizeBoneRecursively(
			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			Transform origin)
		{
			TrySanitizePose(
				boneNode,
				origin);

			foreach (var child in boneNode.Children)
			{
				SanitizeBoneRecursively(
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

		#endregion

		#region Drawing handles

		//Courtesy of https://adroit-things.com/game-engine/exploring-unity-editor-handle-caps/
		private void DrawSkeletonHandles(
			IARToolboxContext context,
			ref int currentFreeHandle,
			IRepository<int, IReadOnlyHierarchyNode<BoneWrapper>> handleToBoneMap)
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

			if (!context.TryGet<List<IReadOnlyHierarchyNode<BoneWrapper>>>(
				KEY_SKELETON_SELECTED_BONES,
				out var selectedBones))
			{
				selectedBones = new List<IReadOnlyHierarchyNode<BoneWrapper>>();

				context.AddOrUpdate(
					KEY_SKELETON_SELECTED_BONES,
					selectedBones);
			}

			bool drawBoneDirectionGizmo =
				context.TryGet<bool>(
					KEY_SKELETON_DRAW_BONE_DIRECTION_GIZMO,
					out var drawGizmo)
				&& drawGizmo;

			var currentEvent = Event.current;

			if (currentEvent.type == EventType.Layout)
			{
				DrawBoneHandlesRecursively(
					ref currentFreeHandle,
					handleToBoneMap,
					EventType.Layout,
					drawBoneDirectionGizmo,

					skeleton.RootNode,
					currentPose,
					selectedBones);
			}

			if (currentEvent.type == EventType.Repaint)
			{
				DrawBoneHandlesRecursively(
					ref currentFreeHandle,
					handleToBoneMap,
					EventType.Repaint,
					drawBoneDirectionGizmo,

					skeleton.RootNode,
					currentPose,
					selectedBones);
			}

			if (currentEvent.type == EventType.MouseDown
				&& currentEvent.button == 0)
			{
				SelectClickedBone(
					skeleton,
					handleToBoneMap,
					selectedBones);
			}
		}

		private void DrawBoneHandlesRecursively(
			ref int currentFreeHandle,
			IRepository<int, IReadOnlyHierarchyNode<BoneWrapper>> handleToBoneMap,
			EventType eventType,
			bool drawBoneDirectionGizmo,

			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			PoseSnapshot currentPose,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones = null)
		{
			if (boneNode == null)
			{
				return;
			}

			TryDrawBoneHandles(
				ref currentFreeHandle,
				handleToBoneMap,
				eventType,
				drawBoneDirectionGizmo,

				boneNode,
				currentPose,
				selectedBones);

			foreach (var child in boneNode.Children)
			{
				DrawBoneHandlesRecursively(
					ref currentFreeHandle,
					handleToBoneMap,
					eventType,
					drawBoneDirectionGizmo,

					child,
					currentPose,
					selectedBones);
			}
		}

		private void TryDrawBoneHandles(
			ref int currentFreeHandle,
			IRepository<int, IReadOnlyHierarchyNode<BoneWrapper>> handleToBoneMap,
			EventType eventType,
			bool drawBoneDirectionGizmo,

			IReadOnlyHierarchyNode<BoneWrapper> boneNode,
			PoseSnapshot currentPose,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones = null)
		{
			if (boneNode == null)
			{
				return;
			}

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
				boneNode);

			currentFreeHandle++;


			bool selected = selectedBones.Contains(boneNode);


			//Set the bone color
			previousHandlesColor = Handles.color;

			Color boneColor = Color.white;

			switch (boneSnapshot.BoneMode)
			{
				case EBoneMode.BONE:
					boneColor = Color.white;
					break;

				case EBoneMode.LOCATOR:
					boneColor = Color.magenta;
					break;

				case EBoneMode.IK_TARGET:
					boneColor = Color.cyan;
					break;
			}

			if (!selected)
			{
				boneColor *= BONE_UNSELECTED_COLOR_MULTIPLICATIVE;
			}

			Handles.color = boneColor;

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
				if (selected && drawBoneDirectionGizmo)
				{
					ARToolboxEditorHelpers.DrawAdjustablePositionHandle(
						boneSnapshot.Position,
						boneSanitizedRotation,
						BONE_FORWARD_HANDLES_LENGTH);
				}
			}
		}

		//Courtesy of https://discussions.unity.com/t/how-to-drag-and-select-multiple-handles-in-editor-script/830359/3
		private void SelectClickedBone(
			SkeletonWrapper skeleton,
			IRepository<int, IReadOnlyHierarchyNode<BoneWrapper>> handleToBoneMap,
			List<IReadOnlyHierarchyNode<BoneWrapper>> selectedBones)
		{
			var handle = HandleUtility.nearestControl;

			if (handleToBoneMap.TryGet(
					handle,
					out var bone)
				&& BoneBelongsToSkeleton(
					skeleton,
					bone))
			{
				bool ctrlPressed = Event.current.control;

				bool shiftPressed = Event.current.shift;

				if (ctrlPressed && shiftPressed)
				{
					SelectAllBones(
						skeleton,
						selectedBones);
				}
				else if (ctrlPressed)
				{
					SelectBoneRecursively(
						bone,
						selectedBones);
				}
				else if (shiftPressed)
				{
					if (selectedBones.Contains(bone))
					{
						selectedBones.Remove(bone);
					}
					else
					{
						selectedBones.Add(bone);
					}
				}
				else
				{
					selectedBones.Clear();

					selectedBones.Add(bone);
				}
			}
		}

		private bool BoneBelongsToSkeleton(
			SkeletonWrapper skeleton,
			IReadOnlyHierarchyNode<BoneWrapper> bone)
		{
			var currentBone = bone;

			while (!currentBone.IsRoot)
			{
				if (currentBone.Parent == null)
				{
					return false;
				}

				currentBone = currentBone.Parent;
			}

			return currentBone == skeleton.RootNode;
		}

		#endregion
	}
}